using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;


namespace MCSharp
{
    public sealed partial class Player
    {
        public static List<Player> players = new List<Player>(64);

        /// <summary>
        /// This needs a better description. What does left do?
        /// </summary>
        public static Dictionary<string, string> left = new Dictionary<string, string>();

        public static byte number { get { return (byte) players.Count; } }

        // Properties
        public GroupEnum Rank { get { return Player.GetRank(name); } }
        public string color { get { return _color; } set { _color = value; } }
        private string _color;

        // Bman Additions
        Random rand = new Random();
        DateTime lastJoke = new DateTime();
        public bool isJailed = false;
        public bool isMuted = false;
        public bool isJoker = false;

        /// <summary>
        /// This keeps track of when a player was last seen. Resets when the server is reset.
        /// </summary>
        public static Dictionary<string, DateTime> lastSeen = new Dictionary<string, DateTime>();

        public string name;

        public byte id;
        public Group group;
        public bool hidden = false;

        public byte[] bindings = new byte[128];

        

        public bool Loading = true;     //True if player is loading a map.

        bool loggedIn = false;

        public Player (Socket s)
        {
            initNetworking(s);

            // Set up bindings
            for (byte i = 0; i < 128; ++i)
                bindings[i] = i;
        }
             

        

        public static GroupEnum GetRank (string name)
        {
            GroupEnum g = GroupEnum.Guest;

            if (Properties.ServerAdministrator == name)
                g = GroupEnum.Administrator;
            else if (Server.banned.Contains(name))
                g = GroupEnum.Banned;
            else if (Server.builders.Contains(name))
                g = GroupEnum.Builder;
            else if (Server.advbuilders.Contains(name))
                g = GroupEnum.AdvBuilder;
            else if (Server.moderators.Contains(name))
                g = GroupEnum.Moderator;
            else if (Server.operators.Contains(name))
                g = GroupEnum.Operator;

            return g;
        }

        public static void ChangeRank (Player p, GroupEnum g)
        {
            bool blnResendMap = ((p.checkOp() && g <= GroupEnum.Moderator) || (!p.checkOp() && g >= GroupEnum.Moderator));
            p.group = Group.Find(g);
            p.ClearActions();
            ChangeRank(p.name, g);

            if (blnResendMap)
            {
                p.ChangeLevel(p.level);
            }
            else
            {
                GlobalDie(p, false);
                GlobalRespawn(p);
            }
        }

        public static void ChangeRank (string name, GroupEnum g)
        {

            if (Server.operators.Contains(name)) { Server.operators.Remove(name); }
            if (Server.moderators.Contains(name)) { Server.moderators.Remove(name); }
            if (Server.advbuilders.Contains(name)) { Server.advbuilders.Remove(name); }
            if (Server.builders.Contains(name)) { Server.builders.Remove(name); }

            // Debug Message        

            switch (g)
            {
                case GroupEnum.Banned:
                    Server.banned.Add(name);
                    break;
                case GroupEnum.Guest:
                    break;
                case GroupEnum.Builder:
                    Server.builders.Add(name);
                    break;
                case GroupEnum.AdvBuilder:
                    Server.advbuilders.Add(name);
                    break;
                case GroupEnum.Moderator:
                    Server.moderators.Add(name);
                    break;
                case GroupEnum.Operator:
                    Server.operators.Add(name);
                    break;
            }

            if (Player.IsOnline(name))
            {
                Player target = Player.Find(name);
                Player.GlobalDie(target, false);
                Player.GlobalSpawn(target, target.pos[0], target.pos[1], target.pos[2], target.rot[0], target.rot[1], false);
            }
        }


        public static void Ban (string name)
        {
            Player.ChangeRank(name, GroupEnum.Banned);
            Logger.Log("BANNED: " + name.ToLower());
        }


        public static bool IsOnline (string name)
        {
            bool blnOnline = false;
            foreach (Player p in players)
            {
                if (p.name.ToLower() == name.ToLower())
                {
                    blnOnline = true;
                    break;
                }
            }
            return blnOnline;
        }



        #region == DISCONNECTING ==

