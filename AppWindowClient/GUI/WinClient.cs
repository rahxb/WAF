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
            cl = new TcpClient();
            cl.Connect("localhost", 1000);
            FTcpClient c = new FTcpClient(cl);
            c.ReceiveData += c_ReceiveData;
            c.StartReceive();

            btnConnectToServer.Enabled = false;
            btnSendDataToServer.Enabled = true;
        }

        /// <summary>
        /// サーバーにデータ送信するボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnSendDataToServer_Click(object sender, EventArgs e)
        {
            btnSendDataToServer.Enabled = false;

            // 送信データを取得して、テキストボックスを空にする
            string strSendData = txtSendData.Text;
            txtSendData.Text = "";

            // 送信データをサーバーに送信する
            byte[] bin = FString.DataToByteArray(FProtocolFormat.ClientMessage(strSendData) + "\r\n");
            await cl.GetStream().WriteAsync(bin, 0, bin.Length);

            btnSendDataToServer.Enabled = true;
        }

        /// <summary>
        /// サーバーからデータ受信イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void c_ReceiveData(object sender, FTcpClient.RecvEventArgs e)
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
