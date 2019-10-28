using CommonTypes;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1 : Form
    {
        IServer serverObj;
        public Form1()
        {
            InitializeComponent();

            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, false);

            serverObj = (IServer)Activator.GetObject(
                typeof(IServer),
                "tcp://localhost:8086/Server");
        }

        private void showBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // serverObj.WriteLine(textBox1.Text);
                List<Meeting> meetings = serverObj.GetMeetings();
                meetingsBox.Nodes.Clear();
                foreach (Meeting m in meetings)
                {
                    TreeNode parent = meetingsBox.Nodes.Add(m.ToString());
                    parent.Tag = m;
                    foreach (Slot s in m.slots)
                    {
                        TreeNode child = parent.Nodes.Add(s.ToString());
                        child.Tag = s;
                        foreach (string user in s.participants)
                        {
                            child.Nodes.Add(user);
                        }
                    }
                }
            }
            catch (SocketException)
            {
                System.Console.WriteLine("Could not locate server");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void createBtn_Click(object sender, EventArgs e)
        {
            List<string> invitees = new List<string>();
            if (numSlotsBox.Text.Length > 0 && slotsBox.Text.Length > 0)
            {
                invitees = new List<string>(Int32.Parse(numSlotsBox.Text));
                foreach (string i in slotsBox.Text.Split(new char[] { '\r', '\n' }))
                    invitees.Add(i);
            }
            List<Slot> slots = new List<Slot>(Int32.Parse(numSlotsBox.Text));
            foreach (string s in slotsBox.Text.Split(new char[] { '\r', '\n' }))
            {
                if (s.Length == 0) continue;
                string[] info = s.Split(',');
                string[] date = info[1].Split('-');
                Slot slot = new Slot(
                    new DateTime(
                        Int32.Parse(date[0]),
                        Int32.Parse(date[1]),
                        Int32.Parse(date[2])),
                    info[0]);
                slots.Add(slot);
            }
            Meeting m = new Meeting(
                userBox.Text,
                topicBox.Text,
                Int32.Parse(participantsBox.Text),
                invitees,
                slots
            );
            serverObj.CreateMeeting(m);
        }

        private void meetingsBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void joinBtn_Click_1(object sender, EventArgs e)
        {
            TreeNode node = meetingsBox.SelectedNode;
            if (node != null)
            {
                try
                {
                    if (node.Parent == null) return;
                    Meeting m = (Meeting)node.Parent.Tag;
                    Slot s = (Slot)node.Tag;
                    serverObj.JoinMeeting(userBox.Text, m.topic, s);
                    MessageBox.Show($"Joined meeting {m.topic} in slot {s.ToString()}");
                }
                catch (InvalidCastException ie)
                {
                    MessageBox.Show(ie.Message);
                }
            }
        }

        private void meetingsBox_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }
    }
}