        public void Disconnect ()
        {
            if (disconnected)
            {
                if (connections.Contains(this))
                    connections.Remove(this);
                return;
            }
            disconnected = true;
            pingTimer.Stop();
            SendKick("Disconnected.");
            if (loggedIn)
            {
                GlobalDie(this, false);
                if (!hidden) { GlobalChat(this, "&c- " + color + name + "&e disconnected.", false); }
                IRCBot.Say(name + " left the game.");
                Logger.Log(name + " disconnected.");
                players.Remove(this);
                Server.s.PlayerListUpdate();
                /*if (!Server.console && Server.win != null)
                    Server.win.UpdateClientList(players);*/
                left.Add(this.name.ToLower(), this.ip);
                //Added by bman for lastseen command
                if (!lastSeen.ContainsKey(this.name.ToLower()))
                {
                    lastSeen.Add(this.name.ToLower(), DateTime.Now);
                    Server.SaveLastSeen();
                }
                else
                {
                    lastSeen[this.name.ToLower()] = DateTime.Now;
                    Server.SaveLastSeen();
                }
            }
            else
            {
                connections.Remove(this);
                Logger.Log(ip + " disconnected.");
            }
            if (Server.afkset.Contains(name))
            {
                Server.afkset.Remove(name);
            }
            //Removes from afk list on disconnect
        }

        public void Kick (string message)
        {
            if (disconnected)
            {
                if (connections.Contains(this))
                    connections.Remove(this);
                return;
            }
            disconnected = true;
            pingTimer.Stop();
            SendKick(message);
            if (loggedIn)
            {
                GlobalDie(this, false);
                GlobalChat(this, "&c- " + color + name + "&e kicked (" + message + ").", false);
                Logger.Log(name + " was kicked. (" + message + ").");
                players.Remove(this);
                Server.s.PlayerListUpdate();
                left.Add(this.name.ToLower(), this.ip);
            }
            else
            {
                connections.Remove(this);
                Logger.Log(ip + " was kicked (" + message + ").");
            }
            if (Server.afkset.Contains(name))
            {
                Server.afkset.Remove(name);
            }//Removes from afk list on disconnect
        }

