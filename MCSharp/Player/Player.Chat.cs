using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MCSharp
{
    public sealed partial class Player
    {
        //spam detection
        public static int spamChatCount = 3;
        public static int spamChatTimer = 4;
        Queue<DateTime> spamChatLog = new Queue<DateTime>(spamChatCount);

        public bool isOpChat = false;
        public bool isWhisperChat = false;
        public string whisperTarget = "";
        public string color { get { return _color; } set { _color = value; } }
        private string _color;



        void HandleChat (byte[] message)
        {
            try
            {
                if (!loggedIn)
                    return;
                if (!group.CanChat)
                    return;

                //byte[] message = (byte[])m;
                string text = Encoding.ASCII.GetString(message, 1, 64).Trim();

                //added by bman
                if (this.isMuted)
                {
                    Logger.Log("(Muted) " + name + ": " + text);
                    return;
                }

                //Added by bman - Joker command
                if (isJoker)
                {
                    if (DateTime.Now.Subtract(lastJoke).TotalSeconds > 3)
                    {
                        Logger.Log("(Joker) " + name + ": " + text, LogType.WorldChat);
                        text = Server.jokerMessages[rand.Next(0, Server.jokerMessages.Count)];
                        lastJoke = DateTime.Now;
                    }
                    else
                    {
                        SendMessage("*Anti-Spam*");
                        return;
                    }
                }

                text = Regex.Replace(text, @"\s\s+", " ");
                foreach (char ch in text)
                {
                    if (ch < 32 || ch >= 127 || ch == '&')
                    {
                        Kick("Illegal character in chat message!");
                        return;
                    }
                }
                if (text.Length == 0)
                    return;
                if (text[0] == '/')
                {
                    text = text.Remove(0, 1);

                    int pos = text.IndexOf(' ');
                    if (pos == -1)
                    {
                        HandleCommand(text.ToLower(), "");
                        return;
                    }
                    string cmd = text.Substring(0, pos).ToLower();
                    string msg = text.Substring(pos + 1);
                    HandleCommand(cmd, msg);
                    return;
                }
                if ((text[0] == '@' || isWhisperChat) && (text[0] != '#'))  // if the user has # in front of their text, manual overide
                {
                    string newtext = "";
                    string to = whisperTarget;
                    string msg = text;

                    if (!isWhisperChat)
                    {
                        newtext = text.Substring(1).Trim();
                        int pos = newtext.IndexOf(' ');
                        if (pos != -1)
                        {
                            to = newtext.Substring(0, pos);
                            msg = newtext.Substring(pos + 1);
                        }
                    }
                    HandleQuery(to, msg);
                    Logger.Log("<" + name + "> to <" + to + ">" + newtext, LogType.PrivateChat);
                    return;
                }
                if ((text[0] == '#') || (isOpChat)) // no overide check is needed here because the "@" whisper check occurs before this code.
                {
                    string newtext = text;
                    if (!isOpChat)
                    {
                        newtext = text.Remove(0, 1).Trim();
                    }
                    GlobalMessageOps("To Ops &f-" + color + name + "&f- " + newtext);
                    if (!checkOp())
                        SendMessage("To Ops &f-" + color + name + "&f- " + newtext);

                    Logger.Log("<" + name + "> " + newtext, LogType.OpChat);
                    return;
                }
                if (text[0] == '%')
                {
                    string newtext = text.Remove(0, 1).Trim();
                    if (!Properties.AllowWorldChat)
                    {
                        GlobalChatWorld(this, newtext, true);
                        Logger.Log("<" + name + "> " + newtext, LogType.WorldChat);
                    }
                    else
                    {
                        GlobalChat(this, newtext);
                        Logger.Log("<" + name + "> " + newtext, LogType.GlobalChat);
                    }

                    IRCBot.Say("<" + name + "> " + newtext);
                    return;
                }


                if (Properties.AllowWorldChat)
                {
                    GlobalChat(this, text);
                    Logger.Log("<" + name + "> " + text, LogType.WorldChat);
                }
                else
                {
                    GlobalChatLevel(this, text, true);
                    Logger.Log("<" + name + "> " + text, LogType.GlobalChat);
                }

                IRCBot.Say(name + ": " + text);
            }
            catch (Exception e)
            {
                Logger.Log("There was an error with chat.", LogType.Error);
                Logger.Log(e.Message, LogType.ErrorMessage);
            }
        }

        /// <summary>
        /// Handles private chat between players
        /// </summary>
        /// <param name="to">The player name we are sending a message to</param>
        /// <param name="message">The message to send to the player</param>
        void HandleQuery (string to, string message)
        {
            // Find the player to send it too
            Player p = Find(to);

            // Make sure we're not trying to send to ourselves
            if (p != this)
            {
                // Make sure we actually found the player and that the player isn't hiding from the world
                if (p != null && !p.hidden)
                {
                    Logger.Log(name + " @" + p.name + ": " + message, LogType.PrivateChat);
                    p.SendChat(this, "&e[<] " + color + name + ": &f" + message);
                    SendChat(this, "&9[>] " + p.color + p.name + ": &f" + message);
                }
                else
                {
                    SendMessage("Player \"" + to + "\" doesn't exist!");
                }
            }
            else
            {
                SendMessage("Trying to talk to yourself, huh?");

            }
        }


        #region == GLOBAL MESSAGES ==

        public static void GlobalChat (Player from, string message)
        {
            GlobalChat(from, message, true);
        }
        public static void GlobalChat (Player from, string message, bool showname)
        {
            if (showname) { message = from.color + from.name + ": &f" + message; }
            players.ForEach(delegate(Player p) { p.SendChat(from, message); });
        }
        public static void GlobalChatLevel (Player from, string message, bool showname)
        {
            if (showname) { message = "<Level>" + from.color + from.name + ": &f" + message; }
            players.ForEach(delegate(Player p) { if (p.level == from.level)p.SendChat(from, message); });
        }
        public static void GlobalChatWorld (Player from, string message, bool showname)
        {
            if (showname) { message = "<World>" + from.color + from.name + ": &f" + message; }
            players.ForEach(delegate(Player p) { p.SendChat(from, message); });
        }
        public static void GlobalMessage (string message)
        {
            players.ForEach(delegate(Player p) { p.SendMessage(message); });
        }
        public static void GlobalMessageLevel (Level l, string message)
        {
            players.ForEach(delegate(Player p) { if (p.level == l) p.SendMessage(message); });
        }
        public static void GlobalMessageOps (string message)     //Send a global messege to ops only
        {
            players.ForEach(delegate(Player p)
            {
                if (p.Rank >= GroupEnum.Moderator)
                {
                    p.SendMessage(message);
                }
            });
        }

        #endregion

    }
}
