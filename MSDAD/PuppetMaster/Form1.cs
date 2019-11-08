using CommonTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text.RegularExpressions;
using System.Threading;
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

        Dictionary<string, string> servers;
        Dictionary<string, string> clients;
        BindingList<string> pcsUrls;

        public Form1()
        {
            InitializeComponent();
            pcsUrls = new BindingList<string>();
            servers = new Dictionary<string, string>();
            clients = new Dictionary<string, string>();
            TcpChannel channel = new TcpChannel();
            // bind pcsUrls to pcsListBox
            pcsListBox.DataSource = pcsUrls;
            System.Runtime.Remoting.Channels.ChannelServices.RegisterChannel(channel, false);
        }

        private void PCSConnectButton_Click(object sender, EventArgs e)
        {
            // check if pcs url already exists
            foreach (string url in pcsUrls)
            {
                if (url == PCSUrlTextBox.Text)
                {
                    return;
                }
            }

            // check if pcs url is valid
            Regex regex = new Regex(@"tcp://.*/\w");
            Match match = regex.Match(PCSUrlTextBox.Text);
            if (!match.Success)
            {
                return;
            }

            pcsUrls.Add(PCSUrlTextBox.Text);
            pcsListBox.SelectedIndex = pcsListBox.Items.Count - 1;
        }

        private void CreateClientButton_Click(object sender, EventArgs e)
        {
            if (pcsListBox.SelectedItem == null)
            {
                return;
            }
            else if (clientUsername.Text.Length == 0 || clientURL.Text.Length == 0 ||
                     clientServerURL.Text.Length == 0 || clientScript.Text.Length == 0)
            {
                return;
            }

            string pcsUrl = (string) pcsListBox.SelectedItem;
            IPCS pcs = (IPCS) Activator.GetObject(typeof(IPCS), pcsUrl);
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
                clients.Add(clientUsername.Text, clientURL.Text);
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
            if (pcsListBox.SelectedItem == null)
            {
                return;
            }
            else if (serverID.Text.Length == 0 || serverURL.Text.Length == 0 ||
                     maxFaults.Text.Length == 0 || minDelays.Text.Length == 0 || maxDelays.Text.Length == 0)
            {
                return;
            }

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

            string pcsUrl = (string) pcsListBox.SelectedItem;
            IPCS pcs = (IPCS) Activator.GetObject(typeof(IPCS), pcsUrl);
            outputBox.Text += "Creating server " + serverID.Text + "...\r\n";
            createServerBox.Enabled = false;
            try
            {
                CreateServerAsync async = new CreateServerAsync(pcs.Server);
                IAsyncResult asyncRes = async.BeginInvoke(serverID.Text, serverURL.Text,
                    faultsMax, delaysMin, delaysMax, null, null);
                asyncRes.AsyncWaitHandle.WaitOne();
                async.EndInvoke(asyncRes);
                servers.Add(serverID.Text, serverURL.Text);
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
            int i = 0;
            EventWaitHandle[] handles = new EventWaitHandle[servers.Count + clients.Count];
            foreach (string server_url in servers.Values)
            {
                EventWaitHandle handle = new EventWaitHandle(false, EventResetMode.ManualReset);
                Thread task = new Thread(() =>
                {
                    try
                    {
                        ((IServer) Activator.GetObject(typeof(IServer), server_url)).Status();
                    }
                    catch (SocketException) { } //TODO: imprimir qqr cena ou atualizar lista de servidores
                    handle.Set();
                });
                handles[i++] = handle;
                task.Start();
            }

            foreach (string client_url in clients.Values)
            {
                EventWaitHandle handle = new EventWaitHandle(false, EventResetMode.ManualReset);
                Thread task = new Thread(() =>
                {
                    try
                    {
                        ((IClient) Activator.GetObject(typeof(IClient), client_url)).Status();
                    }
                    catch (SocketException) { } //TODO: imprimir qqr cena ou atualizar lista de servidores
                    handle.Set();
                });
                handles[i++] = handle;
                task.Start();
            }

            // WaitHandle.WaitAll(handles);
        }

        private void AddRoomButton_Click(object sender, EventArgs e)
        {
            if (roomLocation.Text.Length == 0 || this.roomCapacity.Text.Length == 0 || roomName.Text.Length == 0)
            {
                return;
            }

            int roomCapacity = 0;
            try
            {
                roomCapacity = int.Parse(this.roomCapacity.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show("Room capacity needs to be numeric", "Format Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (OverflowException)
            {
                MessageBox.Show("Room capacity overflowed", "Overflow Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            foreach (var server in servers)
            {
                // server.Key -> id
                // server.Value -> url

                IServer serverObj = (IServer) Activator.GetObject(typeof(IServer), server.Value);

                try
                {
                    AddRoomAsync async = new AddRoomAsync(serverObj.AddRoom);
                    IAsyncResult asyncRes = async.BeginInvoke(roomLocation.Text, roomCapacity, roomName.Text, null, null);
                    asyncRes.AsyncWaitHandle.WaitOne();
                    async.EndInvoke(asyncRes);
                    outputBox.Text += $"Added room <{roomLocation.Text},{roomName.Text}> to server <{server.Key},{server.Value}> \r\n";
                }
                catch (SocketException)
                {
                    MessageBox.Show($"Could not locate <{server.Key},{server.Value}>",
                        "Socket Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            roomLocation.Text = "";
            this.roomCapacity.Text = "";
            roomName.Text = "";
            addRoomBox.Enabled = true;
        }

        private void ShowServers_Click(object sender, EventArgs e)
        {
            if (pcsListBox.SelectedItem == null)
                return;
            string pcsUrl = (string) pcsListBox.SelectedItem;
            IPCS pcs = (IPCS) Activator.GetObject(typeof(IPCS), pcsUrl);
            outputBox.Text += "Getting servers from selected PCS...\r\n";
            try
            {
                GetServersAsync async = new GetServersAsync(pcs.GetServers);
                IAsyncResult asyncRes = async.BeginInvoke(null, null);
                asyncRes.AsyncWaitHandle.WaitOne();
                BindingList<string> serversDataSource = new BindingList<string>(async.EndInvoke(asyncRes));
                serverList.DataSource = serversDataSource;
            }
            catch (SocketException)
            {
                MessageBox.Show("Could not locate selected PCS",
                    "Socket Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CrashButton_Click(object sender, EventArgs e)
        {
            if (pcsListBox.SelectedItem == null ||
                serverList.SelectedItem == null)
                return;
            string server_id = (string) serverList.SelectedItem;
            string pcsUrl = (string) pcsListBox.SelectedItem;
            IPCS pcs = (IPCS) Activator.GetObject(typeof(IPCS), pcsUrl);
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
            if (pcsListBox.SelectedItem == null ||
                serverList.SelectedItem == null)
                return;
            string server_id = (string) serverList.SelectedItem;
            string pcsUrl = (string) pcsListBox.SelectedItem;
            IPCS pcs = (IPCS) Activator.GetObject(typeof(IPCS), pcsUrl);
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
            if (pcsListBox.SelectedItem == null ||
                serverList.SelectedItem == null)
                return;
            string server_id = (string) serverList.SelectedItem;
            string pcsUrl = (string) pcsListBox.SelectedItem;
            IPCS pcs = (IPCS) Activator.GetObject(typeof(IPCS), pcsUrl);
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
            if (scriptBox.TextLength == 0)
                return;

            outputBox.Text += "Reading " + scriptBox.Text + "...\r\n";
            try
            {
                IPCS pcs = null;
                string[] fileLines = File.ReadAllLines(scriptBox.Text);
                foreach (string line in fileLines)
                {
                    string[] commandLine = line.Split(' ');
                    if (commandLine.Length <= 0)
                        continue;

                    outputBox.Text += "Running command " + commandLine[0] + "\r\n";
                    switch (commandLine[0])
                    {
                        case "PCS":
                            if (commandLine.Length == 2)
                                pcs = (IPCS) Activator.GetObject(typeof(IPCS), commandLine[1]);
                            else
                            {
                                outputBox.Text += "ERROR - PCS usage: PCS <pcs_url>\r\n";
                                return;
                            }
                            break;
                        case "Server":
                            if (commandLine.Length == 6)
                            {
                                // find the pcs with a matching hostname
                                pcs = GetPCSFromHostname(commandLine[2]);
                                try
                                {
                                    pcs.Server(commandLine[1], commandLine[2], Int32.Parse(commandLine[3]),
                                        Int32.Parse(commandLine[4]), Int32.Parse(commandLine[5]));
                                    servers.Add(commandLine[1], commandLine[2]);
                                }
                                catch (Exception)
                                {
                                    outputBox.Text += "ERROR - must be connected to PCS\r\n";
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
                                // find the pcs with a matching hostname
                                pcs = GetPCSFromHostname(commandLine[2]);
                                try
                                {
                                    pcs.Client(commandLine[1], commandLine[2], commandLine[3], commandLine[4]);
                                    clients.Add(commandLine[1], commandLine[2]);
                                }
                                catch (Exception)
                                {
                                    outputBox.Text += "ERROR - must be connected to PCS\r\n";
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
                                foreach (var server in servers)
                                {
                                    // server.Key -> id
                                    // server.Value -> url

                                    IServer serverObj = (IServer) Activator.GetObject(typeof(IServer), server.Value);

                                    try
                                    {
                                        AddRoomAsync async = new AddRoomAsync(serverObj.AddRoom);
                                        IAsyncResult asyncRes = async.BeginInvoke(commandLine[1], Int32.Parse(commandLine[2]), commandLine[3], null, null);
                                        asyncRes.AsyncWaitHandle.WaitOne();
                                        async.EndInvoke(asyncRes);
                                        outputBox.Text += $"Added room <{roomLocation.Text},{roomName.Text}> to server <{server.Key},{server.Value}> \r\n";
                                    }
                                    catch (SocketException)
                                    {
                                        outputBox.Text += $"[Socket Exception] Could not locate <{ server.Key},{ server.Value}> \r\n";
                                    }
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
                            int i = 0;
                            EventWaitHandle[] handles = new EventWaitHandle[servers.Count + clients.Count];
                            foreach (string server_url in servers.Values)
                            {
                                Thread task = new Thread(() =>
                                {
                                    ((IServer) Activator.GetObject(typeof(IServer), server_url)).Status();
                                    handles[i++].Set();
                                });
                            }

                            foreach (string client_url in clients.Values)
                            {
                                Thread task = new Thread(() =>
                                {
                                    ((IServer) Activator.GetObject(typeof(IServer), client_url)).Status();
                                    handles[i++].Set();
                                });
                            }

                            //WaitHandle.WaitAll(handles);
                            break;
                        case "Crash":
                            if (commandLine.Length == 2)
                            {
                                if (servers.TryGetValue(commandLine[1], out string server_url))
                                {
                                    try
                                    {
                                        ((IServer) Activator.GetObject(typeof(IServer), server_url)).Crash();
                                    }
                                    catch (SocketException)
                                    {
                                        outputBox.Text += $"[Socket Exception] Could not locate <{commandLine[1]},{server_url}>\r\n";
                                    }
                                }
                                else
                                {
                                    outputBox.Text += "ERROR - server does not exist\r\n";
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
                                if (servers.TryGetValue(commandLine[1], out string server_url))
                                {
                                    try
                                    {
                                        ((IServer) Activator.GetObject(typeof(IServer), server_url)).Freeze();
                                    }
                                    catch (SocketException)
                                    {
                                        outputBox.Text += $"[Socket Exception] Could not locate <{commandLine[1]},{server_url}>\r\n";
                                    }
                                }
                                else
                                {
                                    outputBox.Text += "ERROR - server does not exist\r\n";
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
                                if (servers.TryGetValue(commandLine[1], out string server_url))
                                {
                                    try
                                    {
                                        ((IServer) Activator.GetObject(typeof(IServer), server_url)).Unfreeze();
                                    }
                                    catch (SocketException)
                                    {
                                        outputBox.Text += $"[Socket Exception] Could not locate <{commandLine[1]},{server_url}>\r\n";
                                    }
                                }
                                else
                                {
                                    outputBox.Text += "ERROR - server does not exist\r\n";
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
                                outputBox.Text += "ERROR - Wait usage: Wait <ms>\r\n";
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
            catch (FormatException formatException)
            {
                outputBox.Text += $"ERROR - Could not parse value: ${formatException}\r\n";
            }
            catch (SocketException)
            {
                outputBox.Text += "ERROR - Could not connecto to PCS\r\n";
            }
        }

        private IPCS GetPCSFromHostname(string url)
        {
            IPCS pcs = null;
            foreach (string pcs_url in pcsListBox.Items)
            {
                Uri pcsUri = new Uri(pcs_url);
                Uri serverUri = new Uri(url);
                if (Uri.Compare(pcsUri, serverUri, UriComponents.Host, UriFormat.Unescaped, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    pcs = (IPCS) Activator.GetObject(typeof(IPCS), pcs_url);
                }
            }

            return pcs;
        }
    }
}
