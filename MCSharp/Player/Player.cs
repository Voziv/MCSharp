using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;


namespace MCSharp
{
    public  sealed partial class Player
    {
        // Static Variables
        private static List<string> developers = new List<string>(new string[] { "voziv" });
        private static List<string> supporters = new List<string>(new string[] { "howimineforfish", "merlin33069", "descention", "kakashisuno" });

        public static List<Player> players = new List<Player>(64);
        public static Dictionary<string, string> left = new Dictionary<string, string>();
        public static List<Player> connections = new List<Player>(Properties.MaxPlayers);
        public static byte number { get { return (byte)players.Count; } }
        static System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
        static MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

        // Properties
        public GroupEnum Rank { get { return Player.GetRank(name); } }
        public string color { get { return _color; } set { _color = value;  } }
        private string _color;

        // Variables
        Socket socket;
        System.Timers.Timer loginTimer = new System.Timers.Timer(20000);
        System.Timers.Timer pingTimer = new System.Timers.Timer(500);
        byte[] buffer = new byte[0];
        byte[] tempbuffer = new byte[0xFF];
        public bool disconnected = false;
        public BlockBuffer copyBuffer = new BlockBuffer();
        public UndoBuffer undoPasteBuffer = new UndoBuffer();

        // Bman Additions
        Random rand = new Random();
        DateTime lastJoke = new DateTime();
        public bool isJailed = false;
        public bool isMuted = false;
        public bool isJoker = false;
        public int deletedBlocks = 0;
        public int placedBlocks = 0;
        public static Dictionary<string, DateTime> lastSeen = new Dictionary<string, DateTime>();

        public string name;
        public string ip;
        public byte id;
        public Group group;
        public bool hidden = false;
        public bool painting = false;
        public byte BlockAction = 0;  //0-Nothing 1-solid 2-lava 3-water 4-active_lava 5 Active_water 6 OpGlass
        //public List<Edit> actions = new List<Edit>(128);
        public byte[] bindings = new byte[128];
        public Level level = Server.mainLevel;
        public bool Loading = true;     //True if player is loading a map.

        public delegate void BlockchangeEventHandler(Player p, ushort x, ushort y, ushort z, byte type);
        public event BlockchangeEventHandler Blockchange = null;
        public void ClearBlockchange() { Blockchange = null; }
        public bool HasBlockchange() { return (Blockchange == null); }
        public object blockchangeObject = null;

        public ushort[] pos = new ushort[3] { 0, 0, 0 };
        ushort[] oldpos = new ushort[3] { 0, 0, 0 };
        ushort[] basepos = new ushort[3] { 0, 0, 0 };
        public byte[] rot = new byte[2] { 0, 0 };
        byte[] oldrot = new byte[2] { 0, 0 };

        // grief/spam detection
        public static int spamBlockCount = 55;
        public static int spamBlockTimer = 5;
        Queue<DateTime> spamBlockLog = new Queue<DateTime>(spamBlockCount);

        public static int spamChatCount = 3;
        public static int spamChatTimer = 4;
        Queue<DateTime> spamChatLog = new Queue<DateTime>(spamChatCount);

        bool loggedIn = false;

        public Block doors = new Block();
        public bool isOpChat = false;
        public bool isWhisperChat = false;
        public string whisperTarget = "";

        public Player(Socket s)
        {
            try
            {
                socket = s;
                ip = socket.RemoteEndPoint.ToString().Split(':')[0];
                Logger.Log(ip + " connected.", LogType.Information);

                if (Server.bannedIP.Contains(ip)) { Kick("You're banned!"); return; }
                if (connections.Count >= 5) { Kick("Too many connections!"); return; }

                for (byte i = 0; i < 128; ++i)
                    bindings[i] = i;

                socket.BeginReceive(tempbuffer, 0, tempbuffer.Length, SocketFlags.None,
                                    new AsyncCallback(Receive), this);

                loginTimer.Elapsed += delegate
                {
                    loginTimer.Stop();
                    if (!loggedIn)
                    {
                        Kick("You must login! Try again.");
                    }
                    else if (Rank >= GroupEnum.Operator)
                    {
                        SendMessage("Welcome " + name + "! You rule!");
                        if (Server.LatestVersion > Server.VersionNumber)
                        {
                            SendMessage("[Console]: &cImportant!!! A MCSharp update is available!!");
                        }
                    }
                    else
                    {
                        SendMessage("Welcome " + name + "! Please use /rules");
                    }
                    _color = MCSharp.Group.Find(Rank).Color;
                    if (File.Exists("welcome.txt"))
                    {
                        try
                        {
                            List<string> Welcome = new List<string>();
                            StreamReader wm = File.OpenText("welcome.txt");
                            while (!wm.EndOfStream)
                            {
                                Welcome.Add(wm.ReadLine());
                            }
                            wm.Close();

                            foreach (string w in Welcome)
                                SendMessage(w);
                        }
                        catch
                        {

                        }
                    }
                }; loginTimer.Start();
                pingTimer.Elapsed += delegate { SendPing(); };
                pingTimer.Start();
                connections.Add(this);

            }
            catch (Exception e)
            {
                Logger.Log("Error while in the player constructor", LogType.Error);
                Logger.Log(e.Message, LogType.ErrorMessage);
            }
        }

