using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Xml.Serialization;
using MCSharp.Heartbeat;

namespace MCSharp
{
    public class Server
    {
        // Server Version String
        public static string Version { get { return String.Format("{0:0.00}", VersionNumber); } }
        public static double VersionNumber { get { return 0.94; } }
        public static double LatestVersion = VersionNumber;


        public delegate void LogHandler(string message);
        public delegate void HeartBeatHandler();
        public delegate void MessageEventHandler(string message);
        public delegate void PlayerListHandler(List<Player> playerList);
        public delegate void VoidHandler();

        public event MessageEventHandler OnURLChange;
        public event PlayerListHandler OnPlayerListChange;
        public event VoidHandler OnSettingsUpdate;

        static Socket listen;
        static System.Diagnostics.Process process;
        static System.Timers.Timer updateTimer = new System.Timers.Timer(100);
        static System.Timers.Timer messageTimer = new System.Timers.Timer(60000 * 5);   //Every 5 mins

        static Thread physThread;

        public static PlayerList administrators;
        public static PlayerList operators;
        public static PlayerList moderators;
        public static PlayerList banned;
        public static PlayerList bannedIP;
        public static PlayerList builders;
        public static PlayerList advbuilders;
        public static PlayerList bot;
        public static MapGenerator MapGen;
        public static PlayerList griefExempted;


        public static PerformanceCounter PCCounter;
        public static PerformanceCounter ProcessCounter;


        public static PlayerList ircControllers;

        public static Level mainLevel;
        public static List<Level> levels;
        public static List<string> afkset = new List<string>();
        public static List<string> messages = new List<string>();

        public static List<string> jokerMessages = new List<string>();

        public static List<string> griefExemption = new List<string>(); // Names not to be kicked from grief timer


        public const byte version = 7; // Need to figure out what this is for
        public static string salt = "";

        //public static MainLoop ml;
        public static Server s;

        private bool running;

        // Constructor
        public Server()
        {
            //ml = new MainLoop("server");
            Server.s = this;
        }

        public void Start()
        {
            Logger.Log("Starting Server");
            running = true;
            Logger.Log("Doing sanity checks");


            if (SanityCheck())
            {
                Logger.Log("Sanity Checks Passed");
                Properties.Load();

                if (File.Exists("lastseen.xml"))
                    Server.LoadLastSeen();  //Added by bman

                if (Properties.ServerAdministrator != String.Empty)
                {
                    Thread.Sleep(100);

                    SetupRanks();
                    SetupLevels();

                    if (!SetupNetwork())
                        return;

                    SetupGeneral();
                    SetupIRC();

                    // Init Heartbeat
                    MinecraftHeartbeat.Init();
                    MCSharpUpdateHeartbeat.Init();
                    WOMHeartbeat.Init();



                    // Init physics
                    physThread = new Thread(new ThreadStart(doPhysics));
                    physThread.Start();

                    // Autosaver init
                    new AutoSaver(Properties.BackupInterval);

                    // Check the port forward status
                    doPortCheck();
                }
                else
                {
                    Logger.Log("Error! No Administrator set in the server.properties", LogType.FatalError);
                }
            }
        }

        public void Stop ()
        {
            List<Player> kickList = Player.players;
            kickList.ForEach(delegate(Player p) { p.Kick("Server shutdown."); });
            kickList = Player.connections;
            kickList.ForEach(delegate(Player p) { p.Kick("Server shutdown."); });

            if (physThread != null)
                physThread.Abort();

            // Save Worlds

            // End running
            running = false;
        }

        #region === SETUP ===

