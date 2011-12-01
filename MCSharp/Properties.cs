using System;
using System.IO;
using System.Xml.Serialization;

namespace MCSharp
{
    public static class Properties
    {
        
        public static string ServerAdministrator = "";
        public static string ServerName = "A new MCSharp Server is born";
        public static string ServerMOTD = "Welcome to my MCSharp server!";
        public static byte MaxPlayers = 12;
        public static byte MaxMaps = 5;
        public static int ServerPort = 25565;
        public static bool PublicServer = true;
        public static bool VerifyNames = false;
        public static bool AllowWorldChat = true;
        public static bool GuestGoto = false;
        public static bool DebugEnabled = false;

        public static bool BetaMode = false;

        public static string MainLevel = "main";

        public static bool AutoUnloadEnabled = true;
        


        public static bool AntiTunnelEnabled = true;
        public static byte MaxDepth = 4;
        public static int PhysicsOverload = 7500;
        public static int BackupInterval = 60;

        public static Int32 advBuilderCuboidLimit = 10000;
        public static Int32 moderatorCuboidLimit = 25000;
        public static Int32 operatorCuboidLimit = 50000;
        public static Int32 administratorCuboidLimit = 0;


        // IRC Section
        public static bool IRCEnabled = false;
        public static int IRCPort = 6667;
        public static string IRCNick = "MCsharp";
        public static string IRCServer = "irc.example.com";
        public static string IRCChannel = "#changethis";
        public static bool IRCIdentify = false;
        public static string IRCPassword = "";


        public static void Load()
        {
            string rndchars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random rnd = new Random();
            for (int i = 0; i < 16; ++i) { Server.salt += rndchars[rnd.Next(rndchars.Length)]; }

            if (File.Exists("server.properties"))
            {
                string[] lines = File.ReadAllLines("server.properties");

                foreach (string line in lines)
                {
                    if (line != "" && line[0] != '#')
                    {
                        //int index = line.IndexOf('=') + 1; // not needed if we use Split('=')
                        string key = line.Split('=')[0].Trim();
                        string value = line.Split('=')[1].Trim();

                        switch (key.ToLower())
                        {
                            case "beta-mode":
                                BetaMode = (value.ToLower() == "true") ? true : false;
                                if (BetaMode)
                                {
                                    Logger.Log("WARNING: BETA MODE IS ENABLED!!!", LogType.Warning);
                                    Logger.Log("BETAMODE: Some functions may be unstable/destructive. Make sure you have backups!", LogType.Warning);
                                }
                                break;
                            case "debug-mode":
                                DebugEnabled = (value.ToLower() == "true") ? true : false;
                                break;
                            case "server-administrator":
                                if (ValidString(value, "![]:.,{}~-+()?_/\\ "))
                                {
                                    ServerAdministrator = value;
                                }
                                else
                                {
                                    Logger.Log("Administrator name is invalid! Setting back to nobody.", LogType.Error);
                                }
                                break;
                            case "server-name":
                                if (ValidString(value, "![]:.,{}~-+()?_/\\ "))
                                {
                                    ServerName = value;
                                }
                                else
                                {
                                    Logger.Log("server-name is invalid! Setting it back to default.", LogType.Error);
                                }
                                break;
                            case "motd":
                                if (ValidString(value, "![]&:.,{}~-+()?_/\\ "))
                                {
                                    ServerMOTD = value;
                                }
                                else
                                {
                                    Logger.Log("motd is invalid! Setting it back to default.", LogType.Error);
                                }
                                break;
                            case "port":
                                try
                                {
                                    ServerPort = Convert.ToInt32(value);
                                }
                                catch
                                {
                                    Logger.Log("port is invalid! Setting it back to default.", LogType.Error);
                                }
                                break;
                            case "verify-names":
                                VerifyNames = (value.ToLower() == "true") ? true : false;
                                break;
                            case "public":
                                PublicServer = (value.ToLower() == "true") ? true : false;
                                break;
                            case "world-chat":
                                AllowWorldChat = (value.ToLower() == "true") ? true : false;
                                break;
                            case "guest-goto":
                                GuestGoto = (value.ToLower() == "true") ? true : false;
                                break;
                            case "max-players":
                                try
                                {
                                    if (Convert.ToByte(value) > 64)
                                    {
                                        value = "64";
                                        Logger.Log("Max players has been lowered to 64.");
                                    }
                                    else if (Convert.ToByte(value) < 1)
                                    {
                                        value = "1";
                                        Logger.Log("Max players has been increased to 1.");
                                    }
                                    MaxPlayers = Convert.ToByte(value);
                                }
                                catch
                                {
                                    Logger.Log("max-players is invalid! Setting it back to default.", LogType.Error);
                                }
                                break;
                            case "max-maps":
                                try
                                {
                                    if (Convert.ToByte(value) > 200)
                                    {
                                        value = "200";
                                        Logger.Log("Max maps has been lowered to 200.");
                                    }
                                    else if (Convert.ToByte(value) < 1)
                                    {
                                        value = "1";
                                        Logger.Log("Max maps has been increased to 1.");
                                    }
                                    MaxMaps = Convert.ToByte(value);
                                }
                                catch
                                {
                                    Logger.Log("max-maps is invalid! Setting it back to default.");
                                }
                                break;
                            case "advbuildercuboidlimit":
                                advBuilderCuboidLimit = Convert.ToInt32(value);
                                break;
                            case "moderatorcuboidlimit":
                                moderatorCuboidLimit = Convert.ToInt32(value);
                                break;
                            case "operatorcuboidlimit":
                                operatorCuboidLimit = Convert.ToInt32(value);
                                break;
                            case "auto-unload":
                                AutoUnloadEnabled = (value.ToLower() == "true") ? true : false;
                                break;
                            case "irc":
                                IRCEnabled = (value.ToLower() == "true") ? true : false;
                                break;
                            case "irc-server":
                                IRCServer = value;
                                break;
                            case "irc-nick":
                                IRCNick = value;
                                break;
                            case "irc-channel":
                                    switch (value.ToLower().Trim())
                                    {
                                        case "#mcsharp":
                                        case "#crafted":
                                        case "#mcc":
                                        case "#minecraft":
                                            value = "#changeme";
                                            break;
                                    }
                                IRCChannel = value;
                                break;
                            case "irc-port":
                                try
                                {
                                    IRCPort = Convert.ToInt32(value);
                                }
                                catch
                                {
                                    Logger.Log("irc-port is invalid! Setting it back to default.", LogType.Error);
                                }
                                break;
                            case "irc-identify":
                                try
                                {
                                    IRCIdentify = Convert.ToBoolean(value);
                                }
                                catch
                                {
                                    Logger.Log("irc-identify boolean is invalid! Setting it back to default.", LogType.Error);
                                }
                                break;
                            case "irc-password":
                                IRCPassword = value;
                                break;
                            case "anti-tunnels":
                                AntiTunnelEnabled = (value.ToLower() == "true") ? true : false;
                                break;
                            case "max-depth":
                                try
                                {
                                    MaxDepth = Convert.ToByte(value);
                                }
                                catch
                                {
                                    Logger.Log("maxDepth is invalid! Setting it back to default.", LogType.Error);
                                }
                                break;

                            case "overload":
                                try
                                {
                                    if (Convert.ToInt16(value) > 10000)
                                    {
                                        value = "10000";
                                        Logger.Log("Max overload is 10000.");
                                    }
                                    else if (Convert.ToInt16(value) < 2000)
                                    {
                                        value = "2000";
                                        Logger.Log("Min overload is 2000");
                                    }
                                    PhysicsOverload = Convert.ToInt16(value);
                                }
                                catch
                                {
                                    Logger.Log("overload is invalid! Setting it back to default.", LogType.Error);
                                }
                                break;
                            case "backup-time":
                                if (Convert.ToInt32(value)>1)
                                {
                                    BackupInterval = Convert.ToInt32(value);
                                }
                                break;
                        }
                    }
                }
                Logger.Log("LOADED: server.properties");
                Server.s.SettingsUpdate();
                Save();
            }
            else
                Save();
        }

