using CommonTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class RemoteServerObject : MarshalByRefObject, IServer
    {
        private readonly Dictionary<string, string> clients;
        private readonly Dictionary<string, int> servers;

        private readonly int priority;
        private string leader;
        private int currentTicket;
        private int lastTicket;
        private Dictionary<string, int> tickets;
        private HashSet<string> broadcastedTickets;

        private readonly Dictionary<string, int> vector_clock;

        private readonly List<Meeting> meetings;
        private readonly List<Location> locations;

        private readonly string server_url;
        private readonly int max_faults;
        private readonly int max_delay;
        private readonly int min_delay;

        private bool frozen;
        private int currentPosition;
        private int lastPosition;
        DateTime delayUntil;

        public RemoteServerObject(string server_url, int max_faults, int max_delay, int min_delay, int priority, string leader, Dictionary<string, int> servers)
        {
            this.server_url = server_url;
            this.max_faults = max_faults;
            this.max_delay = max_delay;
            this.min_delay = min_delay;
            this.servers = servers;
            this.priority = priority;
            this.leader = leader;
            this.currentTicket = 0;
            this.lastTicket = 0;
            this.frozen = false;
            this.currentPosition = 0;
            this.lastPosition = 0;

            meetings = new List<Meeting>();
            locations = new List<Location>();
            clients = new Dictionary<string, string>();
            tickets = new Dictionary<string, int>();
            broadcastedTickets = new HashSet<string>();
            vector_clock = new Dictionary<string, int>();
            vector_clock.Add(server_url, 0);

            foreach (string url in servers.Keys)
            {
                vector_clock.Add(url, 0);
            }
        }

        private void MessageHandler()
        {
            lock (this)
            {
                if (frozen)
                {
                    int position = lastPosition++;
                    while (frozen || (!frozen && position != currentPosition))
                    {
                        Monitor.Wait(this);
                        if (!frozen && position == currentPosition)
                        {
                            ++currentPosition;
                            Monitor.PulseAll(this);
                            break;
                        }
                    }
                }
            }
            Random rnd = new Random();
            int delay = rnd.Next(min_delay, max_delay);

            if (DateTime.Now.AddMilliseconds(delay).CompareTo(delayUntil) < 0)
            {
                delay = DateTime.Now.Subtract(delayUntil).Milliseconds;
            }
            else
            {
                lock (this) //TODO: error lock(delayUntil) idk
                {
                    delayUntil = DateTime.Now.AddMilliseconds(delay);
                }
            }
            Thread.Sleep(delay);
        }

        /*
         * Sequence Commands
         */
        public Dictionary<string, int> UpdateVectorClock(Dictionary<string, int> vector)
        {
            if (vector.Count == 0)
            {
                return vector_clock;
            }
            foreach (KeyValuePair<string, int> seq in vector)
            {
                if (!(seq.Value <= vector_clock[seq.Key]))
                {
                    return vector;
                }
            }
            return vector_clock;
        }
        public void IncrementVectorClock(string sender_url)
        {
            Console.WriteLine("[IncrementVectorClock] entering");
            Monitor.Enter(vector_clock);
            int currentCount;
            // currentCount will be zero if the key id doesn't exist..
            vector_clock.TryGetValue(sender_url, out currentCount);
            vector_clock[sender_url] = currentCount + 1;
            // vector_clock[sender_url]++;
            Monitor.PulseAll(vector_clock);
            Monitor.Exit(vector_clock);
            Console.WriteLine("[IncrementVectorClock] leaving");
        }
        public void WaitCausalOrder(string sender_url, Dictionary<string, int> vector)
        {
            Console.WriteLine("[WaitCausalOrder] entering");
            Monitor.Enter(vector_clock);
            bool isTime = false;
            while (!isTime) {
                isTime = true;
                foreach (KeyValuePair<string, int> seq in vector)
                {
                    if (!(seq.Key == sender_url && seq.Value == vector_clock[seq.Key] + 1) || 
                        seq.Key != sender_url && !(seq.Value <= vector_clock[seq.Key]))
                    {
                        isTime = false;
                        Monitor.Wait(vector_clock);
                        break;
                    }
                }
            }
            Monitor.Exit(vector_clock);
            Console.WriteLine("[WaitCausalOrder] leaving");
        }
        public int RequestTicket(string topic)
        {
            Monitor.Enter(leader);
            while (leader == null)
            {
                Monitor.Wait(leader);
            }
            Monitor.Exit(leader);
            try
            {
                return ((IServer) Activator.GetObject(typeof(IServer), leader)).GetTicket(topic);
            }
            catch (SocketException e)
            {
                Console.WriteLine($"[{e.GetType().Name}] Error trying to contact <{server_url}>");
                servers.Remove(leader);
                Election();
            }
            return RequestTicket(topic);
        }

        public int GetTicket(string topic)
        {
            lock (this)
            {
                if (!tickets.Keys.Contains(topic))
                {
                    tickets[topic] = currentTicket++;
                }

                return tickets[topic];
            }
        }

        public void NextInTotalOrder(string lastTopic)
        {
            Console.WriteLine("[NextInTotalOrder] entering");
            Monitor.Enter(tickets);
            lastTicket = tickets[lastTopic];
            tickets.Remove(lastTopic);
            broadcastedTickets.Remove(lastTopic);
            if (tickets.ContainsValue(lastTicket + 1))
            {
                string topic = tickets.FirstOrDefault(x => x.Value == lastTicket + 1).Key;
                Meeting meeting = meetings.Find((m1) => m1.topic.Equals(topic));
                Monitor.Enter(meeting);
                Monitor.Pulse(meeting);
                Monitor.Exit(meeting);
            }
            Monitor.Exit(tickets);
            Console.WriteLine("[NextInTotalOrder] leaving");
        }

        /*
         * Client Management Commands
         */
        public void RegisterClient(string username, string client_url)
        {
            MessageHandler();
            Console.WriteLine($"[RegisterClient] '{username}' , '{client_url}'");
            if (!clients.ContainsKey(username))
            {
                clients[username] = client_url;
                ThreadPool.QueueUserWorkItem(state =>
                {
                    // Replicate the operation
                    // TODO: reliable broadcast?
                    foreach (string url in servers.Keys)
                    {
                        try
                        {
                            ((IServer) Activator.GetObject(typeof(IServer), url)).RegisterClient(username, client_url);
                        }
                        catch (SocketException e)
                        {
                            Console.WriteLine($"[{e.GetType().Name}] Error trying to contact <{url}>");
                        }
                    }
                });
            }
        }
        public void UnregisterClient(string username)
        {
            MessageHandler();
            Console.WriteLine($"[UnregisterClient] '{username}'");
            if (clients.Remove(username))
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    // Replicate the operation
                    // TODO: reliable broadcast?
                    foreach (string url in servers.Keys)
                    {
                        try
                        {
                            ((IServer) Activator.GetObject(typeof(IServer), url)).UnregisterClient(username);
                        }
                        catch (SocketException e)
                        {
                            Console.WriteLine($"[{e.GetType().Name}] Error trying to contact <{url}>");
                        }
                    }
                });
            }
        }
        public Dictionary<string, string> GetClients()
        {
            Console.WriteLine("[GetClients]");
            return this.clients;
        }

        /*
         * Leader Election (Bully's Algorithm)
         * 
         */
        public void Election()
        {
            Monitor.Enter(leader);
            leader = null;
            Monitor.Exit(leader);
            if (!servers.Values.Any((e) => e > priority))
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    foreach (KeyValuePair<string, int> server in servers)
                    {
                        if (server.Value < priority)
                        {
                            ((IServer) Activator.GetObject(typeof(IServer), server.Key)).Elected(server_url);
                        }
                    }
                });
                Elected(server_url);
                return;
            }
            ThreadPool.QueueUserWorkItem(state =>
            {
                bool success = false;
                foreach (KeyValuePair<string, int> server in servers)
                {
                    if (server.Value > priority)
                    {
                        try
                        {
                            ((IServer) Activator.GetObject(typeof(IServer), server.Key)).Election();
                            success = true;
                        }
                        catch (SocketException e)
                        {
                            Console.WriteLine($"[{e.GetType().Name}] Error trying to contact <{server_url}>");
                            servers.Remove(server.Key);
                        }
                    }
                }
                if (success)
                {
                    Monitor.Enter(leader);
                    if (leader == null)
                    {
                        Monitor.Wait(leader, 5_000);
                    }
                    if (leader == null) // Timeout (Re-election)
                    {
                        Monitor.Exit(leader);
                        Election();
                    }
                    else
                    {
                        Monitor.Exit(leader);
                    }
                }
            });
        }

        public void Elected(string leader)
        {
            Monitor.Enter(this.leader);
            this.leader = leader;
            Monitor.PulseAll(this.leader);
            Monitor.Exit(this.leader);
        }

        /*
         * Business Commands
         */
        public List<Meeting> GetMeetings(Dictionary<string, int> vector, List<Meeting> clientMeetings)
        {
            Console.WriteLine("[GetMeetings] " + string.Join(",", meetings.Select(m => m.topic)));
            MessageHandler();
            WaitCausalOrder(String.Empty, vector);
            return meetings.FindAll(m => clientMeetings.Exists(m2 => m.topic.Equals(m2.topic)));
        }
        public void CreateMeeting(Dictionary<string, int> vector, Meeting m)
        {
            Console.WriteLine("[CreateMeeting] " + m);
            MessageHandler();
            WaitCausalOrder(String.Empty, vector);
            if (!meetings.Contains(m))
            {
                foreach (Slot s in m.slots)
                {
                    if (!locations.Exists(l => l.name.Equals(s.location)))
                    {
                        throw new ApplicationException($"The meeting {m.topic} has a slot with an unknown location {s.location}.");
                    }
                }
                // Replicate the operation
                IncrementVectorClock(server_url);
                ThreadPool.QueueUserWorkItem(state =>
                {
                    foreach (string url in servers.Keys)
                    {
                        try
                        {
                            ((IServer) Activator.GetObject(typeof(IServer), url)).RBCreateMeeting(server_url, vector_clock, m);
                        }
                        catch (SocketException e)
                        {
                            Console.WriteLine($"[{e.GetType().Name}] Error trying to contact <{url}>");
                        }
                    }
                });
                meetings.Add(m);
            }
        }
        public void RBCreateMeeting(string sender_url, Dictionary<string,int> vector, Meeting m)
        {
            Console.WriteLine($"[RBCreateMeeting] {sender_url} {vector.Values} {m}");
            WaitCausalOrder(sender_url, vector);
            Monitor.Enter(meetings);
            if (!meetings.Contains(m))
            {
                Monitor.Exit(meetings);
                ThreadPool.QueueUserWorkItem(state =>
                {
                    foreach (string url in servers.Keys)
                    {
                        try
                        {
                            ((IServer) Activator.GetObject(typeof(IServer), url)).RBCreateMeeting(sender_url, vector, m);
                        }
                        catch (SocketException e)
                        {
                            Console.WriteLine($"[{e.GetType().Name}] Error trying to contact <{url}>");
                        }
                    }
                });
                meetings.Add(m);
                IncrementVectorClock(sender_url);
            }
        }
        public void JoinMeeting(string user, Dictionary<string, int> vector, string meetingTopic, List<Slot> slots)
        {
            Console.WriteLine($"[JoinMeeting] {user}, {meetingTopic}");
            MessageHandler();
            WaitCausalOrder(String.Empty, vector);
            Meeting meeting = meetings.Find((m1) => m1.topic.Equals(meetingTopic));
            if (meeting == null)
            {
                throw new ApplicationException($"The meeting {meetingTopic} does not exist.");
            }
            Monitor.Enter(meeting);
            if (meeting.status > CommonTypes.Status.Open)
            {
                Monitor.Exit(meeting);
                throw new ApplicationException($"The meeting {meetingTopic} is either closed or cancelled.");
            }
            bool joined = meeting.Join(user, slots);
            Monitor.Exit(meeting);
            if (joined)
            {
                IncrementVectorClock(server_url);
                List<EventWaitHandle> handles = new List<EventWaitHandle>(this.servers.Count);
                int i = 0;
                foreach (string url in servers.Keys) // Replicate the operation
                {
                    handles.Add(new AutoResetEvent(false));
                    Task.Factory.StartNew((state) =>
                    {
                        int j = (int) state;
                        ((IServer) Activator.GetObject(typeof(IServer), url)).RBJoinMeeting(server_url, vector_clock, user, meetingTopic, slots);
                        handles[j].Set();
                    }, i++);
                }
                for (i = 0; i < max_faults + 1; i++) // Wait for the responses
                {
                    int idx = WaitHandle.WaitAny(handles.ToArray());
                    handles.RemoveAt(idx);
                }
            }
        }
        public void RBJoinMeeting(string sender_url, Dictionary<string,int> vector, string user, string meetingTopic, List<Slot> slots)
        {
            Console.WriteLine($"[RBJoinMeeting] {sender_url}, {user}, {meetingTopic}");
            WaitCausalOrder(sender_url, vector);
            Meeting meeting = meetings.Find((m1) => m1.topic.Equals(meetingTopic));
            Monitor.Enter(meeting);
            bool joined = meeting.Join(user, slots);
            Monitor.Exit(meeting);
            if (joined)
            {
                List<EventWaitHandle> handles = new List<EventWaitHandle>(this.servers.Count - 1);
                int i = 0;
                foreach (string url in servers.Keys)
                {
                    if (url != sender_url)
                    {
                        Task.Factory.StartNew((state) =>
                        {
                            int j = (int) state;
                            ((IServer) Activator.GetObject(typeof(IServer), url)).RBJoinMeeting(sender_url, vector, user, meetingTopic, slots);
                            handles[j].Set();
                        }, i++);
                    }
                }
                for (i = 0; i < max_faults + 1; i++) // Wait for the responses
                {
                    int idx = WaitHandle.WaitAny(handles.ToArray());
                    handles.RemoveAt(idx);
                }
                IncrementVectorClock(sender_url);
            }
        }
        public void CloseMeeting(Dictionary<string, int> vector, string user, string meetingTopic)
        {
            Console.WriteLine($"[CloseMeeting] {user}, {meetingTopic}");
            MessageHandler();
            WaitCausalOrder(String.Empty, vector);
            Meeting meeting = meetings.Find((m1) => m1.topic.Equals(meetingTopic));
            if (meeting == null)
            {
                throw new ApplicationException($"The meeting {meetingTopic} do not exist.");
            }
            if (meeting.status == CommonTypes.Status.Closed || meeting.status == CommonTypes.Status.Cancelled)
            {
                return;
            }
            if (!user.Equals(meeting.coordinator))
            {
                throw new ApplicationException($"You are not authorized to close the meeting {meetingTopic}.");
            }
            IncrementVectorClock(server_url);
            List<EventWaitHandle> handles = new List<EventWaitHandle>(this.servers.Count);
            int i = 0;
            Monitor.Enter(meeting);
            meeting.status = CommonTypes.Status.Closing;
            Monitor.Exit(meeting);
            foreach (string url in servers.Keys) // Replicate the operation
            {
                handles.Add(new AutoResetEvent(false));
                Task.Factory.StartNew((state) =>
                {
                    int j = (int)state;
                    ((IServer)Activator.GetObject(typeof(IServer), url)).RBCloseMeeting(server_url, vector_clock, meetingTopic);
                    handles[j].Set();
                }, i++);
            }
            int ticket = RequestTicket(meetingTopic);
            RBCloseTicket(meetingTopic, ticket);
            for (i = 0; i < max_faults + 1; i++) // Wait for the responses
            {
                int idx = WaitHandle.WaitAny(handles.ToArray());
                handles.RemoveAt(idx);
            }
            Monitor.Enter(meeting);
            while (tickets[meetingTopic] > lastTicket + 1)
            {
                // TODO: Add timeout to Wait [bool Wait(Object, Int32)]
                Monitor.Wait(meeting);
            }
            CloseOperation(meeting);
            Monitor.Exit(meeting);
            NextInTotalOrder(meetingTopic);
        }
        public void RBCloseMeeting(string sender_url, Dictionary<string, int> vector, string meetingTopic)
        {
            Console.WriteLine($"[RBCloseMeeting] {sender_url}, {meetingTopic}");
            MessageHandler();
            WaitCausalOrder(sender_url, vector);
            Meeting meeting = meetings.Find((m1) => m1.topic.Equals(meetingTopic));
            Monitor.Enter(meeting);
            if (meeting.status > CommonTypes.Status.Open)
            {
                Monitor.Exit(meeting);
                return;
            }
            meeting.status = CommonTypes.Status.Closing;
            Monitor.Exit(meeting);
            List<EventWaitHandle> handles = new List<EventWaitHandle>(this.servers.Count);
            int i = 0;
            foreach (string url in servers.Keys)
            {
                if (url != sender_url)
                {
                    handles.Add(new AutoResetEvent(false));
                    Task.Factory.StartNew((state) =>
                    {
                        int j = (int) state;
                        ((IServer) Activator.GetObject(typeof(IServer), url)).RBCloseMeeting(server_url, vector, meetingTopic);
                        handles[j].Set();
                    }, i++);
                }
            }
            while (!tickets.ContainsKey(meetingTopic) || tickets[meetingTopic] > lastTicket + 1)
            {
                if (!Monitor.Wait(meeting, 2000))
                {
                    int ticket = RequestTicket(meetingTopic);
                    RBCloseTicket(meetingTopic, ticket);
                }
            }
            for (i = 0; i < max_faults + 1; i++) // Wait for the responses
            {
                int idx = WaitHandle.WaitAny(handles.ToArray());
                handles.RemoveAt(idx);
            }
            Monitor.Enter(meeting);
            CloseOperation(meeting);
            Monitor.Exit(meeting);
            NextInTotalOrder(meetingTopic);
        }
        public void CloseOperation(Meeting meeting)
        {
            Slot slot = null;
            List<Room> rooms = null;
            List<Room> locked = null;
            foreach (Slot s in meeting.slots) // Find the best slot for the meeting
            {
                Location location = locations.Find(l => l.name.Equals(s.location));
                Monitor.Enter(location.rooms); // Lock the rooms on the current location
                List<Room> free = location.rooms.FindAll(r => !r.booked.Contains(s.date));
                if (free.Count > 0) // There is a free room
                    if (s.participants.Count >= meeting.min_participants && slot == null || slot != null && s.participants.Count > slot.participants.Count)
                    {
                        if (locked != null)
                        {
                            Monitor.Exit(locked); // Release the lock of previous location rooms locked
                        }
                        locked = location.rooms;
                        slot = s;
                        rooms = free;
                    }
                if (rooms != free)
                {
                    Monitor.Exit(location.rooms); // Unlock if the rooms of the current location were not selected
                }
            }
            if (slot == null)
            {
                meeting.status = CommonTypes.Status.Cancelled;
            }
            else
            {
                Room room = null;
                foreach (Room r in rooms) // Find the best room for the meeting
                {
                    if (room == null || slot.participants.Count > room.capacity && room.capacity < r.capacity)
                    {
                        room = r;
                        if (room.capacity == slot.participants.Count)
                        {
                            break;
                        }
                    }
                }
                room.booked.Add(slot.date); // Book the room
                Monitor.Exit(locked);
                if (room.capacity < slot.participants.Count)
                {
                    // If there are more registered participants than the capacity of the selected meeting room
                    slot.participants.RemoveRange(room.capacity, slot.participants.Count - room.capacity);
                }

                meeting.room = room;
                meeting.slot = slot;
                meeting.status = CommonTypes.Status.Closed;
            }
        }
        public void RBCloseTicket(string topic, int ticket)
        {
            if (broadcastedTickets.Contains(topic))
                return;

            broadcastedTickets.Add(topic);
            
            List<EventWaitHandle> handles = new List<EventWaitHandle>(this.servers.Count);
            int i = 0;
            foreach (string url in servers.Keys) // Replicate the operation
            {
                handles.Add(new AutoResetEvent(false));
                Task.Factory.StartNew((state) =>
                {
                    int j = (int) state;
                    ((IServer) Activator.GetObject(typeof(IServer), url)).RBCloseTicket(topic, ticket);
                    handles[j].Set();
                }, i++);
            }
            for (i = 0; i < max_faults + 1; i++) // Wait for the responses
            {
                int idx = WaitHandle.WaitAny(handles.ToArray());
                handles.RemoveAt(idx);
            }
            Monitor.Enter(tickets);
            tickets[topic] = ticket;
            if (leader != server_url && currentTicket < ticket)
                currentTicket = ticket;
            Monitor.Exit(tickets);
            if (tickets[topic] == lastTicket + 1)
            {
                Meeting meeting = meetings.Find((m1) => m1.topic.Equals(topic));
                Monitor.Enter(meeting);
                Monitor.Pulse(meeting);
                Monitor.Exit(meeting);
            }
        }
        public void AddRoom(string location_name, int capacity, string room_name)
        {
            Console.WriteLine($"[AddRoom] {location_name}, {room_name}, {capacity}");
            Location location = locations.Find(l => l.name.Equals(location_name));
            if (location == null)
            {
                location = new Location(location_name);
                locations.Add(location);
            }

            Room room = new Room(room_name, capacity);
            if (!location.rooms.Contains(room))
            {
                location.rooms.Add(room);
            }

        }
        public void Status()
        {
            Console.WriteLine("[Status]");
            Console.WriteLine("Vector Clock:");
            foreach(KeyValuePair<string, int> seq in vector_clock)
            {
                Console.WriteLine($"  {seq.Key} -> {seq.Value}");
            }
            Console.WriteLine("Clients:");
            foreach (string client in clients.Values)
            {
                Console.WriteLine($"  {client}");
            }
            Console.WriteLine("Servers:");
            foreach (string server in servers.Keys)
            {
                Console.WriteLine($"  {server}");
            }
            Console.WriteLine("Meetings:");
            foreach (Meeting m in meetings)
            {
                m.PrettyPrint();
            }
        }

        /*
         * Debuggings Commands
         */
        public void Crash()
        {
            Console.WriteLine("[Crash]");
            Process.GetCurrentProcess().Kill();
        }
        public void Freeze()
        {
            Console.WriteLine("[Freeze]");
            lock (this)
            {
                frozen = true;
            }
        }
        public void Unfreeze()
        {
            Console.WriteLine("[Unfreeze]");
            lock (this)
            {
                frozen = false;
                Monitor.PulseAll(this);
            }
        }
        public override object InitializeLifetimeService()
        {
            return null;
        }

        public string GetAlternativeServer()
        {
            Random rand = new Random();
            int i = rand.Next(servers.Count);
            return servers.Keys.ElementAt(i);
        }

        public void Ping() { }
    }
}