        /// <summary>
        /// Checks the MCSharp environment to ensure everything is ready to launch
        /// </summary>
        /// <returns>Returns if all the checks passed</returns>
        bool SanityCheck()
        {
            bool blnSane = true;
            try
            {
                string levelsPath = Path.GetFullPath("levels/");
                string backupsPath = Path.GetFullPath("levels/backups/");
                string logsPath = Path.GetFullPath("logs/");

                Logger.Log("Checking if current directory has write access", LogType.Debug);
                // Check to see if the current folder has write permission
                if (hasWriteAccessToFolder(Directory.GetCurrentDirectory()))
                {
                    Logger.Log("Current directory does have write access", LogType.Debug);
                    if (Directory.Exists(levelsPath))
                    {
                        Logger.Log("Checking if 'levels' directory has write access", LogType.Debug);
                        // Check to see if the levels folder has write permission
                        if (hasWriteAccessToFolder(levelsPath))
                        {
                            Logger.Log("'levels' directory does have write access", LogType.Debug);
                            if (Directory.Exists(backupsPath))
                            {
                                Logger.Log("Checking if 'levels/backups' directory has write access", LogType.Debug);
                                // Check to see if the levels/backups fikder has write permission
                                if (!hasWriteAccessToFolder(backupsPath))
                                {
                                    Logger.Log("The server does not have write permission to the levels/backups folder.", LogType.Error);
                                    Logger.Log("Folder: " + Directory.GetCurrentDirectory(), LogType.ErrorMessage);
                                    blnSane = false;
                                }
                                else
                                {
                                    Logger.Log("The server does not write permission to the levels/backups folder.", LogType.Debug);
                                }
                            }
                        }
                        else
                        {
                            Logger.Log("The server does not have permission to the levels folder.", LogType.Error);
                            Logger.Log("Folder: " + Directory.GetCurrentDirectory() + "/levels", LogType.ErrorMessage);
                            blnSane = false;
                        }
                    }

                    // Check to see if the logs folder has write permission
                    if (Directory.Exists(logsPath))
                    {
                        if (!hasWriteAccessToFolder(logsPath))
                        {
                            Logger.Log("The server does not have permission to the logs folder.", LogType.Error);
                            Logger.Log("Folder: " + Directory.GetCurrentDirectory(), LogType.ErrorMessage);
                            blnSane = false;
                        }
                    }
                }
                else
                {
                    Logger.Log("The server does not have permission to the current server folder.", LogType.Error);
                    Logger.Log("Folder: " + Directory.GetCurrentDirectory(), LogType.ErrorMessage);
                    blnSane = false;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Sanity check failed. Server may not work as expected.", LogType.Error);
                Logger.Log(ex.Message, LogType.ErrorMessage);
            }

            return blnSane;
        }

        bool hasWriteAccessToFolder(string path)
        {
            bool hasAccess = true;
            try
            {
                path = Path.GetFullPath(path);
                Logger.Log(path, LogType.Debug);
                FileStream myfile = File.Create(path + "permissiontest.txt");
                myfile.Close();
                File.Delete(path + "permissiontest.txt");
                /*FileIOPermission writePermission = new FileIOPermission(FileIOPermissionAccess.AllAccess, path);
                if (!SecurityManager.IsGranted(writePermission))
                {
                    hasAccess = false;
                }*/
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message, LogType.Debug);
                hasAccess = false;
            }
            return hasAccess;
        }

        void SetupIRC()
        {
            if (Properties.IRCEnabled)
                new IRCBot();
        }

        void SetupGeneral()
        {
            // Setup Position Updates
            updateTimer.Elapsed += delegate
            {
                Player.GlobalUpdate();
            };
            updateTimer.Start();


            // Performance counters
            try
            {
                if (System.Environment.OSVersion.Platform != PlatformID.Unix)
                {
                    PCCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                    ProcessCounter = new PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName);
                    PCCounter.BeginInit();
                    ProcessCounter.BeginInit();
                    PCCounter.NextValue();
                    ProcessCounter.NextValue();
                }
            }
            catch (Exception e)
            {
                Logger.Log("Error Setting up performance counters", LogType.Error);
                Logger.Log(e.Message, LogType.ErrorMessage);
            }


            // Setup Random Messages
            messageTimer.Elapsed += delegate
            {
                RandomMessage();
            };
            messageTimer.Start();
            process = System.Diagnostics.Process.GetCurrentProcess();

