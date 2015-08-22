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

        /// <summary>
        /// 接続待受けポート番号
        /// </summary>
        int _localport = 1000;

        /// <summary>
        /// 接続待受け
        /// </summary>
        TcpListener _listener;

        /// <summary>
        /// 接続保持用辞書
        /// </summary>
        Dictionary<string, FTcpClient> _connections = new Dictionary<string, FTcpClient>();

        /// <summary>
        /// 接続してきたクライアントのID採番用
        /// </summary>
        int _ConnectionID = 0;


        /// <summary>
        /// FTcpServerコンストラクタ
        /// </summary>
        public FTcpServer()
        {
            // 
            _log.WriteLine("プログラム起動");

        }

        /// <summary>
        /// FTcpServerデストラクタ
        /// </summary>
        ~FTcpServer()
        {
            _log.WriteLine("プログラム終了");
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
        public int LocalPort { get { return _localport; } }

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

                // 接続されたら初期設定を行い、データ受信モードに移行する
                clientInit(connection);
                connection.StartReceive();
            }
        }

        /// <summary>
        /// クライアントが接続してきたあとの初期設定処理
        /// </summary>
        /// <param name="connection"></param>
        void clientInit(FTcpClient connection)
        {
            // データ受信イベントリスナー設定
            connection.ReceiveData += connection_ReceiveData;

            // 接続に名前付け
            _ConnectionID++;
            _connections.Add("C" + _ConnectionID, connection);
        }

        /// <summary>
        /// クライアントからのデータ受信イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connection_ReceiveData(object sender, FTcpClient.RecvEventArgs e)
        {
            string strConnectionName = "<不明>";
            foreach (KeyValuePair<string, FTcpClient> connection in _connections)
                if (object.ReferenceEquals(connection.Value, sender))
                    strConnectionName = connection.Key;

            string strData = FString.DataToString(e.data);
            _log.WriteLine(string.Format("受信 ({0}) : {1}", strConnectionName, strData));

            ((FTcpClient)sender).SendData(FString.DataToByteArray("hello"));
        }



    }

}