        public void SMPKick (string message)
        {
            if (disconnected)
            {
                if (connections.Contains(this))
                    connections.Remove(this);
                return;
            }
            disconnected = true;
            pingTimer.Stop();



            // Send Kick
            // Get the bytes in UTF16
            byte[] messageBytes = Encoding.BigEndianUnicode.GetBytes(message);

            // Make a new array to hold the message and the length prefix
            byte[] bytes = new byte[messageBytes.Length + 2];

            // Get the length of the string
            byte[] length = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short) message.Length));

            // Merge the arrays
            Buffer.BlockCopy(length, 0, bytes, 0, length.Length);
            Buffer.BlockCopy(messageBytes, 0, bytes, 2, messageBytes.Length);

            // Send the Kick packet
            SendRaw(255, bytes);

            if (connections.Contains(this))
                connections.Remove(this);
            if (loggedIn)
            {
                GlobalDie(this, false);
                GlobalChat(this, "&c- " + color + name + "&e kicked (" + message + ").", false);
                Logger.Log(name + " was kicked. (" + message + ").");
                players.Remove(this);
                Server.s.PlayerListUpdate();
                left.Add(this.name.ToLower(), this.ip);
            }
            else
            {
                connections.Remove(this);
                Logger.Log(ip + " was kicked (" + message + ").");
            }
            if (Server.afkset.Contains(name))
            {
                Server.afkset.Remove(name);
            }//Removes from afk list on disconnect
        }

        #endregion


        #region == CHECKING ==

        public static List<Player> GetPlayers () { return new List<Player>(players); }
        public static bool Exists (string name)
        {
            foreach (Player p in players)
            { if (p.name.ToLower() == name.ToLower()) { return true; } } return false;
        }
        public static bool Exists (byte id)
        {
            foreach (Player p in players)
            { if (p.id == id) { return true; } } return false;
        }
        public static Player Find (string name)
        {
            foreach (Player p in players)
            { if (p.name.ToLower() == name.ToLower()) { return p; } } return null;
        }
        public static Group GetGroup (string name)
        {
            Player who = Player.Find(name); if (who != null) { return who.group; }
            if (Server.banned.All().Contains(name.ToLower())) { return Group.Find("banned"); }
            if (Server.operators.All().Contains(name.ToLower())) { return Group.Find("operator"); }
            return Group.standard;
        }
        public static string GetColor (string name) { return GetGroup(name).Color; }

        #endregion


        #region == OTHER ==

        static byte FreeId ()
        {
            byte freeId = 0;
            bool isFree = false;
            for (freeId = 0; freeId < Properties.MaxPlayers; ++freeId)
            {
                isFree = true;
                foreach (Player p in players)
                {
                    if (p.id == freeId)
                    {
                        isFree = false;
                        break;
                    }
                }

                if (isFree)
                {
                    break;
                }
            }

            if (isFree)
            {
                return freeId;
            }
            else
            {
                unchecked { return (byte) -1; }
            }
        }

        static byte[] StringFormat (string str, int size)
        {
            byte[] bytes = new byte[size];
            bytes = Encoding.ASCII.GetBytes(str.PadRight(size).Substring(0, size));

            return bytes;
        }
        static List<string> Wordwrap (string message)
        {
            List<string> lines = new List<string>();
            message = Regex.Replace(message, @"(&[0-9a-f])+(&[0-9a-f])", "$2");
            message = Regex.Replace(message, @"(&[0-9a-f])+$", "");
            int limit = 64; string color = "";
            while (message.Length > 0)
            {
                if (lines.Count > 0) { message = "> " + color + message.Trim(); }
                if (message.Length <= limit) { lines.Add(message); break; }
                for (int i = limit - 1; i > limit - 9; --i)
                {
                    if (message[i] == ' ')
                    {
                        lines.Add(message.Substring(0, i)); goto Next;
                    }
                }
                lines.Add(message.Substring(0, limit));
Next: message = message.Substring(lines[lines.Count - 1].Length);
                if (lines.Count == 1)
                {
                    limit = 60;
                }
                int index = lines[lines.Count - 1].LastIndexOf('&');
                if (index != -1)
                {
                    if (index < lines[lines.Count - 1].Length - 1)
                    {
                        char next = lines[lines.Count - 1][index + 1];
                        if ("0123456789abcdef".IndexOf(next) != -1) { color = "&" + next; }
                        if (index == lines[lines.Count - 1].Length - 1)
                        {
                            lines[lines.Count - 1] = lines[lines.Count - 1].
                                Substring(0, lines[lines.Count - 1].Length - 2);
                        }
                    }
                    else if (message.Length != 0)
                    {
                        char next = message[0];
                        if ("0123456789abcdef".IndexOf(next) != -1)
                        {
                            color = "&" + next;
                        }
                        lines[lines.Count - 1] = lines[lines.Count - 1].
                            Substring(0, lines[lines.Count - 1].Length - 1);
                        message = message.Substring(1);
                    }
                }
            } return lines;
        }
        public static bool ValidName (string name)
        {
            string allowedchars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz01234567890._";
            foreach (char ch in name) { if (allowedchars.IndexOf(ch) == -1) { return false; } } return true;
        }
        public static byte[] GZip (byte[] bytes)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            GZipStream gs = new GZipStream(ms, CompressionMode.Compress, true);
            gs.Write(bytes, 0, bytes.Length);
            gs.Close();
            ms.Position = 0;
            bytes = new byte[ms.Length];
            ms.Read(bytes, 0, (int) ms.Length);
            ms.Close();
            return bytes;
        }

        #endregion


        bool CheckBlockSpam ()
        {
            if (!Server.griefExempted.Contains(name))
            {
                if (spamBlockLog.Count >= spamBlockCount)
                {
                    DateTime oldestTime = spamBlockLog.Dequeue();
                    double spamTimer = DateTime.Now.Subtract(oldestTime).TotalSeconds;
                    if (spamTimer < spamBlockTimer)
                    {
                        this.Kick("You were kicked by antigrief system. Slow down.");
                        SendMessage(c.red + name + " was kicked for suspected griefing.");
                        Logger.Log(name + " was kicked for block spam (" + spamBlockCount + " blocks in " + spamTimer + " seconds)");
                        return true;
                    }
                }
            }
            else
            {
                spamBlockLog.Clear(); // Should increase performance. If you clear the log then this code should get called less
            }
            spamBlockLog.Enqueue(DateTime.Now);
            return false;
        }
    }
}