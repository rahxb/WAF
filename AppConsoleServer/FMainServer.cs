using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WAF.AppWindowClient;
using WAF.LibCommon;

namespace WAF.AppConsoleServer
{
    public class FMainServer
    {
        Log _log = new Log("MainServer");

        // 
        // 送信コマンドの例
        // 
        //             タブ　　Base64エンコード文字列　改行
        //               ↓　　　　↓　　　　　　　　　　↓
        // PUBLIC-MESSAGE  MESSAGE:
        // 
        FTcpServer _tcpserver = new FTcpServer();


        /// <summary>
        /// 接続保持用辞書
        /// </summary>
        Dictionary<string, FTcpClient> _connections = new Dictionary<string, FTcpClient>();

        /// <summary>
        /// 接続してきたクライアントのID採番用
        /// </summary>
        int _ConnectionID = 0;


        #region Classes




        /// <summary>
        /// データ送信のデータ格納クラス
        /// </summary>
        class SendDataPackage
        {
            public List<TargetNameAndCommandParams> Items = new List<TargetNameAndCommandParams>();
        }

        /// <summary>
        /// 特定の相手にデータ送信するときのコマンドとパラメータ格納クラス
        /// </summary>
        class TargetNameAndCommandParams
        {
            public TargetNameAndCommandParams(string strToName, string strCommandParams)
            {
                this.ToName = strToName;
                this.CommandParams = strCommandParams;
            }
            public string ToName { get; set; }
            public string CommandParams { get; set; }
        }

        #endregion





        /// <summary>
        /// FMainServerのコンストラクタ
        /// </summary>
        public FMainServer()
        {
            _tcpserver = new FTcpServer();
            _tcpserver.ConnectionRequest += _tcpserver_ConnectionRequest;
        }

        public void Start()
        {
            _tcpserver.Listen(1000);
        }

        /// <summary>
        /// クライアントから接続があったら初期化設定を行う
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _tcpserver_ConnectionRequest(object sender, FTcpServer.ConnectionRequestEventArgs e)
        {
            // 初期設定を行う
            clientInit(e.Connection);

            // 接続を拒否しない
            e.Cancel = false;
        }




        public bool IsListen
        {
            get { return _tcpserver.IsListen; }
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
            string strConnectionName = GetConnectionFromFTcpClient((FTcpClient)sender);
            if (strConnectionName == null)
                strConnectionName = "<不明>";

            // ログ出力
            string strReceiveData = FString.DataToString(e.data);
            _log.WriteLine(string.Format("受信 ({0}) : {1}", strConnectionName, strReceiveData));

            // 受信したデータを元にサーバーで各コマンド処理を行う
            SendDataPackage sdp = ServerProcess(strConnectionName, strReceiveData);

            // クライアントに返信が必要な場合はデータを送信する
            SendTo(sdp);

        }

        #region Connectionと接続名の変換

        /// <summary>
        /// FTcpClientから接続名を返す
        /// </summary>
        /// <param name="CurrentConnection"></param>
        /// <returns></returns>
        string GetConnectionFromFTcpClient(FTcpClient CurrentConnection)
        {
            string result = null;
            foreach (KeyValuePair<string, FTcpClient> connection in _connections)
            {
                if (object.ReferenceEquals(connection.Value, CurrentConnection))
                {
                    result = connection.Key;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// 接続名からFTcpClientを返す
        /// </summary>
        /// <param name="strConnectionName"></param>
        /// <returns></returns>
        FTcpClient GetConnectionFromName(string strConnectionName)
        {
            FTcpClient result = null;
            foreach (KeyValuePair<string, FTcpClient> connection in _connections)
            {
                if (connection.Key == strConnectionName)
                {
                    result = connection.Value;
                    break;
                }
            }
            return result;
        }

        #endregion

        
        #region コマンドの基幹処理

        /// <summary>
        /// 受信したデータを元にサーバーで各コマンド処理を行う
        /// </summary>
        /// <param name="strReceiveData"></param>
        /// <returns></returns>
        SendDataPackage ServerProcess(string strConnectionName, string strReceiveData)
        {
            // コマンドとパラメータを分割する
            FProtocolFormat.CommandAndParams cap = FProtocolFormat.GetCommandParams(strReceiveData);

            // コマンドの送り元を付加する
            cap.Params.Add("FROM-NAME", strConnectionName);

            // サーバーのメイン処理を行う
            SendDataPackage sdp = ServerProcessSwitch(cap);

            return sdp;
        }


        /// <summary>
        /// 特定の相手にデータを送信する
        /// </summary>
        /// <param name="sdp"></param>
        void SendTo(SendDataPackage sdp)
        {
            foreach (TargetNameAndCommandParams item in sdp.Items)
            {
                FTcpClient connection = GetConnectionFromName(item.ToName);
                if (connection != null)
                    connection.SendDataNewLine(item.CommandParams);
            }
        }

        #endregion


        #region コマンドの分岐(中間)処理

        /// <summary>
        /// コマンド処理分岐
        /// </summary>
        /// <param name="cap"></param>
        /// <returns></returns>
        SendDataPackage ServerProcessSwitch(FProtocolFormat.CommandAndParams cap)
        {
            SendDataPackage result = null;

            switch (cap.CommandName)
            {
                // ここでのコマンド名は必ず大文字
                case "PUBLIC-MESSAGE":
                    result = SPPublicMessage(cap.Params);
                    break;

                case "PRIVATE-MESSAGE":
                    result = SPPrivateMessage(cap.Params);
                    break;

            }
            return result;
        }

        #endregion


        #region 各コマンドの末端処理



        /// <summary>
         /// 全員にメッセージを送信する準備
         /// </summary>
         /// <param name="dicParams"></param>
         /// <param name="strFromName"></param>
         /// <returns></returns>
        SendDataPackage SPPublicMessage(Dictionary<string, string> dicParams)
        {
            // フォーマット
            // PUBLIC-MESSAGE   MESSAGE:メッセージ(B64)
            //
            SendDataPackage sdp = new SendDataPackage();
            foreach (KeyValuePair<string, FTcpClient> connection in _connections)
                sdp.Items.Add(new TargetNameAndCommandParams(
                      connection.Key
                    , FProtocolFormat.ServerMessage
                        (
                          dicParams["MESSAGE"]
                        , dicParams["FROM-NAME"]
                        )
                    ));
            return sdp;
        }



        /// <summary>
        /// 特定の相手(単数)にのみメッセージを送信する準備
        /// </summary>
        /// <param name="dicParams"></param>
        /// <param name="strFromName"></param>
        /// <returns></returns>
        SendDataPackage SPPrivateMessage(Dictionary<string, string> dicParams)
        {
            // フォーマット
            // PRIVATE-MESSAGE   TO-NAME:ID(B64)   MESSAGE:メッセージ(B64)
            //
            SendDataPackage sdp = new SendDataPackage();
            foreach (KeyValuePair<string, FTcpClient> connection in _connections)
                if (dicParams["TO-NAME"] == connection.Key)
                    sdp.Items.Add(new TargetNameAndCommandParams(
                          connection.Key
                        , FProtocolFormat.ServerMessage
                            (
                              dicParams["MESSAGE"]
                            , dicParams["FROM-NAME"]
                            )
                        ));
            return sdp;
        }




        #endregion


    }
}