        public static bool ValidString(string str)
        {
            return ValidString(str, "");
        }

        public static bool ValidString(string str, string allowed)
        {
            bool blnValid = true;
            string allowedchars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz01234567890" + allowed;
            foreach (char ch in str)
            {
                if (allowedchars.IndexOf(ch) == -1)
                {
                    blnValid = false;
                    break;
                }
            } 
            return blnValid;
        }

        public static bool ValidName(string name)
        {
            bool validName = false;
            if (name.Length <= 16)
            {
                if (ValidString(name))
                {
                    validName = true;
                }
            }
            return validName;
        }

        static void Save()
        {
            try
            {
                StreamWriter w = new StreamWriter(File.Create("server.properties"));
                w.WriteLine("# Edit the settings below to modify how your server operates. This is an explanation of what each setting does.");
                w.WriteLine("#   server-name\t=\tThe name which displays on minecraft.net.");
                w.WriteLine("#   server-administrator\t=\tThe name of the player who will be the administrator of the server.");
                w.WriteLine("#   motd\t=\tThe message which displays when a player connects.");
                w.WriteLine("#   port\t=\tThe port to operate from.");
                w.WriteLine("#   verify-names\t=\tVerify the validity of names.");
                w.WriteLine("#   public\t=\tSet to true to appear in the public server list.");
                w.WriteLine("#   max-players\t=\tThe maximum number of connections.");
                w.WriteLine("#   max-maps\t=\tThe maximum number of maps loaded at once.");
                w.WriteLine("#   world-chat\t=\tSet to true to enable world chat");
                w.WriteLine("#   guest-goto\t=\tSet to true to give guests goto and levels commands.");
                w.WriteLine("#   bannerurl\t=\tPath to your banner.");
                w.WriteLine("#");
                w.WriteLine("#   irc\t=\tSet to true to enable the IRC bot");
                w.WriteLine("#   irc-nick\t=\tThe name of the IRC bot");
                w.WriteLine("#   irc-server\t=\tThe server to connect to");
                w.WriteLine("#   irc-channel\t=\tThe channel to join");
                w.WriteLine("#   irc-port\t=\tThe port to use to connect");
                w.WriteLine("#   irc-identify\t=(true/false)\tDo you want the IRC bot to Identify itself with nickserv. Note: You will need to register it's name with nickserv manually.");
                w.WriteLine("#   irc-password\t=\tThe password you want to use if you're identifying with nickserv");
                w.WriteLine("#");
                w.WriteLine("#   anti-tunnels\t=\tStops people digging below max-depth");
                w.WriteLine("#   max-depth\t=\tThe maximum allowed depth to dig down");
                w.WriteLine("#   backup-time\t=\tThe number of seconds between automatic backups");
                w.WriteLine("#   overload\t=\tThe higher this is, the longer the physics is allowed to lag. Default 1500");
                w.WriteLine("#   advbuildercuboidlimit\t=\tThe higher this is, the more blocks advbuilders can mass edit. Default 10000");
                w.WriteLine("#   moderatorcuboidlimit\t=\tThe higher this is, the more blocks moderators can mass edit. Default 25000");
                w.WriteLine("#   operatorcuboidlimit\t=\tThe higher this is, the more blocks operators can mass edit. Default 50000");
                w.WriteLine("#   auto-unload\t=\tAutomatically unload maps after a period of inactivity.");
                w.WriteLine("#");
                w.WriteLine("#   backup-time\t=\tInterval that backups occur in seconds.");
                w.WriteLine("#");
                w.WriteLine("#   debug-mode\t=\t(true/false)Turns on debug messages.");
                w.WriteLine("#");
                w.WriteLine("#   BETA MODE: Make sure you have backups. Enabled experiemental features that may be harmful to your server.");
                w.WriteLine("#   Please ensure to report that you are using beta mode when reporting any bugs with this turned on.");
                w.WriteLine("#   beta-mode\t=\t(true/false)Turns on beta mode.");
                w.WriteLine();
                w.WriteLine();
                w.WriteLine("# Server options");
                w.WriteLine("server-name = " + ServerName);
                w.WriteLine("server-administrator = " + ServerAdministrator);
                w.WriteLine("motd = " + ServerMOTD);
                w.WriteLine("port = " + ServerPort.ToString());
                w.WriteLine("verify-names = " + VerifyNames.ToString().ToLower());
                w.WriteLine("public = " + PublicServer.ToString().ToLower());
                w.WriteLine("max-players = " + MaxPlayers.ToString());
                w.WriteLine("max-maps = " + MaxMaps.ToString());
                w.WriteLine("world-chat = " + AllowWorldChat.ToString().ToLower());
                w.WriteLine("guest-goto = " + GuestGoto.ToString().ToLower());
                w.WriteLine();
                w.WriteLine("# irc bot options");
                w.WriteLine("irc = " + IRCEnabled.ToString().ToLower());
                w.WriteLine("irc-nick = " + IRCNick);
                w.WriteLine("irc-server = " + IRCServer);
                w.WriteLine("irc-channel = " + IRCChannel);
                w.WriteLine("irc-port = " + IRCPort.ToString());
                w.WriteLine("irc-identify = " + IRCIdentify.ToString());
                w.WriteLine("irc-password = " + IRCPassword);
                w.WriteLine();
                w.WriteLine("# other options");
                w.WriteLine("anti-tunnels = " + AntiTunnelEnabled.ToString().ToLower());
                w.WriteLine("max-depth = " + MaxDepth.ToString());
                w.WriteLine("overload = " + PhysicsOverload.ToString());
                w.WriteLine("advbuildercuboidlimit = " + advBuilderCuboidLimit.ToString());
                w.WriteLine("moderatorcuboidlimit = " + moderatorCuboidLimit.ToString());
                w.WriteLine("operatorcuboidlimit = " + operatorCuboidLimit.ToString());
                w.WriteLine("auto-unload = " + AutoUnloadEnabled.ToString());
                w.WriteLine();
                w.WriteLine("# backup options");
                w.WriteLine("backup-time = " + BackupInterval.ToString());
                w.WriteLine();
                w.WriteLine("#Error logging");
                w.WriteLine("debug-mode = " + DebugEnabled.ToString().ToLower());
                w.WriteLine();
                w.WriteLine("#Beta Options");
                w.WriteLine("beta-mode = " + BetaMode.ToString().ToLower());
                w.Flush();
                w.Close();

                Logger.Log("SAVED: server.properties");
            }
            catch (Exception e)
            {
                Logger.Log("SAVE FAILED! server.properties", LogType.Error);
                Logger.Log(e.Message, LogType.ErrorMessage);
            }
        }
    }
}
