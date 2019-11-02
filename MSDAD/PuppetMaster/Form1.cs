using CommonTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PuppetMaster
{
    public partial class Form1 : Form
    {
        public delegate void CreateServerAsync(string server_id, string URL, int max_faults, int min_delay, int max_delay);
        public delegate void CreateClientAsync(string username, string client_URL, string server_URL, string script_file);
        public delegate void AddRoomAsync(string location, int capacity, string name);
        public delegate void ServerCommandAsync(string server_id);
        public delegate string StatusAsync();
        public delegate string[] GetServersAsync();

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
                async.EndInvoke(asyncRes);
                outputBox.Text += "Client " + clientUsername.Text + " successfully created\r\n";
            }
            catch (SocketException)
            {
                MessageBox.Show("Could not locate selected PCS",
                    "Socket Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                async.EndInvoke(asyncRes);
                outputBox.Text += "Server " + serverID.Text + " successfully created\r\n";
            }
            catch (SocketException)
            {
                MessageBox.Show("Could not locate selected PCS",
                    "Socket Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                async.EndInvoke(asyncRes);
                outputBox.Text += "Successfully added room " + roomName.Text + " to " + roomLocation.Text + "\r\n";
            }
            catch (SocketException)
            {
                MessageBox.Show("Could not locate selected PCS",
                    "Socket Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            roomLocation.Text = "";
            roomCapacity.Text = "";
            roomName.Text = "";
            addRoomBox.Enabled = true;
        }

        private void ShowServers_Click(object sender, EventArgs e)
        {
            IPCS pcs = (IPCS)listBox1.SelectedItem;
            outputBox.Text += "Getting servers from selected PCS...\r\n";
            try
            {
                GetServersAsync async = new GetServersAsync(pcs.GetServers);
                IAsyncResult asyncRes = async.BeginInvoke(null, null);
                asyncRes.AsyncWaitHandle.WaitOne();
                string[] servers = async.EndInvoke(asyncRes);
                foreach (string server in servers)
                {
                    serverList.Items.Add(server);
                }
            }
            catch (SocketException)
            {
                MessageBox.Show("Could not locate selected PCS",
                    "Socket Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CrashButton_Click(object sender, EventArgs e)
        {
            if (serverList.SelectedItem == null) return;
            string server_id = (string)serverList.SelectedItem;
            IPCS pcs = (IPCS)listBox1.SelectedItem;
            try
            {
                ServerCommandAsync async = new ServerCommandAsync(pcs.Crash);
                IAsyncResult asyncRes = async.BeginInvoke(server_id, null, null);
                asyncRes.AsyncWaitHandle.WaitOne();
                async.EndInvoke(asyncRes);
                outputBox.Text += "Successfully crashed " + server_id + "\r\n";
            }
            catch (SocketException)
            {
                MessageBox.Show("Could not locate selected PCS",
                    "Socket Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FreezeButton_Click(object sender, EventArgs e)
        {
            if (serverList.SelectedItem == null) return;
            string server_id = (string)serverList.SelectedItem;
            IPCS pcs = (IPCS)listBox1.SelectedItem;
            try
            {
                ServerCommandAsync async = new ServerCommandAsync(pcs.Freeze);
                IAsyncResult asyncRes = async.BeginInvoke(server_id, null, null);
                asyncRes.AsyncWaitHandle.WaitOne();
                async.EndInvoke(asyncRes);
                outputBox.Text += "Successfully frozen " + server_id + "\r\n";
            }
            catch (SocketException)
            {
                MessageBox.Show("Could not locate selected PCS",
                    "Socket Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UnfreezeButton_Click(object sender, EventArgs e)
        {
            if (serverList.SelectedItem == null) return;
            string server_id = (string)serverList.SelectedItem;
            IPCS pcs = (IPCS)listBox1.SelectedItem;
            try
            {
                ServerCommandAsync async = new ServerCommandAsync(pcs.Unfreeze);
                IAsyncResult asyncRes = async.BeginInvoke(server_id, null, null);
                asyncRes.AsyncWaitHandle.WaitOne();
                async.EndInvoke(asyncRes);
                outputBox.Text += "Successfully unfrozen " + server_id + "\r\n";
            }
            catch (SocketException)
            {
                MessageBox.Show("Could not locate selected PCS",
                    "Socket Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RunScriptButton_Click(object sender, EventArgs e)
        {
            if (scriptBox.TextLength == 0) return;

            outputBox.Text += "Reading " + scriptBox.Text + "...\r\n";
            try
            {
                IPCS pcs = null;
                string[] fileLines = File.ReadAllLines(scriptBox.Text);
                foreach (string line in fileLines)
                {
                    string[] commandLine = line.Split(' ');
                    if (commandLine.Length <= 0) continue;

                    outputBox.Text += "Running command " + commandLine[0] + "\r\n";
                    switch (commandLine[0])
                    {
                        case "PCS":
                            if (commandLine.Length == 2)
                                pcs = (IPCS)Activator.GetObject(typeof(IPCS), commandLine[1]);
                            else
                            {
                                outputBox.Text += "ERROR - PCS usage: PCS <pcs_url>\r\n";
                                return;
                            }
                            break;
                        case "Server":
                            if (commandLine.Length == 6)
                            {
                                if (pcs != null)
                                    pcs.Server(commandLine[1], commandLine[2], Int32.Parse(commandLine[3]),
                                        Int32.Parse(commandLine[4]), Int32.Parse(commandLine[5]));
                                else
                                {
                                    outputBox.Text += "ERROR - must be connected to PCS\r\n";
                                    return;
                                }
                            }
                            else
                            {
                                outputBox.Text +=
                                    "ERROR - Server usage: Server <server_id> <URL> <max_faults> <min_delay> <max_delay>\r\n";
                                return;
                            }
                            break;
                        case "Client":
                            if (commandLine.Length == 5)
                            {
                                if (pcs != null)
                                    pcs.Client(commandLine[1], commandLine[2], commandLine[3], commandLine[4]);
                                else
                                {
                                    outputBox.Text += "ERROR - must be connected to PCS\r\n";
                                    return;
                                }
                            }
                            else
                            {
                                outputBox.Text +=
                                    "ERROR - Client usage: Client <username> <client_URL> <server_URL> <script_file>\r\n";
                                return;
                            }
                            break;
                        case "AddRoom":
                            if (commandLine.Length == 4)
                            {
                                if (pcs != null)
                                    pcs.AddRoom(commandLine[1], Int32.Parse(commandLine[2]), commandLine[3]);
                                else
                                {
                                    outputBox.Text += "ERROR - must be connected to PCS\r\n";
                                    return;
                                }
                            }
                            else
                            {
                                outputBox.Text +=
                                    "ERROR - AddRoom usage: AddRoom <location> <capacity> <room_name>\r\n";
                                return;
                            }
                            break;
                        case "Status":
                            if (commandLine.Length == 1)
                            {
                                if (pcs != null)
                                    outputBox.Text += pcs.Status() + "\r\n";
                                else
                                {
                                    outputBox.Text += "ERROR - must be connected to PCS\r\n";
                                    return;
                                }
                            }
                            else
                            {
                                outputBox.Text +=
                                    "ERROR - Status usage: Status\r\n";
                                return;
                            }
                            break;
                        case "Crash":
                            if (commandLine.Length == 2)
                            {
                                if (pcs != null)
                                    pcs.Crash(commandLine[1]);
                                else
                                {
                                    outputBox.Text += "ERROR - must be connected to PCS\r\n";
                                    return;
                                }
                            }
                            else
                            {
                                outputBox.Text +=
                                    "ERROR - Crash usage: Crash <server_id>\r\n";
                                return;
                            }
                            break;
                        case "Freeze":
                            if (commandLine.Length == 2)
                            {
                                if (pcs != null)
                                    pcs.Freeze(commandLine[1]);
                                else
                                {
                                    outputBox.Text += "ERROR - must be connected to PCS\r\n";
                                    return;
                                }
                            }
                            else
                            {
                                outputBox.Text +=
                                    "ERROR - Freeze usage: Freeze <server_id>\r\n";
                                return;
                            }
                            break;
                        case "Unfreeze":
                            if (commandLine.Length == 2)
                            {
                                if (pcs != null)
                                    pcs.Unfreeze(commandLine[1]);
                                else
                                {
                                    outputBox.Text += "ERROR - must be connected to PCS\r\n";
                                    return;
                                }
                            }
                            else
                            {
                                outputBox.Text +=
                                    "ERROR - Unfreeze usage: Unfreeze <server_id>\r\n";
                                return;
                            }
                            break;
                        case "Wait":
                            if (commandLine.Length == 2)
                            {
                                Thread.Sleep(Int32.Parse(commandLine[1]));
                            }
                            else
                            {
                                outputBox.Text +=
                                    "ERROR - Wait usage: Wait <ms>\r\n";
                                return;
                            }
                            break;
                        default:
                            outputBox.Text += "Command not recognized\r\n";
                            break;
                    }
                }
            }
            catch (IOException)
            {
                outputBox.Text += "ERROR - Could not read file " + scriptBox.Text + "\r\n";
            }
            catch (FormatException)
            {
                outputBox.Text += "ERROR - Could not parse value\r\n";
            }
            catch (SocketException)
            {
                outputBox.Text += "ERROR - Could not connecto to PCS\r\n";
            }
        }
    }
}
