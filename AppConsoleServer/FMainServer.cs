using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WAF.AppWindowClient;
using WAF.LibCommon;

namespace WAF.AppConsoleServer
{
    class FMainServer
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






        /// <summary>
        /// FMainServerのコンストラクタ
        /// </summary>
        public FMainServer()
        {
            _tcpserver = new FTcpServer();
            _tcpserver.ConnectionRequest += _tcpserver_ConnectionRequest;
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


        #region Classes


        /// <summary>
        /// コマンド名とパラメータ(複数)を格納するクラス
        /// </summary>
        class CommandAndParams
        {
            public string CommandName { get; set; }
            public Dictionary<string, string> Params { get; set; }
        }

        /// <summary>
        /// 特定の相手にデータ送信するときのコマンドとパラメータ格納クラス
        /// </summary>
        class TargetNameAndCommandParams
        {
            public TargetNameAndCommandParams(string strTargetName, string strCommandParams)
            {
                this.TargetName = strTargetName;
                this.CommandParams = strCommandParams;
            }
            public string TargetName { get; set; }
            public string CommandParams { get; set; }
        }

        /// <summary>
        /// データ送信のデータ格納クラス
        /// </summary>
        class SendDataPackage
        {
            public List<TargetNameAndCommandParams> Items = new List<TargetNameAndCommandParams>();
        }


        #endregion



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
            SendDataPackage sdp = ServerProcess(strReceiveData);

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
        SendDataPackage ServerProcess(string strReceiveData)
        {
            // コマンドとパラメータを分割する
            CommandAndParams cap = GetCommandParams(strReceiveData);

            // サーバーのメイン処理を行う
            SendDataPackage sdp = ServerProcessSwitch(cap);

            return sdp;
        }


        /// <summary>
        /// コマンド名とパラメータを分割して返す
        /// </summary>
        /// <param name="strReceivedData"></param>
        /// <returns></returns>
        CommandAndParams GetCommandParams(string strReceivedData)
        {
            CommandAndParams result = new CommandAndParams();

            string[] arNameAndValue = strReceivedData.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
            string strValue = "";

            if (1 <= arNameAndValue.Length)
                // コマンド名は大小文字の区別をなくすため、強制的に大文字にする
                result.CommandName = arNameAndValue[0].ToUpper();

            Dictionary<string, string> dicParams = new Dictionary<string, string>();
            for (int i = 1; i < arNameAndValue.Length; i++)
            {
                // ”パラメータ名:パラメータ値”の区切りを分割する
                string[] arParamNameAndValues = strValue.Split(new char[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);
                // パラメータ名は大小文字の区別をなくすため、強制的に大文字にする
                string strParamName = arParamNameAndValues[0].ToUpper();

                if (1 == arParamNameAndValues.Length)
                {
                    // パラメータ値なしは、
                    // パラメータ名のみパラメータ辞書に追加する
                    dicParams.Add(strParamName, "");
                }
                else if (2 <= arParamNameAndValues.Length)
                {
                    // パラメータをBase64デコードして、パラメータ辞書に追加する
                    byte[] binValue = System.Text.Encoding.UTF8.GetBytes(arParamNameAndValues[1]);
                    string strPlainValue = System.Convert.ToBase64String(binValue);
                    dicParams.Add(strParamName, strPlainValue);
                }
            }
            result.Params = dicParams;

            return result;
        }

        /// <summary>
        /// 特定の相手にデータを送信する
        /// </summary>
        /// <param name="sdp"></param>
        void SendTo(SendDataPackage sdp)
        {
            foreach (TargetNameAndCommandParams item in sdp.Items)
            {
                FTcpClient connection = GetConnectionFromName(item.TargetName);
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
        SendDataPackage ServerProcessSwitch(CommandAndParams cap)
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
        /// コマンド送信用フォーマット(全員にメッセージを送信する)
        /// </summary>
        /// <param name="strMessae"></param>
        /// <returns></returns>
        string SFPublicMessage(string strMessae)
        {
            return string.Format("{0}\tMESSAGE:{1}", "PUBLIC-MESSAGE", strMessae);
        }
        
        /// <summary>
         /// 全員にメッセージを送信する準備
         /// </summary>
         /// <param name="dicParams"></param>
         /// <returns></returns>
        SendDataPackage SPPublicMessage(Dictionary<string, string> dicParams)
        {
            // フォーマット
            // PUBLIC-MESSAGE   MESSAGE:メッセージ(B64)
            //
            SendDataPackage sdp = new SendDataPackage();
            foreach (KeyValuePair<string, FTcpClient> connection in _connections)
                sdp.Items.Add(new TargetNameAndCommandParams(connection.Key, SFPublicMessage(dicParams["MESSAGE"])));
            return sdp;
        }



        /// <summary>
        /// コマンド送信用フォーマット(特定の相手(単数)にのみメッセージを送信する)
        /// </summary>
        /// <param name="strMessae"></param>
        /// <param name="strTargetName"></param>
        /// <returns></returns>
        string SFPrivateMessage(string strMessae, string strTargetName)
        {
            return string.Format("{0}\tMESSAGE:{1}\tTARGET-NAME:{2}", "PRIVATE-MESSAGE", strMessae, strTargetName);
        }

        /// <summary>
        /// 特定の相手(単数)にのみメッセージを送信する準備
        /// </summary>
        /// <param name="dicParams"></param>
        /// <returns></returns>
        SendDataPackage SPPrivateMessage(Dictionary<string, string> dicParams)
        {
            // フォーマット
            // PRIVATE-MESSAGE   TARGET-NAME:ID(B64)   MESSAGE:メッセージ(B64)
            //
            SendDataPackage sdp = new SendDataPackage();
            foreach (KeyValuePair<string, FTcpClient> connection in _connections)
                if (dicParams["TARGET-NAME"] == connection.Key)
                    sdp.Items.Add(new TargetNameAndCommandParams(connection.Key, SFPrivateMessage(dicParams["MESSAGE"], connection.Key)));
            return sdp;
        }




        #endregion


    }
}
