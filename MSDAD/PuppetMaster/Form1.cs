using CommonTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PuppetMaster
{
    public partial class Form1 : Form
    {
        public delegate void CreateServerAsync(string server_id, string URL, int max_faults, int min_delay, int max_delay);
        public delegate void CreateClientAsync(string username, string client_URL, string server_URL, string script_file);
        public delegate void AddRoomAsync(string location, int capacity, string name);
        public delegate string StatusAsync();

        List<IPCS> pcsList;
        public Form1()
        {
            InitializeComponent();
            pcsList = new List<IPCS>();
        }

        private void PCSConnectButton_Click(object sender, EventArgs e)
        {
            if (PCSUrlTextBox.Text.Length == 0)
                return;

            TcpChannel channel = new TcpChannel();
            System.Runtime.Remoting.Channels.ChannelServices.RegisterChannel(channel, false);
            IPCS PCSObj = (IPCS)Activator.GetObject(typeof(IPCS), PCSUrlTextBox.Text);

            pcsList.Add(PCSObj);
            listBox1.Items.Add(PCSObj);
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
        }

        private void CreateClientButton_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null) return;
            else if (clientUsername.Text.Length == 0 || clientURL.Text.Length == 0 ||
                clientServerURL.Text.Length == 0 || clientScript.Text.Length == 0)
                return;

            IPCS pcs = (IPCS)listBox1.SelectedItem;
            outputBox.Text += "Creating client " + clientUsername.Text + "...\r\n";
            createClientBox.Enabled = false;
            try
            {
                CreateClientAsync async = new CreateClientAsync(pcs.Client);
                IAsyncResult asyncRes = async.BeginInvoke(clientUsername.Text, clientURL.Text,
                    clientServerURL.Text, clientScript.Text, null, null);
                asyncRes.AsyncWaitHandle.WaitOne();
            }
            catch (SocketException)
            {
                MessageBox.Show("Could not locate selected PCS",
                    "Socket Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            outputBox.Text += "Client " + clientUsername.Text + " successfully created\r\n";
            clientUsername.Text = "";
            clientURL.Text = "";
            clientServerURL.Text = "";
            clientScript.Text = "";
            createClientBox.Enabled = true;
        }

        private void CreateServerButton_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null) return;
            else if (serverID.Text.Length == 0 || serverURL.Text.Length == 0 ||
                maxFaults.Text.Length == 0 || minDelays.Text.Length == 0 || maxDelays.Text.Length == 0)
                return;

            int faultsMax = 0, delaysMax = 0, delaysMin = 0;
            try
            {
                faultsMax = Int32.Parse(maxFaults.Text);
                delaysMax = Int32.Parse(maxDelays.Text);
                delaysMin = Int32.Parse(minDelays.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show("Max faults, max delays and min delays need to be numeric",
                    "Format Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } 
            catch (OverflowException)
            {
                MessageBox.Show("Max faults, max delays or min delays overflowed",
                    "Overflow Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            IPCS pcs = (IPCS)listBox1.SelectedItem;
            outputBox.Text += "Creating server " + serverID.Text + "...\r\n";
            createServerBox.Enabled = false;
            try
            {
                CreateServerAsync async = new CreateServerAsync(pcs.Server);
                IAsyncResult asyncRes = async.BeginInvoke(serverID.Text, serverURL.Text,
                    faultsMax, delaysMin, delaysMax, null, null);
                asyncRes.AsyncWaitHandle.WaitOne();
            }
            catch (SocketException)
            {
                MessageBox.Show("Could not locate selected PCS",
                    "Socket Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            outputBox.Text += "Server " + serverID.Text + " successfully created\r\n";
            serverID.Text = "";
            serverURL.Text = "";
            maxFaults.Text = "";
            minDelays.Text = "";
            maxDelays.Text = "";
            createServerBox.Enabled = true;
        }

        private void StatusButton_Click(object sender, EventArgs e)
        {

        }

        private void AddRoomButton_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null) return;
            else if (roomLocation.Text.Length == 0 || roomCapacity.Text.Length == 0 ||
                roomName.Text.Length == 0) return;

            int roomC = 0;
            try
            {
                roomC = Int32.Parse(roomCapacity.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show("Room capacity needs to be numeric", "Format Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (OverflowException)
            {
                MessageBox.Show("Room capacity overflowed", "Overflow Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            IPCS pcs = (IPCS)listBox1.SelectedItem;
            outputBox.Text += "Adding room " + roomName.Text + " to " + roomLocation.Text + "...\r\n";
            addRoomBox.Enabled = false;
            try
            {
                AddRoomAsync async = new AddRoomAsync(pcs.AddRoom);
                IAsyncResult asyncRes = async.BeginInvoke(roomLocation.Text, roomC, roomName.Text, null, null);
                asyncRes.AsyncWaitHandle.WaitOne();
            }
            catch (SocketException)
            {
                MessageBox.Show("Could not locate selected PCS",
                    "Socket Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            outputBox.Text += "Successfully added room " + roomName.Text + " to " + roomLocation.Text + "\r\n";
            roomLocation.Text = "";
            roomCapacity.Text = "";
            roomName.Text = "";
            addRoomBox.Enabled = true;
        }
    }
}
