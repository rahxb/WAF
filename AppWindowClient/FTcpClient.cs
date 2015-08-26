using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using WAF.LibCommon;
using System.IO;

namespace WAF.AppWindowClient
{
    public class FTcpClient
    {

        #region RecvDataイベント発生用

        public class RecvEventArgs : EventArgs
        {
            public byte[] data;
        }
        public event EventHandler<RecvEventArgs> ReceiveData = null;
        void RiseEvent_ReceiveData(byte[] data)
        {
            if (ReceiveData != null)
            {
                RecvEventArgs e = new RecvEventArgs();
                e.data = data;
                ReceiveData(this, e);
            }
        }

        #endregion

        TcpClient _client = null;
        
        public FTcpClient(TcpClient c)
        {
            _client = c;
        }

        /// <summary>
        /// 受信処理を開始する(非同期)
        /// </summary>
        public async void StartReceive()
        {
            await Task.Factory.StartNew(() => Loop());
        }

        public void Close()
        {
            if(_client != null)
                _client.Close();
        }

        /// <summary>
        /// データを送信する
        /// </summary>
        /// <param name="bin"></param>
        public void SendDataNewLine(string strData)
        {
            SendData(strData + "\n");
        }
        public async void SendData(string strData)
        {
            byte[] binData = FString.DataToByteArray(strData);
            await _client.GetStream().WriteAsync(binData, 0, binData.Length);
        }
        /// <summary>
        /// 受信処理を行う
        /// </summary>
        public async void Loop()
        {
            MemoryStream memTemp = new MemoryStream();
            MemoryStream memBuffer = new MemoryStream();
            byte[] binReadBuffer = new byte[1024];

            while (true)
            {
                try
                {
                    // 無限ループのためCPU負荷軽減のため 1ms 待機
                    System.Threading.Thread.Sleep(1);

                    // 非同期でデータを受信する
                    int nReadbytes = await _client.GetStream().ReadAsync(binReadBuffer, 0, binReadBuffer.Length);
                    
                    // 直前の余りデータの後に受信データを追加する（バッファに受信データを追加）
                    memBuffer.Write(binReadBuffer, 0, nReadbytes);
                    binReadBuffer = memBuffer.ToArray();
                    nReadbytes = binReadBuffer.Length;

                    // binReadBuffer配列にデータがあるため
                    // バッファが不要となり空にする
                    FToolKit.ClearMemoryStream(memBuffer);

                    // 受信データ＋バッファデータのサイズが1以上なら次の処理
                    if (0 < nReadbytes)
                    {

                        // 改行で終わってない場合はデータを繰り越す(前処理)
                        int nStartIndex = 0;
                        int nCount = 0;
                        for (int i = 0; i < nReadbytes; i++)
                        {
                            nCount++;
                            if (binReadBuffer[i] == '\n')
                            {

                                // 取得開始位置と取得終了位置（改行まで）のバイト配列を取得する
                                FToolKit.ClearMemoryStream(memTemp);
                                memTemp.Write(binReadBuffer, nStartIndex, nCount);
                                byte[] binData = memTemp.ToArray();
                                FToolKit.ClearMemoryStream(memTemp);

                                // 次の開始位置を設定し、カウントも0にリセットする
                                nStartIndex = i + 1;
                                nCount = 0;

                                // 受信イベントを発生する
                                RiseEvent_ReceiveData(binData);
                            }
                        }

                        if (nStartIndex == 0)
                        {
                            // 改行がないためすべて余りデータとしてバッファに追加し
                            // 次回のループに回す
                            memBuffer.Write(binReadBuffer, 0, nReadbytes);
                        }
                        else
                        {
                            // 余りデータがあるならバッファに追加して
                            // 次回ループに回す
                            if (0 < nCount && nStartIndex + nCount != nReadbytes)
                                memBuffer.Write(binReadBuffer, nStartIndex, nCount);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 
                    System.Diagnostics.Debug.WriteLine(ex.Message);

                    FToolKit.ClearMemoryStream(memBuffer);
                    FToolKit.ClearMemoryStream(memTemp);
                }
            }
        }
    }


}
