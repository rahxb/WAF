using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

using WAF.AppWindowClient;
using WAF.LibCommon;

namespace WAF.AppConsoleServer
{

    public class FTcpServer
    {
        Log _log = new Log("Server");

        int _localport = 1000;
        TcpListener _listener;
        Dictionary<string, FTcpClient> _clients = new Dictionary<string, FTcpClient>();
    
        public FTcpServer()
        {
            // 

        }

        public void listen(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();

            // 接続受け待ちの開始
            IsListen = false;
            Task.Factory.StartNew(() => accept());

        }

        public int LocalPort { get { return _localport; } }

        public bool IsListen { get; set; } = false;

        async void accept()
        {
            while (true)
            {

                // 無限ループのためCPU負荷軽減用Sleep
                System.Threading.Thread.Sleep(1);

                // acceptが呼び出されているためtrueにする
                if (IsListen == false)
                    IsListen = true;

                // 接続されるまで待機する
                FTcpClient c = new FTcpClient(await _listener.AcceptTcpClientAsync());

                // 接続されたら初期設定を行い、データ受信モードに移行する
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
            string str = FString.DataToString(e.data);
            _log.WriteLine("", string.Format("受信 ({0}) : {1}", name, str));

            ((FTcpClient)sender).SendData(FString.DataToByteArray("hello"));
        }



    }

}
