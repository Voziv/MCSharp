using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;


using MCSharp;
using MCSharp.Heartbeat;

namespace MCSharp_GUI
{
    public partial class mainForm : Form
    {
        BindingList<string> playerList = new BindingList<string>();
        Server server;
        LogType LogTypes;
        Process thisProcess = Process.GetCurrentProcess();

        delegate void SetTextCallback(String message);
        delegate void AppendTextCallback(String message, Color clr);

        public mainForm()
        {
            InitializeComponent();
        }

        private void UpdateUrl(string url)
        {
            // Update server url
            SetTextCallback d = new SetTextCallback(UpdateUrlTextbox);
            this.Invoke(d, new object[] { url });
        }

        private void UpdateUrlTextbox(string url)
        {
            externalURLTextbox.Text = url;
            playNowButton.Enabled = true;
        }

        private void PCTimer_Tick(object sender, EventArgs e)
        {
            MemoryStatusLabel.Text = Math.Round((double)Environment.WorkingSet / 1048576).ToString() + "MB";
            if (Math.Round((double)Environment.WorkingSet / 1048576) > 2048)
            {
                thisProcess.Kill();
            }

            // Update heartbeat 
            if (MinecraftHeartbeat.MissedBeats > 4)
            {
                serverStatusLabel.BackColor = Color.Red;
                serverStatusLabel.Text = "Heartbeat Error!";
            }
            else if (MinecraftHeartbeat.Hash != null)
            {
                serverStatusLabel.BackColor = Color.Green;
                serverStatusLabel.Text = "Heartbeat OK!";
            }

            if (MCSharpUpdateHeartbeat.Instance.UpdateAvailable)
            {
                updateAvailableLabel.DisplayStyle = ToolStripItemDisplayStyle.Text;
                updateAvailableLabel.Text = String.Format("NEW Version {0} available!", Server.LatestVersion);
            }
            else
            {
                updateAvailableLabel.DisplayStyle = ToolStripItemDisplayStyle.None;
            }
        }

        private void FileExitMenu_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            PCTimer.Start();
            updateAvailableLabel.DisplayStyle = ToolStripItemDisplayStyle.None;
            LogTypes |= LogType.Information;
            LogTypes |= LogType.Warning;
            LogTypes |= LogType.Error;
            LogTypes |= LogType.FatalError;
            LogTypes |= LogType.UserCommand;
            LogTypes |= LogType.UserActivity;
            LogTypes |= LogType.SuspiciousActivity;
            LogTypes |= LogType.ConsoleOutput;
            LogTypes |= LogType.IRCChat;
            LogTypes |= LogType.PrivateChat;
            LogTypes |= LogType.WorldChat;
            LogTypes |= LogType.OpChat;
            LogTypes |= LogType.GlobalChat;
            
            server = new Server();
            server.OnSettingsUpdate += SettingsUpdate;
            server.OnURLChange += UpdateUrl;

            playerListbox.DataSource = Player.players;
            Player.players.ListChanged += new ListChangedEventHandler(players_ListChanged);
            Logger.OnLog += UpdateLog;
            BGWorker1.RunWorkerAsync();
        }

        public void players_ListChanged(object o, ListChangedEventArgs args)
        {
            ProgramStatusLabel.Text = Player.players.Count + " Players Connected";
        }

        public void Log(string message)
        {
            Log(message, consoleLogTextbox.ForeColor);
        }

        public void Log(string message, Color clr)
        {
            this.consoleLogTextbox.AppendText(message, clr);
            if (consoleLogAutoScrollCheckbox.Checked)
            {
                this.consoleLogTextbox.SelectionStart = consoleLogTextbox.Text.Length;
                this.consoleLogTextbox.ScrollToCaret();
            }
        }

        
        public void UpdateLog(string message, LogType type, LogSource src)
        {
            Color clr = consoleLogTextbox.ForeColor;
            try
            {
                if ((LogType.Warning & type) == type)
                    clr = Color.Yellow;
                else if ((LogType.OpChat & type) == type || (LogType.GlobalChat & type) == type || (LogType.IRCChat & type) == type || (LogType.GlobalChat & type) == type)
                    clr = Color.White;
                else if ((LogType.Error & type) == type || (LogType.FatalError & type) == type || (LogType.ErrorMessage & type) == type)
                    clr = Color.Red;

                message = Environment.NewLine + "[" + DateTime.Now.ToString("hh:mm:ss tt") + "]"
                               + "[" + Enum.GetName(typeof(LogType), type).Substring(0, 1) + "]"
                               + " - " + message;

                if ((LogTypes & type) == type || ((LogType.Debug & type) == type && MCSharp.Properties.DebugEnabled))
                {
                    if (this.consoleLogTextbox.InvokeRequired)
                    {
                        AppendTextCallback d = new AppendTextCallback(Log);
                        this.Invoke(d, new object[] { message, clr });
                    }
                    else
                    {
                        Log(message, clr);
                    }
                }
            }
            catch (Exception) {  }
        }

        private void SettingsUpdate()
        {
            string message = MCSharp.Properties.ServerName;
            if (this.consoleLogTextbox.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(UpdateTitle);
                this.Invoke(d, new object[] { message });
            }
            else
            {
                UpdateTitle(message);
            }
            
        }

        private void UpdateTitle(String message)
        {
            this.Text = "MCSharp - " + message;
        }

        private void mainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            server.Stop();
            Logger.OnLog -= UpdateLog;
        }

        private void BGWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
            server.Start();
        }

        private void autoScrollCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (consoleLogAutoScrollCheckbox.Checked)
            {
                this.consoleLogTextbox.SelectionStart = consoleLogTextbox.Text.Length;
                this.consoleLogTextbox.ScrollToCaret();
            }
        }

        private void sendChatButton_Click(object sender, EventArgs e)
        {
            server.ParseInput(chatTextbox.Text);
            chatTextbox.Text = String.Empty;
        }

        private void playNowButton_Click (object sender, EventArgs e)
        {
            Process.Start(externalURLTextbox.Text);
        }


    }
}
