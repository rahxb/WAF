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
        Log _log = new Log("TcpServer");
        

        /// <summary>
        /// 接続待受け
        /// </summary>
        TcpListener _listener;


        #region イベント発生用

        public class ConnectionRequestEventArgs
        {
            public FTcpClient Connection { get; set; }
            public bool Cancel { get; set; }
        }

        public event EventHandler<ConnectionRequestEventArgs> ConnectionRequest = null; 

        ConnectionRequestEventArgs RaiseEventConnectionRequest(FTcpClient connection)
        {
            ConnectionRequestEventArgs e = new ConnectionRequestEventArgs();

            if (ConnectionRequest != null)
            {
                e.Connection = connection;
                e.Cancel = false;
                ConnectionRequest(this, e);
            }
            return e;
        }

        #endregion


        /// <summary>
        /// FTcpServerコンストラクタ
        /// </summary>
        public FTcpServer()
        {
            // 
            _log.WriteLine("Beging TcpServer");

        }

        /// <summary>
        /// FTcpServerデストラクタ
        /// </summary>
        ~FTcpServer()
        {
            _log.WriteLine("End TcpServer");
        }

        /// <summary>
        /// 接続待受けを開始する
        /// </summary>
        /// <param name="port"></param>
        public void Listen(int port)
        {
            // 接続待受け開始
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();
            IsListen = false;
            Task.Factory.StartNew(() => accept());
        }


        /// <summary>
        /// TCPのローカルポートを返す
        /// </summary>
        public int LocalPort { get; } = 1000;

        /// <summary>
        /// 接続待受け状態を返す
        /// </summary>
        public bool IsListen { get; set; } = false;

        /// <summary>
        /// 接続待受け処理と
        /// </summary>
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
                FTcpClient connection = new FTcpClient(await _listener.AcceptTcpClientAsync());

                // 接続があったことをイベント先に知らせる
                ConnectionRequestEventArgs cre = RaiseEventConnectionRequest(connection);


                // 接続されたら初期設定を行い、データ受信モードに移行する
                if (cre.Cancel)
                    cre.Connection.Close();
                else
                    connection.StartReceive();

            }
        }


    }

}