            /*if (File.Exists("griefExemption.txt"))
            {
                StreamReader r = File.OpenText("griefExemption.txt");
                while (!r.EndOfStream)
                    griefExemption.Add(r.ReadLine());
            }
            else
                File.Create("griefExemption.txt").Close();
            */
            if (File.Exists("messages.txt"))
            {
                StreamReader r = File.OpenText("messages.txt");
                while (!r.EndOfStream)
                    messages.Add(r.ReadLine());
            }
            else
                File.Create("messages.txt").Close();

            // Setup Joker
            if (File.Exists("joker.txt"))
            {
                StreamReader r = File.OpenText("joker.txt");
                while (!r.EndOfStream)
                    jokerMessages.Add(r.ReadLine());
            }
            else
                File.Create("joker.txt").Close();


        }

        void SetupLevels()
        {
            levels = new List<Level>(Properties.MaxMaps);
            MapGen = new MapGenerator();

            Random random = new Random();

            if (File.Exists("levels/" + Properties.MainLevel + ".lvl"))
            {
                mainLevel = Level.Load(Properties.MainLevel);
                if (mainLevel == null)
                {
                    if (File.Exists("levels/" + Properties.MainLevel + ".lvl.backup"))
                    {
                        Logger.Log("Atempting to load backup.", LogType.Debug);
                        File.Copy("levels/" + Properties.MainLevel + ".lvl.backup", "levels/" + Properties.MainLevel + ".lvl", true);
                        mainLevel = Level.Load(Properties.MainLevel);
                        if (mainLevel == null)
                        {
                            Logger.Log("BACKUP FAILED!", LogType.Error);
                        }
                    }
                    else
                    {
                        Logger.Log("BACKUP NOT FOUND!", LogType.Error);
                    }

                }
            }
            else
            {
                Logger.Log("Warning: No main.lvl found.", LogType.Debug);
                Logger.Log("Creating Default main.lvl", LogType.Debug);
                mainLevel = new Level(Properties.MainLevel, 128, 64, 128, "flat");

                mainLevel.permissionvisit = LevelPermission.Guest;
                mainLevel.permissionbuild = LevelPermission.Guest;
                mainLevel.Save();
            }
            levels.Add(mainLevel);

            if (File.Exists("autoload.txt"))
            {
                try
                {
                    string[] lines = File.ReadAllLines("autoload.txt");
                    foreach (string line in lines)
                    {
                        //int temp = 0;
                        string _line = line.Trim();
                        try
                        {

                            if (_line == "") { continue; }
                            if (_line[0] == '#') { continue; }
                            int index = _line.IndexOf("=");

                            string key = line.Split('=')[0].Trim();
                            string value;
                            try
                            {
                                value = line.Split('=')[1].Trim();
                            }
                            catch
                            {
                                value = "0";
                            }

                            if (!key.Equals("main"))
                            {
                                Command.all.Find("load").Use(key + " " + value);
                            }
                            else // Main's already loaded so we just check and set the physics level
                            {
                                try
                                {
                                    int temp = int.Parse(value);
                                    if (temp >= 0 && temp <= 2)
                                    {
                                        mainLevel.Physics = (Physics)temp;
                                    }
                                }
                                catch
                                {
                                    Logger.Log("The Physics variable for main in autoload.txt is invalid!", LogType.Warning);
                                }
                            }


                        }
                        catch (Exception ex)
                        {
                            Logger.Log(_line + " failed.", LogType.Error);
                            Logger.Log(ex.Message, LogType.ErrorMessage);
                        }
                    }
                }
                catch
                {
                    Logger.Log("autoload.txt error", LogType.Error);
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            else
            {
                Logger.Log("autoload.txt does not exist", LogType.Debug);
            }
        }

        bool SetupNetwork()
        {
            bool success = true;
            try
            {
                Logger.Log("Creating listening socket on port " + Properties.ServerPort + "... ");
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, Properties.ServerPort);
                listen = new Socket(endpoint.Address.AddressFamily,
                                    SocketType.Stream, ProtocolType.Tcp);
                listen.Bind(endpoint);
                listen.Listen((int)SocketOptionName.MaxConnections);

                listen.BeginAccept(new AsyncCallback(Accept), null);
            }
            catch (SocketException e)
            {
                Logger.Log(e.Message, LogType.ErrorMessage);
                success = false;
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, LogType.ErrorMessage);
                success = false;
            }

            if (!success)
                Logger.Log("Could not create socket connection.  Shutting down.", LogType.FatalError);
            else
                Logger.Log("Listen Socket created successfully.", LogType.Information);

            return success;
        }

