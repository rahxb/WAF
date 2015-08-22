using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace WAF.AppWindowClient
{
    public partial class WinClient : Form
    {
        TcpClient cl;

        public WinClient()
        {
            InitializeComponent();

            button1.Enabled = true;
            button2.Enabled = false;
        }
        
        
        private void button1_Click(object sender, EventArgs e)
        {
            cl = new TcpClient();
            cl.Connect("localhost", 2000);
            FTcpClient c = new FTcpClient(cl);
            c.ReceiveData += c_ReceiveData;
            c.StartReceive();

            button1.Enabled = false;
            button2.Enabled = true;
        }

        private void c_ReceiveData(object sender, FTcpClient.RecvEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Client : " + FTcpClient.DataToString(e.data));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;

            byte[] bin = FTcpClient.DataToByteArray(textBox1.Text);
            cl.GetStream().Write(bin, 0, bin.Length);

            button2.Enabled = true;
        }



    }
}
