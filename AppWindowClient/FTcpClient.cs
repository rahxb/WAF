using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

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

        public FTcpClient()
        {
            //
        }

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



        /// <summary>
        /// データを送信する
        /// </summary>
        /// <param name="bin"></param>
        public async void SendData(byte[] bin)
        {
            await _client.GetStream().WriteAsync(bin, 0, bin.Length);
        }

        /// <summary>
        /// 受信処理を行う
        /// </summary>
        public async void Loop()
        {
            System.IO.MemoryStream mem = new System.IO.MemoryStream();
            byte[] binReadBuffer = new byte[1024];
            while (true)
            {
                try
                {
                    System.Threading.Thread.Sleep(1);

                    // 非同期でデータを受信する
                    int readbytes = await _client.GetStream().ReadAsync(binReadBuffer, 0, binReadBuffer.Length);
                    if (0 < readbytes)
                    {
                        // 受信バッファのデータを整形する
                        mem.Seek(0, System.IO.SeekOrigin.Begin);
                        mem.Write(binReadBuffer, 0, readbytes);
                        byte[] binData = mem.ToArray();
                        mem.SetLength(0);

                        // 受信イベントを発生する
                        RiseEvent_ReceiveData(binData);
                    }
                }
                catch (Exception ex)
                {
                    // 
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }
    }


}
