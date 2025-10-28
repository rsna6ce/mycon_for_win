
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonScan = new System.Windows.Forms.Button();
            this.listBoxIPaddrs = new System.Windows.Forms.ListBox();
            this.textBoxLatestMessage = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.checkBoxLocalLP = new System.Windows.Forms.CheckBox();
            this.buttonAddIPAddr = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonStart
            // 
            this.buttonStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonStart.Location = new System.Drawing.Point(4, 152);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(120, 23);
            this.buttonStart.TabIndex = 0;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonStop.Location = new System.Drawing.Point(128, 152);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(116, 23);
            this.buttonStop.TabIndex = 0;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // buttonScan
            // 
            this.buttonScan.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonScan.Location = new System.Drawing.Point(4, 4);
            this.buttonScan.Name = "buttonScan";
            this.buttonScan.Size = new System.Drawing.Size(208, 23);
            this.buttonScan.TabIndex = 0;
            this.buttonScan.Text = "Scan";
            this.buttonScan.UseVisualStyleBackColor = true;
            this.buttonScan.Click += new System.EventHandler(this.buttonScan_Click);
            // 
            // listBoxIPaddrs
            // 
            this.listBoxIPaddrs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxIPaddrs.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.listBoxIPaddrs.FormattingEnabled = true;
            this.listBoxIPaddrs.ItemHeight = 12;
            this.listBoxIPaddrs.Location = new System.Drawing.Point(4, 32);
            this.listBoxIPaddrs.Name = "listBoxIPaddrs";
            this.listBoxIPaddrs.Size = new System.Drawing.Size(240, 88);
            this.listBoxIPaddrs.TabIndex = 1;
            // 
            // textBoxLatestMessage
            // 
            this.textBoxLatestMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxLatestMessage.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBoxLatestMessage.Location = new System.Drawing.Point(120, 124);
            this.textBoxLatestMessage.Name = "textBoxLatestMessage";
            this.textBoxLatestMessage.ReadOnly = true;
            this.textBoxLatestMessage.Size = new System.Drawing.Size(124, 19);
            this.textBoxLatestMessage.TabIndex = 2;
            this.textBoxLatestMessage.Text = "________________";
            // 
            // timer1
            // 
            this.timer1.Interval = 1;
            // 
            // checkBoxLocalLP
            // 
            this.checkBoxLocalLP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxLocalLP.AutoSize = true;
            this.checkBoxLocalLP.Checked = true;
            this.checkBoxLocalLP.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxLocalLP.Location = new System.Drawing.Point(4, 128);
            this.checkBoxLocalLP.Name = "checkBoxLocalLP";
            this.checkBoxLocalLP.Size = new System.Drawing.Size(97, 16);
            this.checkBoxLocalLP.TabIndex = 3;
            this.checkBoxLocalLP.Text = "local loopback";
            this.checkBoxLocalLP.UseVisualStyleBackColor = true;
            // 
            // buttonAddIPAddr
            // 
            this.buttonAddIPAddr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAddIPAddr.Location = new System.Drawing.Point(216, 4);
            this.buttonAddIPAddr.Name = "buttonAddIPAddr";
            this.buttonAddIPAddr.Size = new System.Drawing.Size(28, 23);
            this.buttonAddIPAddr.TabIndex = 0;
            this.buttonAddIPAddr.Text = "+";
            this.buttonAddIPAddr.UseVisualStyleBackColor = true;
            this.buttonAddIPAddr.Click += new System.EventHandler(this.buttonAddIPAddr_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(255, 186);
            this.Controls.Add(this.checkBoxLocalLP);
            this.Controls.Add(this.textBoxLatestMessage);
            this.Controls.Add(this.listBoxIPaddrs);
            this.Controls.Add(this.buttonScan);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonAddIPAddr);
            this.Controls.Add(this.buttonStart);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
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
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.CheckBox checkBoxLocalLP;
        private System.Windows.Forms.Button buttonAddIPAddr;
    }
}

