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
        private HashSet<string> crashed_servers;

        private readonly int priority;
        private string leader;
        private object leader_lock;
        private int currentTicket;
        private int lastTicket;
        private Dictionary<string, int> tickets;
        private HashSet<string> broadcastedTickets;

        private readonly VectorClock vectorClock;

        private readonly List<Meeting> meetings;
        private readonly List<Location> locations;

        private readonly string server_url;
        private readonly int max_faults;
        private readonly int max_delay;
        private readonly int min_delay;
        private int gossip_count;
        private int current_faults;
        private object faults_lock;

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
            this.gossip_count = 0;
            this.current_faults = 0;
            this.faults_lock = new object();

            this.servers = servers;
            this.priority = priority;
            this.leader = leader;
            this.leader_lock = new object();
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
            crashed_servers = new HashSet<string>();

            vectorClock = new VectorClock(server_url);
            vectorClock.Init(servers.Keys);
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
        public VectorClock UpdateVectorClock(VectorClock vector)
        {
            if (vector.Count == 0)
            {
                return vectorClock;
            }
            foreach (KeyValuePair<string, int> seq in vector.vector)
            {
                if (!(seq.Value <= vectorClock[seq.Key]))
                {
                    return vector;
                }
            }
            return vectorClock;
        }
        public void IncrementVectorClock(string sender_url)
        {
            Console.WriteLine("[IncrementVectorClock] entering");
            Monitor.Enter(vectorClock);
            // currentCount will be zero if the key id doesn't exist..
            vectorClock.TryGetValue(sender_url, out int currentCount);
            vectorClock[sender_url] = currentCount + 1;
            // vector_clock[sender_url]++;
            Monitor.PulseAll(vectorClock);
            Monitor.Exit(vectorClock);
            Console.WriteLine("[IncrementVectorClock] leaving");
        }
        public void WaitCausalOrder(string sender_url, VectorClock vector)
        {
            Console.WriteLine("[WaitCausalOrder] entering");
            Monitor.Enter(vectorClock);

            while (vectorClock.Delay(vector))
            {
                Monitor.Wait(vectorClock);
            }

            Monitor.Exit(vectorClock);
            Console.WriteLine("[WaitCausalOrder] leaving");
        }
        public int RequestTicket(string topic)
        {
            Monitor.Enter(leader_lock);
            while (leader == null)
            {
                Monitor.Wait(leader_lock);
            }
            Monitor.Exit(leader_lock);
            Console.WriteLine($"[RequestTicket] Requesting {leader}");
            try
            {
                return ((IServer) Activator.GetObject(typeof(IServer), leader)).GetTicket(topic);
            }
            catch (SocketException e)
            {
                Console.WriteLine($"[{e.GetType().Name} @ RequestTicket] Error trying to contact <{leader}>");
                ServerCrash(leader);
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
                    tickets[topic] = ++currentTicket;
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
                            Console.WriteLine($"[{e.GetType().Name} @ RegisterClient] Error trying to contact <{url}>");
                        }
                    }
                });
                int value = (int) ((double) clients.Count + 0.5) / 2;
                gossip_count = value > 3 ? 3 : value; // Which value guarantees that all clients receive the meeting?
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
                            Console.WriteLine($"[{e.GetType().Name} @ UnregisterClient] Error trying to contact <{url}>");
                        }
                    }
                });
            }
        }
        public Dictionary<string, string> GetClients()
        {
            MessageHandler();
            Console.WriteLine("[GetClients]");
            return this.clients;
        }
        public List<string> GetGossipClients(string vetoUrl, Meeting m)
        {
            MessageHandler();
            Console.WriteLine($"[GetGossipClients] veto: {vetoUrl}");
            List<string> gossip_clients = new List<string>(gossip_count);
            Random rand = new Random();
            if (m.invitees.Count > 0)
            {
                for (int i = 0; i < gossip_count; ++i)
                {
                    int j = rand.Next(m.invitees.Count);
                    string url = clients[m.invitees[j]];
                    if (url != vetoUrl)
                    {
                        gossip_clients.Add(url);
                    }
                    else
                    {
                        i--;
                    }
                }
            }
            else
            {
                List<string> clients_urls = clients.Values.ToList();
                for (int i = 0; i < gossip_count; ++i)
                {
                    int j = rand.Next(clients.Count);
                    string url = clients_urls[j];

                    if (url != vetoUrl)
                    {
                        gossip_clients.Add(url);
                    }
                    else
                    {
                        i--;
                    }
                }
            }
            return gossip_clients;
        }

        /*
         * Leader Election (Bully's Algorithm)
         * 
         */
        public void Election()
        {
            Console.WriteLine("[Election]");
            MessageHandler();
            Monitor.Enter(leader_lock);
            leader = null;
            Monitor.Exit(leader_lock);
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
                            Console.WriteLine($"[{e.GetType().Name} @ Election] Error trying to contact <{server.Key}>");
                            ServerCrash(server.Key);
                        }
                    }
                }
                if (success)
                {
                    Monitor.Enter(leader_lock);
                    if (leader == null)
                    {
                        Monitor.Wait(leader_lock, 5_000);
                    }
                    if (leader == null) // Timeout (Re-election)
                    {
                        Monitor.Exit(leader_lock);
                        Election();
                    }
                    else
                    {
                        Monitor.Exit(leader_lock);
                    }
                }
            });
        }
        public void Elected(string leader)
        {
            Console.WriteLine($"[Elected] {leader}");
            Monitor.Enter(leader_lock);
            this.leader = leader;
            Monitor.PulseAll(leader_lock);
            Monitor.Exit(leader_lock);
        }

        /*
         * Business Commands
         */
        public List<Meeting> GetMeetings(VectorClock vector, List<Meeting> clientMeetings)
        {
            MessageHandler();
            Console.WriteLine("[GetMeetings] " + string.Join(",", meetings.Select(m => m.topic)));
            WaitCausalOrder(String.Empty, vector);
            return meetings.FindAll(m => clientMeetings.Exists(m2 => m.topic.Equals(m2.topic)));
        }
        public void CreateMeeting(VectorClock vector, Meeting m)
        {
            MessageHandler();
            Console.WriteLine("[CreateMeeting] " + m);
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
                            ((IServer) Activator.GetObject(typeof(IServer), url)).RBCreateMeeting(server_url, vectorClock, m);
                        }
                        catch (SocketException e)
                        {
                            Console.WriteLine($"[{e.GetType().Name} @ CreateMeeting] Error trying to contact <{url}>");
                            ServerCrash(url);
                        }
                    }
                });
                meetings.Add(m);
            }
        }
        public void RBCreateMeeting(string sender_url, VectorClock vector, Meeting m)
        {
            MessageHandler();
            Console.WriteLine($"[RBCreateMeeting] {sender_url} {vector} {m}");
            WaitCausalOrder(sender_url, vector);
            Monitor.Enter(meetings);
            if (!meetings.Contains(m))
            {
                meetings.Add(m);
                ThreadPool.QueueUserWorkItem(state =>
                {
                    foreach (string url in servers.Keys)
                    {
                        if (url != sender_url)
                        {
                            try
                            {
                                ((IServer)Activator.GetObject(typeof(IServer), url)).RBCreateMeeting(sender_url, vector, m);
                            }
                            catch (SocketException e)
                            {
                                Console.WriteLine($"[{e.GetType().Name} @ RBCreateMeeting] Error trying to contact <{url}>");
                                ServerCrash(url);
                            }
                        }
                    }
                });
                IncrementVectorClock(sender_url);
            }
            Monitor.Exit(meetings);
        }
        public void JoinMeeting(string user, VectorClock vector, string meetingTopic, List<Slot> slots)
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
                List<EventWaitHandle> handles = new List<EventWaitHandle>();
                int i = 0;
                foreach (string url in servers.Keys) // Replicate the operation
                {
                    handles.Add(new AutoResetEvent(false));
                    Task.Factory.StartNew((state) =>
                    {
                        int j = (int) state;
                        try
                        {
                            ((IServer) Activator.GetObject(typeof(IServer), url)).RBJoinMeeting(server_url, vectorClock, user, meetingTopic, slots);
                            handles[j].Set();
                        }
                        catch (SocketException e)
                        {
                            Console.WriteLine($"[{e.GetType().Name} @ JoinMeeting] Error trying to contact <{url}>");
                            ServerCrash(url);
                        }
                    }, i++);
                }
                for (i = 0; i < max_faults - current_faults; i++) // Wait for the responses
                {
                    int idx = WaitHandle.WaitAny(handles.ToArray(), 1000);
                    if (idx == WaitHandle.WaitTimeout && max_faults - current_faults < 1)
                    {
                        Console.WriteLine("[JoinMeeting] No more ACKs");
                        break;
                    }
                    else if (idx == WaitHandle.WaitTimeout)
                    {
                        // Delay receiving ACKs
                        i--;
                        continue;
                    }
                    handles.RemoveAt(idx);
                }
            }
        }
        public void RBJoinMeeting(string sender_url, VectorClock vector, string user, string meetingTopic, List<Slot> slots)
        {
            MessageHandler();
            Console.WriteLine($"[RBJoinMeeting] {sender_url}, {user}, {meetingTopic}");
            WaitCausalOrder(sender_url, vector);
            Meeting meeting = meetings.Find((m1) => m1.topic.Equals(meetingTopic));
            Monitor.Enter(meeting);
            bool joined = meeting.Join(user, slots);
            Monitor.Exit(meeting);
            if (joined)
            {
                List<EventWaitHandle> handles = new List<EventWaitHandle>();
                int i = 0;
                foreach (string url in servers.Keys)
                {
                    if (url != sender_url)
                    {
                        handles.Add(new AutoResetEvent(false));
                        Task.Factory.StartNew((state) =>
                        {
                            int j = (int) state;
                            try
                            {
                                ((IServer) Activator.GetObject(typeof(IServer), url)).RBJoinMeeting(sender_url, vector, user, meetingTopic, slots);
                                handles[j].Set();
                            }
                            catch (SocketException e)
                            {
                                Console.WriteLine($"[{e.GetType().Name} @ RBJoinMeeting] Error trying to contact <{url}>");
                                ServerCrash(url);
                            }
                        }, i++);
                    }
                }
                for (i = 0; i < max_faults - current_faults - 1; i++) // Wait for the responses
                {
                    int idx = WaitHandle.WaitAny(handles.ToArray(), 1000);
                    if (idx == WaitHandle.WaitTimeout && max_faults - current_faults < 1)
                    {
                        Console.WriteLine("[RBJoinMeeting] No more ACKs");
                        break;
                    }
                    else if (idx == WaitHandle.WaitTimeout)
                    {
                        // Delay receiving ACKs
                        i--;
                        continue;
                    }
                    handles.RemoveAt(idx);
                }
                IncrementVectorClock(sender_url);
            }
        }
        public void CloseMeeting(VectorClock vector, string user, string meetingTopic)
        {
            MessageHandler();
            Console.WriteLine($"[CloseMeeting] {user}, {meetingTopic}");
            WaitCausalOrder(String.Empty, vector);
            Meeting meeting = meetings.Find((m1) => m1.topic.Equals(meetingTopic));
            if (meeting == null)
            {
                throw new ApplicationException($"The meeting {meetingTopic} do not exist.");
            }
            if (meeting.status > CommonTypes.Status.Open)
            {
                return;
            }
            if (!user.Equals(meeting.coordinator))
            {
                throw new ApplicationException($"You are not authorized to close the meeting {meetingTopic}.");
            }
            IncrementVectorClock(server_url);
            List<EventWaitHandle> handles = new List<EventWaitHandle>();
            int i = 0;
            Monitor.Enter(meeting);
            meeting.status = CommonTypes.Status.Closing;
            Monitor.Exit(meeting);
            foreach (string url in servers.Keys) // Replicate the operation
            {
                handles.Add(new AutoResetEvent(false));
                Task.Factory.StartNew((state) =>
                {
                    int j = (int) state;
                    try
                    {
                        ((IServer) Activator.GetObject(typeof(IServer), url)).RBCloseMeeting(server_url, vectorClock, meetingTopic);
                        handles[j].Set();
                    }
                    catch (SocketException e)
                    {
                        Console.WriteLine($"[{e.GetType().Name} @ CloseMeeting] Error trying to contact <{url}>");
                        ServerCrash(url);
                    }
                }, i++);
            }
            int ticket = RequestTicket(meetingTopic);
            RBCloseTicket(server_url, meetingTopic, ticket);
            for (i = 0; i < max_faults - current_faults; i++) // Wait for the responses
            {
                int idx = WaitHandle.WaitAny(handles.ToArray(), 1000);
                if (idx == WaitHandle.WaitTimeout && max_faults - current_faults < 1)
                {
                    Console.WriteLine("[CloseMeeting] No more ACKs");
                    break;
                }
                else if (idx == WaitHandle.WaitTimeout)
                {
                    // Delay receiving ACKs
                    i--;
                    continue;
                }
                handles.RemoveAt(idx);
            }
            Monitor.Enter(meeting);
            while (tickets[meetingTopic] > lastTicket + 1)
            {
                Monitor.Wait(meeting);
            }
            CloseOperation(meeting);
            Monitor.Exit(meeting);
            NextInTotalOrder(meetingTopic);
        }
        public void RBCloseMeeting(string sender_url, VectorClock vector, string meetingTopic)
        {
            MessageHandler();
            Console.WriteLine($"[RBCloseMeeting] {sender_url}, {meetingTopic}");
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
            List<EventWaitHandle> handles = new List<EventWaitHandle>();
            int i = 0;
            foreach (string url in servers.Keys)
            {
                if (url != sender_url)
                {
                    handles.Add(new AutoResetEvent(false));
                    Task.Factory.StartNew((state) =>
                    {
                        int j = (int) state;
                        try
                        {
                            ((IServer) Activator.GetObject(typeof(IServer), url)).RBCloseMeeting(server_url, vectorClock, meetingTopic);
                            handles[j].Set();
                        }
                        catch (SocketException e)
                        {
                            Console.WriteLine($"[{e.GetType().Name} @ RBCloseMeeting] Error trying to contact <{url}>");
                            ServerCrash(url);
                        }
                    }, i++);
                }
            }
            while (!tickets.ContainsKey(meetingTopic) || tickets[meetingTopic] > lastTicket + 1)
            {
                if (!Monitor.Wait(meeting, 2000))
                {
                    if (tickets.ContainsKey(meetingTopic) && tickets[meetingTopic] > lastTicket + 1)
                        continue;
                    Console.WriteLine($"[RBCloseMeeting] ticket for {meetingTopic} not received, requesting and broadcasting");
                    int ticket = RequestTicket(meetingTopic);
                    RBCloseTicket(server_url, meetingTopic, ticket);
                }
            }
            for (i = 0; i < max_faults - current_faults - 1; i++) // Wait for the responses
            {
                int idx = WaitHandle.WaitAny(handles.ToArray(), 1000);
                if (idx == WaitHandle.WaitTimeout && max_faults - current_faults < 1)
                {
                    Console.WriteLine("[RBCloseMeeting] No more ACKs");
                    break;
                }
                else if (idx == WaitHandle.WaitTimeout)
                {
                    // Delay receiving ACKs
                    i--;
                    continue;
                }
                handles.RemoveAt(idx);
            }
            Monitor.Enter(meeting);
            CloseOperation(meeting);
            Monitor.Exit(meeting);
            IncrementVectorClock(server_url);
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
        public void RBCloseTicket(string sender_url, string topic, int ticket)
        {
            MessageHandler();
            Console.WriteLine($"[RBCloseTicket] {topic} {ticket}");
            if (broadcastedTickets.Contains(topic))
            {
                return;
            }

            broadcastedTickets.Add(topic);

            List<EventWaitHandle> handles = new List<EventWaitHandle>();
            int i = 0;
            Monitor.Enter(servers);
            foreach (string url in servers.Keys) // Replicate the operation
            {
                Monitor.Exit(servers);
                if (url != sender_url)
                {
                    handles.Add(new AutoResetEvent(false));
                    Task.Factory.StartNew((state) =>
                    {
                        int j = (int) state;
                        try
                        {
                            ((IServer) Activator.GetObject(typeof(IServer), url)).RBCloseTicket(server_url, topic, ticket);
                            handles[j].Set();
                        }
                        catch (SocketException e)
                        {
                            Console.WriteLine($"[{e.GetType().Name} @ RBCloseTicket] Error trying to contact <{url}>");
                            ServerCrash(url);
                        }
                    }, i++);
                }
                Monitor.Enter(servers);
            }
            Monitor.Exit(servers);
            Monitor.Enter(faults_lock);
            for (i = 0; i < max_faults - current_faults - 1; i++) // Wait for the responses
            {
                Monitor.Exit(faults_lock);
                int idx = WaitHandle.WaitAny(handles.ToArray(), 1000);
                Monitor.Enter(faults_lock);
                if (idx == WaitHandle.WaitTimeout && max_faults - current_faults < 1)
                {
                    Console.WriteLine("[RBCloseTicket] No more ACKs");
                    break;
                }
                else if (idx == WaitHandle.WaitTimeout)
                {
                    // Delay receiving ACKs
                    i--;
                    continue;
                }
                handles.RemoveAt(idx);
            }
            Monitor.Exit(faults_lock);
            Monitor.Enter(tickets);
            tickets[topic] = ticket;
            if (leader != server_url && currentTicket < ticket)
                currentTicket = ticket;
            Monitor.Exit(tickets);
            if (tickets[topic] == lastTicket + 1)
            {
                Console.WriteLine("[RBCloseTicket] pulsing meeting");
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
        public void ServerCrash(string crash_url)
        {
            Console.WriteLine($"[ServerCrash] {crash_url}");
            if (crashed_servers.Contains(crash_url)) return;
            Monitor.Enter(faults_lock);
            current_faults++;
            Monitor.Exit(faults_lock);
            RBServerCrash(server_url, crash_url);
        }
        public void RBServerCrash(string sender_url, string crash_url)
        {
            MessageHandler();
            Console.WriteLine($"[RBServerCrash] {crash_url}");
            if (crashed_servers.Contains(crash_url))
            {
                return;
            }

            crashed_servers.Add(crash_url);

            List<EventWaitHandle> handles = new List<EventWaitHandle>();
            int i = 0;
            Monitor.Enter(servers);
            foreach (string url in servers.Keys) // Replicate the operation
            {
                Monitor.Exit(servers);
                if (url != sender_url || crashed_servers.Contains(url))
                {
                    handles.Add(new AutoResetEvent(false));
                    Task.Factory.StartNew((state) =>
                    {
                        int j = (int) state;
                        try
                        {
                            ((IServer) Activator.GetObject(typeof(IServer), url)).RBServerCrash(server_url, crash_url);
                            handles[j].Set();
                        }
                        catch (SocketException e)
                        {
                            Console.WriteLine($"[{e.GetType().Name} @ RBServerCrash] Error trying to contact <{url}>");
                            ServerCrash(url);
                        }
                    }, i++);
                }
                Monitor.Enter(servers);
            }
            Monitor.Exit(servers);
            Monitor.Enter(faults_lock);
            for (i = 0; i < max_faults - current_faults; i++) // Wait for the responses
            {
                Monitor.Exit(faults_lock);
                int idx = WaitHandle.WaitAny(handles.ToArray(), 1000);
                Monitor.Enter(faults_lock);
                if (idx == WaitHandle.WaitTimeout && max_faults - current_faults < 1)
                {
                    Console.WriteLine("[RBServerCrash] No more ACKs");
                    break;
                }
                else if (idx == WaitHandle.WaitTimeout)
                {
                    // Delay receiving ACKs
                    i--;
                    continue;
                }
                handles.RemoveAt(idx);
            }
            Monitor.Exit(faults_lock);

            //crashed_servers.Remove(crash_url);
            Monitor.Enter(servers);
            servers.Remove(crash_url);
            Monitor.Exit(servers);
        }
        public void Status()
        {
            Console.WriteLine("[Status]");
            vectorClock.PrettyPrint();

            Console.WriteLine("Clients:");
            foreach (string client in clients.Values)
            {
                Console.WriteLine($"  {client}");
            }
            Console.WriteLine("Leader: " + leader);
            Console.WriteLine("Servers:");
            foreach (string server in servers.Keys)
            {
                Console.WriteLine($"  {server}");
            }
            Console.WriteLine("Crashed:");
            foreach (string server in crashed_servers)
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

        public string GetAlternativeServer()
        {
            MessageHandler();
            Random rand = new Random();
            int i = rand.Next(servers.Count);
            return servers.Keys.ElementAt(i);
        }

        public void Ping() { }
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
