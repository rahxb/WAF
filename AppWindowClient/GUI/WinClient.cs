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

        System.Threading.Timer _tmrCheckConnected;
        int _CheckConnectedIntervalMS = 5000;


        /// <summary>
        /// WinClientのコンストラクタ
        /// </summary>
        public WinClient()
        {
            InitializeComponent();

            // ボタン状態を初期化
            btnConnectToServer.Enabled = true;
            btnSendDataToServer.Enabled = false;

            _tmrCheckConnected = new System.Threading.Timer(new System.Threading.TimerCallback(_tmrCheckConnected_Tick), null, 0, _CheckConnectedIntervalMS);
        }

        void _tmrCheckConnected_Tick(object e)
        {
            if (_tcpClient != null && _tcpClient.IsConnected == false)
            {
                if (this.InvokeRequired)
                    // スレッド上であればUIスレッドに再帰
                    this.Invoke(new MethodInvoker(() => { _tmrCheckConnected_Tick(e); }));
                else
                {
                    _tcpClient.Close();
                    _tcpClient = null;

                    txtRecvDataList.AppendText("接続が切れました");
                }
            }
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
            _tcpClient.Closed += _tcpClient_Closed;
            _tcpClient.StartReceive();

            btnConnectToServer.Enabled = false;
            btnSendDataToServer.Enabled = true;
        }

        private void _tcpClient_Closed(object sender, EventArgs e)
        {
            txtRecvDataList.AppendText("Closed\r\n");
        }

        /// <summary>
        /// サーバーにデータ送信するボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSendDataToServer_Click(object sender, EventArgs e)
        {
            btnSendDataToServer.Enabled = false;

            if (_tcpClient != null)
            {
                // 送信データを取得して、テキストボックスを空にする
                string strSendData = txtSendData.Text;
                txtSendData.Text = "";

                // 送信データをサーバーに送信する
                _tcpClient.SendData(FProtocolFormat.ClientMessage(strSendData) + "\n");
            }

            btnSendDataToServer.Enabled = true;
        }

        /// <summary>
        /// サーバーからデータ受信イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _tcpClient_ReceiveData(object sender, FTcpClient.RecvEventArgs e)
        {
            recv((FTcpClient)sender, e);
        }

        /// <summary>
        /// データ受信後の処理
        /// </summary>
        /// <param name="c"></param>
        /// <param name="e"></param>
        void recv(FTcpClient c, FTcpClient.RecvEventArgs e)
        {
            if (this.InvokeRequired)
                // スレッド上であればUIスレッドに再帰
                this.Invoke(new MethodInvoker(() => { recv(c, e); }));
            else
            {
                // テキストボックス
                FProtocolFormat.CommandAndParams cap = FProtocolFormat.GetCommandParams(FString.DataToString((e.data)));
                switch (cap.CommandName)
                {
                    case "PUBLIC-MESSAGE":
                        txtRecvDataList.AppendText(string.Format("{0} : {1}\r\n"
                            , cap.Params["FROM-NAME"]
                            , cap.Params["MESSAGE"]
                            ));
                        break;

                    case "PRIVATE-MESSAGE":
                        txtRecvDataList.AppendText(string.Format("{0} : ({1})\r\n"
                            , cap.Params["FROM-NAME"]
                            , cap.Params["MESSAGE"]
                            ));
                        break;

                    case "NOOP":
                        // DEBUG only
                        //System.Threading.Thread.Sleep(6000);

                        c.SendDataNewLine("NOOP");
                        break;
                }
            }
        }




    }
}
