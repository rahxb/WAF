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

using WAF.LibCommon;

namespace WAF.AppWindowClient
{
    public partial class WinClient : Form
    {
        Log _log = new Log("Client");

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
            cl.Connect("localhost", 1000);
            FTcpClient c = new FTcpClient(cl);
            c.ReceiveData += c_ReceiveData;
            c.StartReceive();

            button1.Enabled = false;
            button2.Enabled = true;
        }

        private void c_ReceiveData(object sender, FTcpClient.RecvEventArgs e)
        {
            _log.WriteLine("Client - ReceivedData", FString.DataToString(e.data));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;

            byte[] bin = FString.DataToByteArray(textBox1.Text);
            cl.GetStream().Write(bin, 0, bin.Length);

            button2.Enabled = true;
        }



    }
}
