namespace MCSharp_GUI
{
    partial class mainForm
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
            this.mainFormStatusStrip = new System.Windows.Forms.StatusStrip();
            this.ProgramStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.MemoryStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.updateAvailableLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.serverStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.mainFormMenuStrip = new System.Windows.Forms.MenuStrip();
            this.FileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.FileExitMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpAboutMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.PCTimer = new System.Windows.Forms.Timer(this.components);
            this.BGWorker1 = new System.ComponentModel.BackgroundWorker();
            this.playerListbox = new System.Windows.Forms.ListBox();
            this.playNowButton = new System.Windows.Forms.Button();
            this.externalURLTextbox = new System.Windows.Forms.RichTextBox();
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.sendChatButton = new System.Windows.Forms.Button();
            this.chatTextbox = new System.Windows.Forms.TextBox();
            this.consoleLogAutoScrollCheckbox = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.consoleLogTextbox = new MCSharp_GUI.ConsoleRichTextbox();
            this.mainFormStatusStrip.SuspendLayout();
            this.mainFormMenuStrip.SuspendLayout();
            this.mainTabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainFormStatusStrip
            // 
            this.mainFormStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ProgramStatusLabel,
            this.MemoryStatusLabel,
            this.updateAvailableLabel,
            this.serverStatusLabel});
            this.mainFormStatusStrip.Location = new System.Drawing.Point(0, 464);
            this.mainFormStatusStrip.Name = "mainFormStatusStrip";
            this.mainFormStatusStrip.Size = new System.Drawing.Size(781, 22);
            this.mainFormStatusStrip.SizingGrip = false;
            this.mainFormStatusStrip.TabIndex = 2;
            // 
            // ProgramStatusLabel
            // 
            this.ProgramStatusLabel.Name = "ProgramStatusLabel";
            this.ProgramStatusLabel.Size = new System.Drawing.Size(511, 17);
            this.ProgramStatusLabel.Spring = true;
            this.ProgramStatusLabel.Text = "0 Players Connected";
            this.ProgramStatusLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // MemoryStatusLabel
            // 
            this.MemoryStatusLabel.Name = "MemoryStatusLabel";
            this.MemoryStatusLabel.Size = new System.Drawing.Size(31, 17);
            this.MemoryStatusLabel.Text = "0MB";
            // 
            // updateAvailableLabel
            // 
            this.updateAvailableLabel.BackColor = System.Drawing.Color.Red;
            this.updateAvailableLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.updateAvailableLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.updateAvailableLabel.Name = "updateAvailableLabel";
            this.updateAvailableLabel.Size = new System.Drawing.Size(105, 17);
            this.updateAvailableLabel.Text = "Update Available!";
            // 
            // serverStatusLabel
            // 
            this.serverStatusLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.serverStatusLabel.Name = "serverStatusLabel";
            this.serverStatusLabel.Size = new System.Drawing.Size(119, 17);
            this.serverStatusLabel.Text = "Heartbeat Loading...";
            // 
            // mainFormMenuStrip
            // 
            this.mainFormMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileMenu,
            this.HelpMenu});
            this.mainFormMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainFormMenuStrip.Name = "mainFormMenuStrip";
            this.mainFormMenuStrip.Size = new System.Drawing.Size(781, 24);
            this.mainFormMenuStrip.TabIndex = 3;
            this.mainFormMenuStrip.Text = "MenuStrip1";
            // 
            // FileMenu
            // 
            this.FileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileExitMenu});
            this.FileMenu.Name = "FileMenu";
            this.FileMenu.Size = new System.Drawing.Size(37, 20);
            this.FileMenu.Text = "&File";
            // 
            // FileExitMenu
            // 
            this.FileExitMenu.Name = "FileExitMenu";
            this.FileExitMenu.Size = new System.Drawing.Size(92, 22);
            this.FileExitMenu.Text = "E&xit";
            this.FileExitMenu.Click += new System.EventHandler(this.FileExitMenu_Click);
            // 
            // HelpMenu
            // 
            this.HelpMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.HelpAboutMenu});
            this.HelpMenu.Name = "HelpMenu";
            this.HelpMenu.Size = new System.Drawing.Size(44, 20);
            this.HelpMenu.Text = "&Help";
            // 
            // HelpAboutMenu
            // 
            this.HelpAboutMenu.Name = "HelpAboutMenu";
            this.HelpAboutMenu.Size = new System.Drawing.Size(107, 22);
            this.HelpAboutMenu.Text = "&About";
            // 
            // PCTimer
            // 
            this.PCTimer.Interval = 1000;
            this.PCTimer.Tick += new System.EventHandler(this.PCTimer_Tick);
            // 
            // BGWorker1
            // 
            this.BGWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BGWorker1_DoWork);
            // 
            // playerListbox
            // 
            this.playerListbox.FormattingEnabled = true;
            this.playerListbox.Location = new System.Drawing.Point(646, 81);
            this.playerListbox.Name = "playerListbox";
            this.playerListbox.Size = new System.Drawing.Size(121, 368);
            this.playerListbox.TabIndex = 5;
            // 
            // playNowButton
            // 
            this.playNowButton.Enabled = false;
            this.playNowButton.Location = new System.Drawing.Point(565, 25);
            this.playNowButton.Name = "playNowButton";
            this.playNowButton.Size = new System.Drawing.Size(75, 23);
            this.playNowButton.TabIndex = 13;
            this.playNowButton.Text = "Play Now";
            this.playNowButton.UseVisualStyleBackColor = true;
            this.playNowButton.Click += new System.EventHandler(this.playNowButton_Click);
            // 
            // externalURLTextbox
            // 
            this.externalURLTextbox.Location = new System.Drawing.Point(15, 27);
            this.externalURLTextbox.Name = "externalURLTextbox";
            this.externalURLTextbox.ReadOnly = true;
            this.externalURLTextbox.Size = new System.Drawing.Size(544, 23);
            this.externalURLTextbox.TabIndex = 16;
            this.externalURLTextbox.Text = "Waiting for External URL...";
            // 
            // mainTabControl
            // 
            this.mainTabControl.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.mainTabControl.Controls.Add(this.tabPage1);
            this.mainTabControl.Controls.Add(this.tabPage2);
            this.mainTabControl.HotTrack = true;
            this.mainTabControl.Location = new System.Drawing.Point(15, 56);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(629, 397);
            this.mainTabControl.TabIndex = 20;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Transparent;
            this.tabPage1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabPage1.Controls.Add(this.sendChatButton);
            this.tabPage1.Controls.Add(this.consoleLogTextbox);
            this.tabPage1.Controls.Add(this.chatTextbox);
            this.tabPage1.Controls.Add(this.consoleLogAutoScrollCheckbox);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(621, 368);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Console";
            // 
            // sendChatButton
            // 
            this.sendChatButton.Location = new System.Drawing.Point(485, 339);
            this.sendChatButton.Name = "sendChatButton";
            this.sendChatButton.Size = new System.Drawing.Size(53, 23);
            this.sendChatButton.TabIndex = 22;
            this.sendChatButton.Text = "Send";
            this.sendChatButton.UseVisualStyleBackColor = true;
            this.sendChatButton.Click += new System.EventHandler(this.sendChatButton_Click);
            // 
            // chatTextbox
            // 
            this.chatTextbox.Location = new System.Drawing.Point(6, 340);
            this.chatTextbox.MaxLength = 150;
            this.chatTextbox.Name = "chatTextbox";
            this.chatTextbox.Size = new System.Drawing.Size(473, 20);
            this.chatTextbox.TabIndex = 20;
            // 
            // consoleLogAutoScrollCheckbox
            // 
            this.consoleLogAutoScrollCheckbox.AutoSize = true;
            this.consoleLogAutoScrollCheckbox.Checked = true;
            this.consoleLogAutoScrollCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.consoleLogAutoScrollCheckbox.Location = new System.Drawing.Point(545, 342);
            this.consoleLogAutoScrollCheckbox.Name = "consoleLogAutoScrollCheckbox";
            this.consoleLogAutoScrollCheckbox.Size = new System.Drawing.Size(72, 17);
            this.consoleLogAutoScrollCheckbox.TabIndex = 18;
            this.consoleLogAutoScrollCheckbox.Text = "Autoscroll";
            this.consoleLogAutoScrollCheckbox.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.Transparent;
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(621, 368);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Server.Properties";
            // 
            // consoleLogTextbox
            // 
            this.consoleLogTextbox.BackColor = System.Drawing.Color.Black;
            this.consoleLogTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.consoleLogTextbox.ForeColor = System.Drawing.Color.LightGray;
            this.consoleLogTextbox.Location = new System.Drawing.Point(6, 6);
            this.consoleLogTextbox.Name = "consoleLogTextbox";
            this.consoleLogTextbox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.consoleLogTextbox.Size = new System.Drawing.Size(611, 327);
            this.consoleLogTextbox.TabIndex = 21;
            this.consoleLogTextbox.Text = "";
            // 
            // mainForm
            // 
            this.AcceptButton = this.sendChatButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(781, 486);
            this.Controls.Add(this.mainTabControl);
            this.Controls.Add(this.externalURLTextbox);
            this.Controls.Add(this.playNowButton);
            this.Controls.Add(this.playerListbox);
            this.Controls.Add(this.mainFormStatusStrip);
            this.Controls.Add(this.mainFormMenuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "mainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MCSharp - Server Name";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.mainForm_FormClosing);
            this.Load += new System.EventHandler(this.mainForm_Load);
            this.mainFormStatusStrip.ResumeLayout(false);
            this.mainFormStatusStrip.PerformLayout();
            this.mainFormMenuStrip.ResumeLayout(false);
            this.mainFormMenuStrip.PerformLayout();
            this.mainTabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.StatusStrip mainFormStatusStrip;
        internal System.Windows.Forms.ToolStripStatusLabel ProgramStatusLabel;
        internal System.Windows.Forms.ToolStripStatusLabel MemoryStatusLabel;
        internal System.Windows.Forms.MenuStrip mainFormMenuStrip;
        internal System.Windows.Forms.ToolStripMenuItem FileMenu;
        internal System.Windows.Forms.ToolStripMenuItem FileExitMenu;
        internal System.Windows.Forms.ToolStripMenuItem HelpMenu;
        internal System.Windows.Forms.ToolStripMenuItem HelpAboutMenu;
        internal System.Windows.Forms.Timer PCTimer;
        private System.ComponentModel.BackgroundWorker BGWorker1;
        private System.Windows.Forms.ListBox playerListbox;
        private System.Windows.Forms.Button playNowButton;
        private System.Windows.Forms.RichTextBox externalURLTextbox;
        private System.Windows.Forms.ToolStripStatusLabel serverStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel updateAvailableLabel;
        private System.Windows.Forms.TabControl mainTabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private ConsoleRichTextbox consoleLogTextbox;
        private System.Windows.Forms.TextBox chatTextbox;
        private System.Windows.Forms.CheckBox consoleLogAutoScrollCheckbox;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button sendChatButton;
    }
}

