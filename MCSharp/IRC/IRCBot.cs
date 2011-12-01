using System;
using System.Collections.Generic;
using System.Text;
using Meebey.SmartIrc4net;
using System.Threading;

namespace MCSharp
{
    class IRCBot
    {
        static IrcClient irc = new IrcClient();
        static string server = Properties.IRCServer;
        static string channel = Properties.IRCChannel;
        static string nick = Properties.IRCNick;
        static Thread ircThread;

        static string[] names;

        public IRCBot()
        {
            // The IRC Bot must run in a seperate thread, or else the server will freeze.
            ircThread = new Thread(new ThreadStart(delegate
            {
                // Attach event handlers
                irc.OnConnecting += new EventHandler(OnConnecting);
                irc.OnConnected += new EventHandler(OnConnected);
                irc.OnChannelMessage += new IrcEventHandler(OnChanMessage);
                irc.OnJoin += new JoinEventHandler(OnJoin);
                irc.OnPart += new PartEventHandler(OnPart);
                irc.OnQuit += new QuitEventHandler(OnQuit);
                irc.OnNickChange += new NickChangeEventHandler(OnNickChange);
                //irc.OnDisconnected += new EventHandler(OnDisconnected);
                irc.OnQueryMessage += new IrcEventHandler(OnPrivMsg);
                irc.OnNames += new NamesEventHandler(OnNames);
                irc.OnChannelAction += new ActionEventHandler(OnAction);

                // Attempt to connect to the IRC server
                try { irc.Connect(server, Properties.IRCPort); }
                catch (Exception ex) { Console.WriteLine("Unnable to connect to IRC server: {0}", ex.Message); }
            }));
            ircThread.Start();
        }

        // While connecting
        void OnConnecting(object sender, EventArgs e)
        {
            Logger.Log("Connecting to IRC");
        }
        // When connected
        void OnConnected(object sender, EventArgs e)
        {
            Logger.Log("Connected to IRC");
            irc.Login(nick, nick, 0, nick);

            // Check to see if we want to register our bot with nickserv
            
            if (Properties.IRCIdentify && Properties.IRCPassword != string.Empty)
            {
                Logger.Log("Identifying with Nickserv");
                irc.SendMessage(SendType.Message, "nickserv", "IDENTIFY " + Properties.IRCPassword);
            }

            Logger.Log("Joining channel");
            irc.RfcJoin(channel);
           
            
            irc.Listen();
        }

