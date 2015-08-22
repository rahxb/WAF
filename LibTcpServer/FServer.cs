using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

using WAF.LibTcpClient;

namespace WAF.LibTcpServer
{

    public class FTcpServer
    {
        TcpListener _listener;
        Dictionary<string, FTcpClient> _clients = new Dictionary<string, FTcpClient>();
        
        public void listen(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();

            Task.Factory.StartNew(() => accept());
            FTcpClient c = new FTcpClient();
            c.Show();

            FTcpClient c2 = new FTcpClient();
            c2.Show();

            
        }

        async void accept()
        {
            while (true)
            {
                System.Threading.Thread.Sleep(1);

                FTcpClient c = new FTcpClient(await _listener.AcceptTcpClientAsync());
                clientInit(c);
                c.StartReceive();
            }
        }

        int _ClientID = 0;
        void clientInit(FTcpClient c)
        {
            c.ReceiveData += c_ReceiveData;

            _ClientID++;
            _clients.Add("C" + _ClientID, c);
        }


        private void c_ReceiveData(object sender, FTcpClient.RecvEventArgs e)
        {
            string name = "";
            foreach (KeyValuePair<string, FTcpClient> c in _clients)
                if (object.ReferenceEquals(c.Value, sender))
                    name = c.Key;
            string str = FTcpClient.DataToString(e.data);
            System.Diagnostics.Debug.WriteLine(string.Format("受信 ({0}) : {1}", name, str));

            ((FTcpClient)sender).SendData(FTcpClient.DataToByteArray("hello"));
        }



    }

}
