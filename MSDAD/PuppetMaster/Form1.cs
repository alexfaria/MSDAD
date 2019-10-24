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
            if (PCSUrlTextBox.Text == null)
                return;

            TcpChannel channel = new TcpChannel();
            System.Runtime.Remoting.Channels.ChannelServices.RegisterChannel(channel, false);
            IPCS PCSObj = (IPCS)Activator.GetObject(typeof(IPCS), PCSUrlTextBox.Text);

            pcsList.Add(PCSObj);
            listBox1.Items.Add(PCSObj);
        }

        private void CreateServerButton_Click(object sender, EventArgs e)
        {
            IPCS pcs = (IPCS)listBox1.SelectedItem;
            pcs.Server("server_1", "tcp://localhost:8080/Server", 2, 1, 1);
        }
    }
}
