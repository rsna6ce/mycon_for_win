
namespace mycon_for_win
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
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
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonScan = new System.Windows.Forms.Button();
            this.listBoxIPaddrs = new System.Windows.Forms.ListBox();
            this.textBoxLatestMessage = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(4, 124);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(80, 23);
            this.buttonStart.TabIndex = 0;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(92, 124);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(80, 23);
            this.buttonStop.TabIndex = 0;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            // 
            // buttonScan
            // 
            this.buttonScan.Location = new System.Drawing.Point(4, 4);
            this.buttonScan.Name = "buttonScan";
            this.buttonScan.Size = new System.Drawing.Size(172, 23);
            this.buttonScan.TabIndex = 0;
            this.buttonScan.Text = "Scan";
            this.buttonScan.UseVisualStyleBackColor = true;
            // 
            // listBoxIPaddrs
            // 
            this.listBoxIPaddrs.FormattingEnabled = true;
            this.listBoxIPaddrs.ItemHeight = 12;
            this.listBoxIPaddrs.Items.AddRange(new object[] {
            "a",
            "b",
            "c",
            "d",
            "e",
            "f",
            "g",
            "h",
            "o",
            "j",
            "k"});
            this.listBoxIPaddrs.Location = new System.Drawing.Point(4, 32);
            this.listBoxIPaddrs.Name = "listBoxIPaddrs";
            this.listBoxIPaddrs.Size = new System.Drawing.Size(168, 88);
            this.listBoxIPaddrs.TabIndex = 1;
            // 
            // textBoxLatestMessage
            // 
            this.textBoxLatestMessage.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBoxLatestMessage.Location = new System.Drawing.Point(4, 152);
            this.textBoxLatestMessage.Name = "textBoxLatestMessage";
            this.textBoxLatestMessage.ReadOnly = true;
            this.textBoxLatestMessage.Size = new System.Drawing.Size(168, 19);
            this.textBoxLatestMessage.TabIndex = 2;
            this.textBoxLatestMessage.Text = "________________";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(340, 188);
            this.Controls.Add(this.textBoxLatestMessage);
            this.Controls.Add(this.listBoxIPaddrs);
            this.Controls.Add(this.buttonScan);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonStart);
            this.Name = "Form1";
            this.Text = "mycon for windows";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button buttonScan;
        private System.Windows.Forms.ListBox listBoxIPaddrs;
        private System.Windows.Forms.TextBox textBoxLatestMessage;
    }
}