        void SetupRanks()
        {
            if (File.Exists("ranks/admins.txt"))
            {
                File.Copy("ranks/admins.txt", "ranks/moderators.txt", true);
                File.Delete("ranks/admins.txt");
            }

            griefExempted = new PlayerList("griefExemptions.txt");
            banned = new PlayerList("banned.txt");
            bannedIP = new PlayerList("banned-ip.txt");
            builders = new PlayerList("builders.txt");
            advbuilders = new PlayerList("advbuilders.txt");
            moderators = new PlayerList("moderators.txt");
            operators = new PlayerList("operators.txt");
            bot = new PlayerList("bots.txt");
            ircControllers = new PlayerList("../IRC_Controllers.txt");

            if (!bot.Contains("flist"))
            {
                bot.Add("flist");
            }
            Command.InitAll();
            Group.InitAll();
        }

        #endregion

        /// <summary>
        /// Checks the server port with utorrent to find out if the port is open to the internet
        /// </summary>
        /// <returns>True if port is open</returns>
        /// <remarks>Based on an example given by Fragmer (me at matvei dot org)</remarks>
        public static bool doPortCheck()
        {
            bool portOpen = false;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.utorrent.com/testport?plain=1&port=" + Properties.ServerPort);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream);
                        if (reader.ReadLine().StartsWith("ok"))
                        {
                            portOpen = true;
                            Logger.Log("Port " + Properties.ServerPort + " is open!");
                        }
                        else
                        {
                            Logger.Log("Port " + Properties.ServerPort + " is closed. You will need to set up forwarding.", LogType.Warning);
                        }
                    }
                }
                else
                {
                    Logger.Log("Port check did not complete. Please try again.", LogType.Warning);
                }

            }
            catch
            {
                Logger.Log("Could not start listening on port " + Properties.ServerPort + ". Another program may be using the port.", LogType.Warning);
            }
            return portOpen;
        }

        static void Accept(IAsyncResult result)
        {
            // found information: http://www.codeguru.com/csharp/csharp/cs_network/sockets/article.php/c7695
            // -Descention
            try
            {
                Logger.Log("Accepting New Connection", LogType.Debug);
                new Player(listen.EndAccept(result));
                
                listen.BeginAccept(new AsyncCallback(Accept), null);
            }
            catch (SocketException e)
            {
                Logger.Log(e.Message, LogType.ErrorMessage);
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, LogType.ErrorMessage);
            }
        }

        public static void ForceExit()
        {
            Player.players.ForEach(delegate(Player p) { p.Kick("Server shutdown."); });
            Player.connections.ForEach(delegate(Player p) { p.Kick("Server shutdown."); });

            if (physThread != null)
                physThread.Abort();
            if (process != null)
            {
                Logger.Log("Killing Process...", LogType.Error);
                process.Kill();
            }

        }

        public void PlayerListUpdate()
        {
            if (Server.s.OnPlayerListChange != null) Server.s.OnPlayerListChange(Player.players);
        }

        public void UpdateUrl(string url)
        {
            if (OnURLChange != null) OnURLChange(url);
        }

        /// <summary>
        /// This function parses input from the server console. In reality the server console should handle the wait for input
        /// and call a function within the library to parse a specific command.
        /// </summary>
        public void ParseInput()        //Handle console commands
        {
            string cmd;
            string msg;
            string output;
            while (running)
            {
                string input = Console.ReadLine();
                if (input == null)
                    continue;
                cmd = input.Split(' ')[0];
                if (input.Split(' ').Length > 1)
                    msg = input.Substring(input.IndexOf(' ')).Trim();
                else
                    msg = "";
                try
                {
                    switch (cmd)
                    {
                        case "help":
                            output = "Commands that the console can use: \n";
                            try
                            {
                                foreach (Command command in Command.all.All())
                                {
                                    if (command.ConsoleSupport)
                                    {
                                        output += command.Name + ", ";
                                    }

                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                            output = output.Remove(output.Length - 2);
                            Console.WriteLine(output);
                            break;
                        default:
                            Command runCmd = Command.all.Find(cmd);
                            if (runCmd != null)
                            {
                                if (runCmd.ConsoleSupport)
                                {
                                    runCmd.Use(msg);
                                }
                                else
                                {
                                    Console.WriteLine("This command is not supported by the console!");
                                }
                            }
                            else
                            {
                                Console.WriteLine("No such command!");
                            }
                            break;
                    }
                }
                catch (Exception e)
                {
                    Logger.Log(e.Message, LogType.ErrorMessage);
                }
                //Thread.Sleep(10);
            }
        }

        public static void doPhysics()
        {
            int wait = 250;
            while (true)
            {
                try
                {
                    if (wait > 0)
                    {
                        Thread.Sleep(wait);
                    }
                    DateTime Start = DateTime.Now;
                    levels.ForEach(delegate(Level L)    //update every level
                    {
                        L.CalcPhysics();
                    });
                    TimeSpan Took = DateTime.Now - Start;
                    wait = (int)250 - (int)Took.TotalMilliseconds;
                    if (wait < -Properties.PhysicsOverload)
                    {
                        levels.ForEach(delegate(Level L)    //update every level
                        {
                            try
                            {
                                L.Physics = Physics.Off;
                                L.ClearPhysics();
                            }
                            catch
                            {

                            }
                        });
                        Logger.Log("!PHYSICS SHUTDOWN!", LogType.Warning);
                        Player.GlobalMessage("!PHYSICS SHUTDOWN!");
                        wait = 250;
                    }
                    else if (wait < (int)(-Properties.PhysicsOverload * 0.75f))
                    {
                        Logger.Log("Physics is getting a bit overloaded...", LogType.Debug);
                    }
                }
                catch (Exception e)
                {
                    Logger.Log("Physics error!", LogType.Error);
                    Logger.Log(e.Message, LogType.ErrorMessage);
                    wait = 250;
                }

            }
        }

        public static void RandomMessage()
        {
            if (Player.number != 0 && messages.Count > 0)
                Player.GlobalMessage(messages[new Random().Next(0, messages.Count)]);
        }

        internal void SettingsUpdate()
        {
            if (OnSettingsUpdate != null) OnSettingsUpdate();
        }

        //Added by bman
        public static void SaveLastSeen()
        {
            List<string> saveList = new List<string>();
            foreach (KeyValuePair<string, DateTime> kvp in Player.lastSeen)
            {
                saveList.Add(kvp.Key + "," + kvp.Value.ToString());
            }

            // Serialization
            XmlSerializer xs = new XmlSerializer(typeof(List<string>));
            TextWriter xw = new StreamWriter("lastseen.xml");
            xs.Serialize(xw, saveList);
            xw.Close();
        }

        //Added by bman
        public static void LoadLastSeen()
        {
            // Deserialization
            List<string> loadList = new List<string>();
            XmlSerializer xs = new XmlSerializer(typeof(List<string>));
            TextReader xr = new StreamReader("lastseen.xml");
            loadList = (List<string>)xs.Deserialize(xr);
            xr.Close();

            Dictionary<string, DateTime> dict = new Dictionary<string, DateTime>();
            foreach (string s in loadList)
            {
                string key, value;
                string[] temp = s.Split(',');
                key = temp[0];
                value = temp[1];

                dict.Add(key, DateTime.Parse(value));
            }
            Player.lastSeen = dict;
        }
    }
}