        void OnNames(object sender, NamesEventArgs e)
        {
            names = e.UserList;
        }
        //void OnDisconnected(object sender, EventArgs e)
        //{
        //    try { irc.Connect(server, 6667); }
        //    catch (Exception ex) { Console.WriteLine("Failed to reconnect to IRC"); }
        //}
        // On public channel message
        void OnChanMessage(object sender, IrcEventArgs e)
        {

            string temp = e.Data.Message;
            
            string allowedchars = "1234567890-=qwertyuiop[]\\asdfghjkl;'zxcvbnm,./!@#$%^*()_+QWERTYUIOPASDFGHJKL:\"ZXCVBNM<>? ";

            foreach (char ch in temp)
            {
                if (allowedchars.IndexOf(ch) == -1)
                    temp = temp.Replace(ch.ToString(), "");
            }

            Logger.Log("IRC: " + e.Data.Nick + ": " + temp, LogType.IRCChat);
            Player.GlobalMessage("IRC: &1" + e.Data.Nick + ": &f" + temp);

            //s.Log("IRC: " + e.Data.Nick + ": " + e.Data.Message);
            //Player.GlobalMessage("IRC: &1" + e.Data.Nick + ": &f" + e.Data.Message);
        }
        // When someone joins the IRC
        void OnJoin(object sender, JoinEventArgs e)
        {
            Logger.Log(e.Data.Nick + " has joined the IRC channel", LogType.IRCChat);
            Player.GlobalMessage(e.Data.Nick + " has joined the IRC channel");
            irc.RfcNames(channel);
        }
        // When someone leaves the IRC
        void OnPart(object sender, PartEventArgs e)
        {
            Logger.Log(e.Data.Nick + " has left the IRC channel", LogType.IRCChat);
            Player.GlobalMessage(e.Data.Nick + " has left the IRC channel");
            irc.RfcNames(channel);
        }
        void OnQuit(object sender, QuitEventArgs e)
        {
            Logger.Log(e.Data.Nick + " has Left the IRC channel", LogType.IRCChat);
            Player.GlobalMessage(e.Data.Nick + " has left the IRC channel");
            irc.RfcNames(channel);
        }
        void OnPrivMsg(object sender, IrcEventArgs e)
        {
            Logger.Log("IRC RECIEVING MESSESGE", LogType.Debug);
            if (Server.ircControllers.Contains(e.Data.Nick))
            {
                string cmd;
                string msg;
                int len = e.Data.Message.Split(' ').Length;
                cmd = e.Data.Message.Split(' ')[0];
                if (len > 1)
                    msg = e.Data.Message.Substring(e.Data.Message.IndexOf(' ')).Trim();
                else
                    msg = "";

                //Console.WriteLine(cmd + " : " + msg);
                Logger.Log(cmd + " : " + msg, LogType.Debug);
                switch (cmd)
                {
                    case "kick":
                        Command.all.Find("kick").Use(null, msg); break;
                    case "ban":
                        Command.all.Find("ban").Use(null, msg); break;
                    case "banip":
                        Command.all.Find("banip").Use(null, msg); break;
                    case "setrank":
                        Command.all.Find("setrank").Use(null, msg); break;
                    case "say":
                        irc.SendMessage(SendType.Message, channel, msg); break;
                    default:
                        irc.SendMessage(SendType.CtcpReply, e.Data.Nick, "Fail No Such Command"); break;
                }
            }
        }
        void OnNickChange(object sender, NickChangeEventArgs e)
        {
            string key;
            if (e.NewNickname.Split('|').Length == 2)
            {
                key = e.NewNickname.Split('|')[1];
                if (key != null && key != "")
                {
                    switch (key)
                    {
                        case "AFK":
                            Player.GlobalMessage("IRC: " + e.OldNickname + " is AFK"); Server.afkset.Add(e.OldNickname); break;
                        case "Away":
                            Player.GlobalMessage("IRC: " + e.OldNickname + " is Away"); Server.afkset.Add(e.OldNickname); break;
                    }
                }
            }
            else if (Server.afkset.Contains(e.NewNickname))
            {
                Player.GlobalMessage("IRC: " + e.NewNickname + " is no longer away");
                Server.afkset.Remove(e.NewNickname);
            }
            else
                Player.GlobalMessage("IRC: " + e.OldNickname + " is now known as " + e.NewNickname);

            irc.RfcNames(channel);
        }
        void OnAction(object sender, ActionEventArgs e)
        {
            Player.GlobalMessage("* " + e.Data.Nick + " " + e.ActionMessage);
        }
        
        
        /// <summary>
        /// A simple say method for use outside the bot class
        /// </summary>
        /// <param name="msg">what to send</param>
        public static void Say(string msg)
        {
            if (irc != null && irc.IsConnected && Properties.IRCEnabled)
                irc.SendMessage(SendType.Message, channel, msg);
        }
        public static bool IsConnected()
        {
            if (irc.IsConnected)
                return true;
            else
                return false;
        }
        public static void Reset()
        {
            if (irc.IsConnected)
                irc.Disconnect();
            ircThread = new Thread(new ThreadStart(delegate
            {
                try { irc.Connect(server, Properties.IRCPort); }
                catch (Exception e)
                {
                    Logger.Log("Error Connecting to IRC", LogType.Error);
                    Logger.Log(e.Message, LogType.ErrorMessage);
                }
            }));
            ircThread.Start();
        }
        public static string[] GetConnectedUsers()
        {
            return names;
        }
    }
}
