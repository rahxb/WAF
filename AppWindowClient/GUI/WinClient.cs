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

        TcpClient _cl;
        FTcpClient _tcpClient;


        /// <summary>
        /// WinClientのコンストラクタ
        /// </summary>
        public WinClient()
        {
            InitializeComponent();

            // ボタン状態を初期化
            btnConnectToServer.Enabled = true;
            btnSendDataToServer.Enabled = false;
        }
        
        
        /// <summary>
        /// サーバーに接続するボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConnectToServer_Click(object sender, EventArgs e)
        {
            _cl = new TcpClient();
            _cl.Connect("localhost", 1000);
            _tcpClient = new FTcpClient(_cl);
            _tcpClient.ReceiveData += _tcpClient_ReceiveData;
            _tcpClient.StartReceive();

            btnConnectToServer.Enabled = false;
            btnSendDataToServer.Enabled = true;
        }

        /// <summary>
        /// サーバーにデータ送信するボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSendDataToServer_Click(object sender, EventArgs e)
        {
            btnSendDataToServer.Enabled = false;

            // 送信データを取得して、テキストボックスを空にする
            string strSendData = txtSendData.Text;
            txtSendData.Text = "";

            // 送信データをサーバーに送信する
            _tcpClient.SendData(FProtocolFormat.ClientMessage(strSendData) + "\n" + "aaaaA");

            btnSendDataToServer.Enabled = true;
        }

        /// <summary>
        /// サーバーからデータ受信イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _tcpClient_ReceiveData(object sender, FTcpClient.RecvEventArgs e)
        {
            recv(e);
        }

        /// <summary>
        /// データ受信後の処理
        /// </summary>
        /// <param name="e"></param>
        void recv(FTcpClient.RecvEventArgs e)
        {
            if (this.InvokeRequired)
                // スレッド上であればUIスレッドに再帰
                this.Invoke(new MethodInvoker(() => { recv(e); }));
            else
            {
                // テキストボックス
                FProtocolFormat.CommandAndParams cap = FProtocolFormat.GetCommandParams(FString.DataToString((e.data)));
                if (cap.CommandName == "PUBLIC-MESSAGE" || cap.CommandName == "PRIVATE-MESSAGE")
                    txtRecvDataList.AppendText(string.Format("{0} : {1}\r\n"
                        , cap.Params["FROM-NAME"]
                        , cap.Params["MESSAGE"]
                        ));
            }
        }




    }
}
