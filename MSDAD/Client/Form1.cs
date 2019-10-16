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
            ChannelServices.RegisterChannel(channel, true);

            serverObj = (IServer)Activator.GetObject(
                typeof(IServer),
                "tcp://localhost:8086/Server");
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                serverObj.WriteLine(textBox1.Text);
                List<Meeting> meetings = serverObj.getMeetings();
                foreach (Meeting m in meetings)
                {
                    textBox1.Text += m.ToString() + "\r\n";
                }
            }
            catch (SocketException)
            {
                System.Console.WriteLine("Could not locate server");
            }
        }
    }
}
