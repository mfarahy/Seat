namespace Seat
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timer_persec = new System.Windows.Forms.Timer(this.components);
            this.lsvLogs = new System.Windows.Forms.ListView();
            this.chTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chLogText = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblTradeUpdate = new System.Windows.Forms.Label();
            this.btnLoad = new System.Windows.Forms.Button();
            this.pbProgressBar = new System.Windows.Forms.ProgressBar();
            this.btnStart = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // timer_persec
            // 
            this.timer_persec.Interval = 1000;
            // 
            // lsvLogs
            // 
            this.lsvLogs.AutoArrange = false;
            this.lsvLogs.BackColor = System.Drawing.Color.Black;
            this.lsvLogs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chTime,
            this.chLogText});
            this.lsvLogs.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lsvLogs.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lsvLogs.FullRowSelect = true;
            this.lsvLogs.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lsvLogs.HideSelection = false;
            this.lsvLogs.Location = new System.Drawing.Point(0, 246);
            this.lsvLogs.MultiSelect = false;
            this.lsvLogs.Name = "lsvLogs";
            this.lsvLogs.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lsvLogs.Size = new System.Drawing.Size(476, 146);
            this.lsvLogs.TabIndex = 17;
            this.lsvLogs.UseCompatibleStateImageBehavior = false;
            this.lsvLogs.View = System.Windows.Forms.View.Details;
            // 
            // chTime
            // 
            this.chTime.Text = "Time";
            this.chTime.Width = 87;
            // 
            // chLogText
            // 
            this.chLogText.Text = "Log Text";
            this.chLogText.Width = 500;
            // 
            // lblTradeUpdate
            // 
            this.lblTradeUpdate.AutoSize = true;
            this.lblTradeUpdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.lblTradeUpdate.Location = new System.Drawing.Point(449, 225);
            this.lblTradeUpdate.Name = "lblTradeUpdate";
            this.lblTradeUpdate.Size = new System.Drawing.Size(26, 18);
            this.lblTradeUpdate.TabIndex = 18;
            this.lblTradeUpdate.Text = "🕒";
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(12, 12);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(174, 23);
            this.btnLoad.TabIndex = 22;
            this.btnLoad.Text = "Warmup";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // pbProgressBar
            // 
            this.pbProgressBar.Location = new System.Drawing.Point(12, 231);
            this.pbProgressBar.Name = "pbProgressBar";
            this.pbProgressBar.Size = new System.Drawing.Size(431, 12);
            this.pbProgressBar.TabIndex = 29;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(12, 41);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(174, 23);
            this.btnStart.TabIndex = 30;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(476, 392);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.pbProgressBar);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.lblTradeUpdate);
            this.Controls.Add(this.lsvLogs);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Timer timer_persec;
        private System.Windows.Forms.ListView lsvLogs;
        private System.Windows.Forms.ColumnHeader chTime;
        private System.Windows.Forms.ColumnHeader chLogText;
        private System.Windows.Forms.Label lblTradeUpdate;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.ProgressBar pbProgressBar;
        private System.Windows.Forms.Button btnStart;
    }
}

