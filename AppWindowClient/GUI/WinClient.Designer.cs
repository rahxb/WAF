namespace WAF.AppWindowClient
{
    partial class WinClient
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnConnectToServer = new System.Windows.Forms.Button();
            this.btnSendDataToServer = new System.Windows.Forms.Button();
            this.txtSendData = new System.Windows.Forms.TextBox();
            this.txtRecvDataList = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnConnectToServer
            // 
            this.btnConnectToServer.Location = new System.Drawing.Point(12, 12);
            this.btnConnectToServer.Name = "btnConnectToServer";
            this.btnConnectToServer.Size = new System.Drawing.Size(168, 38);
            this.btnConnectToServer.TabIndex = 0;
            this.btnConnectToServer.Text = "サーバーに接続";
            this.btnConnectToServer.UseVisualStyleBackColor = true;
            this.btnConnectToServer.Click += new System.EventHandler(this.btnConnectToServer_Click);
            // 
            // btnSendDataToServer
            // 
            this.btnSendDataToServer.Location = new System.Drawing.Point(229, 66);
            this.btnSendDataToServer.Name = "btnSendDataToServer";
            this.btnSendDataToServer.Size = new System.Drawing.Size(118, 25);
            this.btnSendDataToServer.TabIndex = 1;
            this.btnSendDataToServer.Text = "送 信";
            this.btnSendDataToServer.UseVisualStyleBackColor = true;
            this.btnSendDataToServer.Click += new System.EventHandler(this.btnSendDataToServer_Click);
            // 
            // txtSendData
            // 
            this.txtSendData.Location = new System.Drawing.Point(12, 66);
            this.txtSendData.Name = "txtSendData";
            this.txtSendData.Size = new System.Drawing.Size(211, 25);
            this.txtSendData.TabIndex = 2;
            // 
            // txtRecvDataList
            // 
            this.txtRecvDataList.Location = new System.Drawing.Point(12, 108);
            this.txtRecvDataList.Multiline = true;
            this.txtRecvDataList.Name = "txtRecvDataList";
            this.txtRecvDataList.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtRecvDataList.Size = new System.Drawing.Size(335, 169);
            this.txtRecvDataList.TabIndex = 3;
            // 
            // WinClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 289);
            this.Controls.Add(this.txtRecvDataList);
            this.Controls.Add(this.txtSendData);
            this.Controls.Add(this.btnSendDataToServer);
            this.Controls.Add(this.btnConnectToServer);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WinClient";
            this.ShowIcon = false;
            this.Text = "WAF Client";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnConnectToServer;
        private System.Windows.Forms.Button btnSendDataToServer;
        private System.Windows.Forms.TextBox txtSendData;
        private System.Windows.Forms.TextBox txtRecvDataList;
    }
}

