using System;
using System.Collections.Generic;

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
