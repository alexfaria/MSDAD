using CommonTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PuppetMaster
{
    public partial class Form1 : Form
    {
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
            pcs.Client(clientUsername.Text, clientURL.Text, clientServerURL.Text, clientScript.Text);
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
                MessageBox.Show("Max faults, max delays and min delays need to be numeric", "Format Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } 
            catch (OverflowException)
            {
                MessageBox.Show("Max faults, max delays or min delays overflowed", "Overflow Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            IPCS pcs = (IPCS)listBox1.SelectedItem;
            pcs.Server(serverID.Text, serverURL.Text, faultsMax, delaysMin, delaysMax);
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
            pcs.AddRoom(roomLocation.Text, roomC, roomName.Text);
        }
    }
}