        public void ChangeLevel(string strLevel)
        {
            foreach (Level level in Server.levels)
            {
                if (level.name.ToLower() == strLevel.ToLower())
                {
                    ChangeLevel(level);
                    break;
                }
            }
        }

        public void ChangeLevel(Level lvlLevel)
        {
            Loading = true;

            // Clear the players player list
            foreach (Player pl in Player.players)
            {
                if (this.level == pl.level && this != pl)
                {
                    this.SendDie(pl.id);
                }
            }

            // Clear out bots from the list too
            foreach (PlayerBot bot in PlayerBot.playerbots)
            {
                if (this.level == bot.level)
                {
                    this.SendDie(bot.id);
                }       //Kills current bot list for player
            }

            this.ClearBlockchange();
            this.BlockAction = 0;
            this.painting = false;
            Player.GlobalDie(this, true);
            this.level = lvlLevel;
            this.SendMotd();
            this.SendMap();
            ushort x = (ushort)((0.5 + level.spawnx) * 32);
            ushort y = (ushort)((1 + level.spawny) * 32);
            ushort z = (ushort)((0.5 + level.spawnz) * 32);
            if (!this.hidden)
            {
                Player.GlobalSpawn(this, x, y, z, level.rotx, level.roty, true);
            }
            else unchecked
                {
                    this.SendPos((byte)-1, x, y, z, level.rotx, level.roty);
                }
            foreach (Player pl in Player.players)
            {
                if (this.level == pl.level && this != pl && !pl.hidden)
                {
                    this.SendSpawn(pl.id, pl.color + pl.name, pl.pos[0], pl.pos[1], pl.pos[2], pl.rot[0], pl.rot[1]);
                }
            }
            foreach (PlayerBot bot in PlayerBot.playerbots)   //Send bots to the player
            {
                if (bot.level == this.level)
                {
                    this.SendSpawn(bot.id, bot.color + bot.name, bot.pos[0], bot.pos[1], bot.pos[2], bot.rot[0], bot.rot[1]);
                }
            }
            this.Loading = false;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void ClearActions()
        {
            ClearBlockchange();
            ClearBindings();
            painting = false;
        }

        public int ClearBindings()
        {
            int bindCount = 0;
            for (byte i = 0; i < 128; ++i)
            {
                if (Block.Placable(i) && bindings[i] != i)
                {
                    bindings[i] = i;
                    bindCount += 1;
                }
            }
            return bindCount;
        }

        public static GroupEnum GetRank(string name)
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

        public static void ChangeRank(Player p, GroupEnum g)
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

        public static void ChangeRank(string name, GroupEnum g)
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

        public static void Ban(string name)
        {
            Player.ChangeRank(name, GroupEnum.Banned);
            Logger.Log("BANNED: " + name.ToLower());
        }

        public static bool IsOnline(string name)
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


        #region == GLOBAL MESSAGES ==

        public static void GlobalBlockchange(Level level, ushort x, ushort y, ushort z, byte type)
        {
            players.ForEach(delegate(Player p) { if (p.level == level) { p.SendBlockchange(x, y, z, type); } });
        }
        public static void GlobalChat(Player from, string message) { GlobalChat(from, message, true); }
        public static void GlobalChat(Player from, string message, bool showname)
        {
            if (showname) { message = from.color + from.name + ": &f" + message; }
            players.ForEach(delegate(Player p) { p.SendChat(from, message); });
        }
        public static void GlobalChatLevel(Player from, string message, bool showname)
        {
            if (showname) { message = "<Level>" + from.color + from.name + ": &f" + message; }
            players.ForEach(delegate(Player p) { if (p.level == from.level)p.SendChat(from, message); });
        }
        public static void GlobalChatWorld(Player from, string message, bool showname)
        {
            if (showname) { message = "<World>" + from.color + from.name + ": &f" + message; }
            players.ForEach(delegate(Player p) { p.SendChat(from, message); });
        }
        public static void GlobalMessage(string message)
        {
            players.ForEach(delegate(Player p) { p.SendMessage(message); });
        }
        public static void GlobalMessageLevel(Level l, string message)
        {
            players.ForEach(delegate(Player p) { if (p.level == l) p.SendMessage(message); });
        }
        public static void GlobalMessageOps(string message)     //Send a global messege to ops only
        {
            players.ForEach(delegate(Player p)
            {
                if (p.Rank >= GroupEnum.Moderator)
                {
                    p.SendMessage(message);
                }
            });
        }

        public static void GlobalRespawn(Player respawnTarget)
        {
            GlobalSpawn(respawnTarget, respawnTarget.pos[0], respawnTarget.pos[1], respawnTarget.pos[2], respawnTarget.rot[0], respawnTarget.rot[1], false);
        }


        public static void GlobalSpawn(Player from, ushort x, ushort y, ushort z, byte rotx, byte roty, bool self)
        {
            players.ForEach(delegate(Player p)
            {
                if (p.Loading && p != from) { return; }
                if (p.level != from.level || (from.hidden && !self)) { return; }
                if (p != from) { p.SendSpawn(from.id, from.color + from.name, x, y, z, rotx, roty); }
                else if (self)
                {
                    p.pos = new ushort[3] { x, y, z }; p.rot = new byte[2] { rotx, roty };
                    p.oldpos = p.pos; p.basepos = p.pos; p.oldrot = p.rot;
                    unchecked { p.SendSpawn((byte)-1, from.color + from.name, x, y, z, rotx, roty); }
                }
            });
        }
        public static void GlobalDie(Player from, bool self)
        {
            players.ForEach(delegate(Player p)
            {
                if (p.level != from.level || (from.hidden && !self)) { return; }
                if (p != from) { p.SendDie(from.id); }
                else if (self) { unchecked { p.SendDie((byte)-1); } }
            });
        }
        public static void GlobalUpdate() { players.ForEach(delegate(Player p) { if (!p.hidden) { p.UpdatePosition(); } }); }

        #endregion


        #region == DISCONNECTING ==

        public void Disconnect()
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

        public void Kick(string message)
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

        public void SMPKick(string message)
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

        public static List<Player> GetPlayers() { return new List<Player>(players); }
        public static bool Exists(string name)
        {
            foreach (Player p in players)
            { if (p.name.ToLower() == name.ToLower()) { return true; } } return false;
        }
        public static bool Exists(byte id)
        {
            foreach (Player p in players)
            { if (p.id == id) { return true; } } return false;
        }
        public static Player Find(string name)
        {
            foreach (Player p in players)
            { if (p.name.ToLower() == name.ToLower()) { return p; } } return null;
        }
        public static Group GetGroup(string name)
        {
            Player who = Player.Find(name); if (who != null) { return who.group; }
            if (Server.banned.All().Contains(name.ToLower())) { return Group.Find("banned"); }
            if (Server.operators.All().Contains(name.ToLower())) { return Group.Find("operator"); }
            return Group.standard;
        }
        public static string GetColor(string name) { return GetGroup(name).Color; }

        #endregion


        #region == OTHER ==

        static byte FreeId()
        {
            for (byte i = 0; i < Properties.MaxPlayers; ++i)
            {
                foreach (Player p in players)
                {
                    if (p.id == i) { goto Next; }
                } return i;
            Next: continue;
            } unchecked { return (byte)-1; }
        }
        static byte[] StringFormat(string str, int size)
        {
            byte[] bytes = new byte[size];
            bytes = enc.GetBytes(str.PadRight(size).Substring(0, size));
            return bytes;
        }
        static List<string> Wordwrap(string message)
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
        public static bool ValidName(string name)
        {
            string allowedchars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz01234567890._";
            foreach (char ch in name) { if (allowedchars.IndexOf(ch) == -1) { return false; } } return true;
        }
        public static byte[] GZip(byte[] bytes)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            GZipStream gs = new GZipStream(ms, CompressionMode.Compress, true);
            gs.Write(bytes, 0, bytes.Length);
            gs.Close();
            ms.Position = 0;
            bytes = new byte[ms.Length];
            ms.Read(bytes, 0, (int)ms.Length);
            ms.Close();
            return bytes;
        }

        #endregion



        public static bool checkDev(Player p)
        {
            return checkDev(p.name);
        }

        public static bool checkDev(string strName)
        {
            bool blnIsDeveloper = false;
            foreach (string developer in developers)
            {
                if (strName == developer)
                {
                    blnIsDeveloper = true;
                    break;
                }
            }
            return blnIsDeveloper;
        }

        public static bool checkSupporter(Player p)
        {
            return checkSupporter(p.name);
        }
        public static bool checkSupporter(string strName)
        {
            bool blnIsSupporter = false;
            foreach (string supporter in supporters)
            {
                if (strName == supporter)
                {
                    blnIsSupporter = true;
                    break;
                }
            }
            return blnIsSupporter;
        }


        bool CheckBlockSpam()
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