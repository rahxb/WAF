using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

namespace WAF
{

    class FServer
    {
        TcpListener _listener;
        Dictionary<string, FClient> _clients = new Dictionary<string, FClient>();
        
        public void listen(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();

            Task.Factory.StartNew(() => accept());
            WinClient c = new WinClient();
            c.Show();

            WinClient c2 = new WinClient();
            c2.Show();

            
        }

        async void accept()
        {
            while (true)
            {
                System.Threading.Thread.Sleep(1);

                FClient c = new FClient(await _listener.AcceptTcpClientAsync());
                clientInit(c);
                c.StartReceive();
            }
        }

        int _ClientID = 0;
        void clientInit(FClient c)
        {
            c.ReceiveData += c_ReceiveData;

            _ClientID++;
            _clients.Add("C" + _ClientID, c);
        }


        private void c_ReceiveData(object sender, FClient.RecvEventArgs e)
        {
            string name = "";
            foreach (KeyValuePair<string, FClient> c in _clients)
                if (object.ReferenceEquals(c.Value, sender))
                    name = c.Key;
            string str = FClient.DataToString(e.data);
            System.Diagnostics.Debug.WriteLine(string.Format("受信 ({0}) : {1}", name, str));

            ((FClient)sender).SendData(FClient.DataToByteArray("hello"));
        }



    }

}
