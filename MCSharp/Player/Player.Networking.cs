using System;
using System.Timers;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Text;
using System.Security.Cryptography;
using MCSharp.World;

namespace MCSharp
{
    public sealed partial class Player
    {
        /// <summary>
        /// This is the byte that the client sends to identify the 
        /// protocol version
        /// </summary>
        public const byte MinecraftClassicProtocolVersion = 7; 

        // Variables
        Socket socket;
        public string ip;

        public static List<Player> connections = new List<Player>(Properties.MaxPlayers);

        System.Timers.Timer loginTimer = new System.Timers.Timer(20000);
        System.Timers.Timer pingTimer = new System.Timers.Timer(500);

        byte[] buffer = new byte[0];
        byte[] tempbuffer = new byte[0xFF];

        public bool disconnected = false;

        /// <summary>
        /// Initializes the players networking and handles the incoming socket connection.
        /// </summary>
        /// <param name="s">The incoming socket connection</param>
        void initNetworking (Socket s)
        {
            try
            {
                socket = s;
                ip = socket.RemoteEndPoint.ToString().Split(':')[0];
                Logger.Log(ip + " connected.", LogType.Information);

                // Check to see if they are IP Banned
                if (!Server.bannedIP.Contains(ip))
                {
                    // Check to see if we already have too many connections
                    if (connections.Count < 5)
                    {
                        socket.BeginReceive(tempbuffer, 0, tempbuffer.Length, SocketFlags.None,
                                        new AsyncCallback(Receive), this);
                        loginTimer.Elapsed += loginTimerCallback;
                        loginTimer.Start();

                        // Start pinging the client
                        pingTimer.Elapsed += delegate { SendPing(); };
                        pingTimer.Start();

                        connections.Add(this);
                    }
                    else
                    {
                        Kick("Too many connections!");
                    }
                }
                else
                {
                    Kick("You're banned!");
                }
            }
            catch (Exception e)
            {
                Logger.Log("Error while initializing the player networking", LogType.Error);
                Logger.Log(e.Message, LogType.ErrorMessage);
                Kick("Server Error!");
            }
        }

        /// <summary>
        /// Handles the login timer callback once it finishes. If the player
        /// has not been authenticated then we kick them.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void loginTimerCallback (Object sender, ElapsedEventArgs e)
        {
            loginTimer.Stop();
            if (!loggedIn)
            {
                Kick("You must login to play. Please try again.");
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
        }


        #region == INCOMING ==

        /// <summary>
        /// Handles network callbacks and adding to the players buffer
        /// </summary>
        /// <param name="result"></param>
        static void Receive (IAsyncResult result)
        {
            Player p = (Player) result.AsyncState;
            if (p.disconnected)
                return;
            try
            {
                int length = p.socket.EndReceive(result);
                if (length == 0) { p.Disconnect(); return; }

                byte[] b = new byte[p.buffer.Length + length];
                Buffer.BlockCopy(p.buffer, 0, b, 0, p.buffer.Length);
                Buffer.BlockCopy(p.tempbuffer, 0, b, p.buffer.Length, length);

                p.buffer = p.HandleMessage(b);
                p.socket.BeginReceive(p.tempbuffer, 0, p.tempbuffer.Length, SocketFlags.None,
                                      new AsyncCallback(Receive), p);
            }
            catch (SocketException)
            {
                p.Disconnect();
            }
            catch (Exception e)
            {
                Logger.Log(e.Message);
                p.Kick("Error!");
            }
        }

        /// <summary>
        /// Handles messages in the buffer
        /// </summary>
        /// <param name="buffer">the buffer to process</param>
        /// <returns>returns the buffer minus the packet we just handled</returns>
        byte[] HandleMessage (byte[] buffer)
        {
            try
            {
                // Get the length of the message by checking the first byte
                int length = 0; byte msg = buffer[0];

                // Verify the length based on the packet id
                switch (msg)
                {
                    case 0:
                        length = 130;
                        break; // login
                    case 5:
                        if (!loggedIn)
                            goto default;
                        length = 8;
                        break; // blockchange
                    case 8:
                        if (!loggedIn)
                            goto default;
                        length = 9;
                        break; // input
                    case 13:
                        if (!loggedIn)
                            goto default;
                        length = 65;
                        break; // chat
                    case 2:
                        SMPKick("Please use the Minecraft Classic client to log onto this server!");
                        return new byte[0];
                    default:
                        // We should see if it's a minecraft smp client trying to log in
                        Kick("Unhandled message id \"" + msg + "\"!");
                        //Kick("Please use the Minecraft Classic client to log onto this server!");
                        return new byte[0];
                }

                // Copy the packet we are working with into the message so we can work on it and
                // truncate the packet from the buffer
                if (buffer.Length > length)
                {
                    byte[] message = new byte[length];
                    Buffer.BlockCopy(buffer, 1, message, 0, length);

                    byte[] tempbuffer = new byte[buffer.Length - length - 1];
                    Buffer.BlockCopy(buffer, length + 1, tempbuffer, 0, buffer.Length - length - 1);

                    buffer = tempbuffer;

                    // Handle our packet
                    switch (msg)
                    {
                        case 0:
                            HandleLogin(message);
                            break;
                        case 5:
                            if (!loggedIn)
                                break;
                            HandleBlockchange(message);
                            break;
                        case 8:
                            if (!loggedIn)
                                break;
                            HandleInput(message);
                            break;
                        case 13:
                            if (!loggedIn)
                                break;
                            HandleChat(message);
                            break;
                    }
                    //thread.Start((object)message);
                    if (buffer.Length > 0)
                        buffer = HandleMessage(buffer);
                    else
                        return new byte[0];
                }
            }
            catch (Exception e)
            {
                Logger.Log(e.Message);
            }
            return buffer;
        }

        /// <summary>
        /// Handles a player login packet
        /// </summary>
        /// <param name="message">The login packet</param>
        void HandleLogin (byte[] message)
        {
            try
            {
                //byte[] message = (byte[])m;
                if (loggedIn)
                    return;

                byte version = message[0];
                name = Encoding.ASCII.GetString(message, 1, 64).Trim();
                string verify = Encoding.ASCII.GetString(message, 65, 32).Trim();
                byte type = message[129];

                if (Server.banned.Contains(name)) { Kick("You're banned!"); return; }
                if (Player.players.Count >= Properties.MaxPlayers) { Kick("Server full!"); return; }
                if (version != MinecraftClassicProtocolVersion) { Kick("Wrong version!"); return; }
                if (name.Length > 16 || !ValidName(name)) { Kick("Illegal name!"); return; }

                if (Properties.VerifyNames)
                {
                    if (verify == "--" || verify != BitConverter.ToString(

                        MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(Server.salt + name))).
                        Replace("-", "").ToLower().TrimStart('0'))
                    {
                        if (ip != "127.0.0.1")
                        {
                            Kick("Login failed! Try again."); return;
                        }
                    }
                }
                Player old = Player.Find(name);
                Logger.Log(ip + " logging in as " + name + ".");

                if (old != null)
                {
                    if (Properties.VerifyNames)
                    {
                        old.Kick("Someone else logged in as " + name + ". Duplicate logins are not allowed!");
                    }
                    else { Kick("Already logged in!"); return; }
                }
                left.Remove(name.ToLower());

                if (Properties.ServerAdministrator == name)
                    group = Group.Find("administrator");
                else if (Server.bot.Contains(name))
                    group = Group.Find("bots");
                else if (Server.operators.Contains(name))
                    group = Group.Find("operator");
                else if (Server.moderators.Contains(name))
                    group = Group.Find("moderator");
                else if (Server.advbuilders.Contains(name))
                    group = Group.Find("advbuilder");
                else if (Server.builders.Contains(name))
                    group = Group.Find("builder");
                else
                    group = Group.standard;

                SendMotd();
                SendMap();

                if (disconnected)
                    return;

                loggedIn = true;
                id = FreeId();

                players.Add(this);
                connections.Remove(this);

                

                GlobalChat(this, "&a+ " + color + name + "&e joined the game.", false);

                /*
                if (!Server.console && Server.win != null)
                    Server.win.UpdateClientList(players);
                */
                IRCBot.Say(name + " joined the game.");

                //Test code to show wehn people come back with different accounts on the same IP
                string temp = "Lately known as:";
                bool found = false;
                if (ip != "127.0.0.1")
                {
                    foreach (KeyValuePair<string, string> prev in left)
                    {
                        if (prev.Value == ip)
                        {
                            found = true;
                            temp += " " + prev.Key;
                        }
                    }
                    if (found)
                    {
                        GlobalMessageOps(temp);
                        Logger.Log(temp);
                        IRCBot.Say(temp);
                    }
                }

                ushort x = (ushort) ((0.5 + level.spawnx) * 32);
                ushort y = (ushort) ((1 + level.spawny) * 32);
                ushort z = (ushort) ((0.5 + level.spawnz) * 32);
                pos = new ushort[3] { x, y, z }; rot = new byte[2] { level.rotx, level.roty };

                GlobalSpawn(this, x, y, z, rot[0], rot[1], true);
                foreach (Player p in players)
                {
                    if (p.level == level && p != this && !p.hidden)
                        SendSpawn(p.id,
                            p.color + p.name,
                            p.pos[0],
                            p.pos[1],
                            p.pos[2],
                            p.rot[0],
                            p.rot[1]);
                }
                Loading = false;
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, LogType.ErrorMessage);
                Player.GlobalMessage("An error occurred: " + e.Message);
            }
        }


        /// <summary>
        /// Handles a player blockchange packet
        /// </summary>
        /// <param name="message">The blockchange packet</param>
        void HandleBlockchange (byte[] message)
        {
            try
            {
                // Bots are not allows making block changes
                if (group.Name == "bots") { return; }

                // Players who are not logged in cannot
                if (!loggedIn)
                    return;

                // Players who spam can't make block changes
                if (CheckBlockSpam())
                    return;

                // Get the packet information
                ushort x = NTHO(message, 0);
                ushort y = NTHO(message, 2);
                ushort z = NTHO(message, 4);
                byte action = message[6];
                byte newBlockType = message[7];

                // If the client is trying to place a block > 49, it's either:
                // - A new version (unlikely)
                // - A hacked client (much more likely)
                if (newBlockType > 49)
                {
                    Kick("Unknown block type!");
                    return;
                }

                // Get the block that we're trying to change
                byte targetBlockType = level.GetTile(x, y, z);

                // Chan't changed a block type of Zero
                if (targetBlockType == Block.Zero)
                {
                    return;
                }

                // Check to see if the player is allowed editing the map
                if (group.Permission < level.permissionbuild)
                {
                    SendMessage("Your not allowed to edit this map.");
                    // Replace the block we just destroyed
                    SendBlockchange(x, y, z, targetBlockType);
                    return;
                }

                // Check to see if we have a block change to process
                // Blockchange actions now have priority, allowing people to /about blocks they cant change
                if (Blockchange != null)
                {
                    Blockchange(this, x, y, z, newBlockType);
                    return;
                }

                // Check to see if the permission on the level is guest
                // If the permission is guest, then we check to make sure no hacked clients are messing with the map
                // TODO: I'm not sure this should only apply to guests -- Voziv -- 
                if (group.Permission == LevelPermission.Guest)
                {
                    // Let banned players greif
                    if (Rank == GroupEnum.Banned) //Just let them think theyre are griefing instead.
                    {
                        return;
                    }

                    // Get the distance that the user is trying to place the block
                    int Diff = 0;
                    Diff = Math.Abs((int) (pos[0] / 32) - x);
                    Diff += Math.Abs((int) (pos[1] / 32) - y);
                    Diff += Math.Abs((int) (pos[2] / 32) - z);

                    // TODO: This gets triggered by WOM and possible the normal minecraft client. I'm thinking we should just leave it at 10 blocks they get kicked
                    if (Diff > 9)   //Danger level compensation
                    {
                        if (Diff > 10)  //Too much distance
                        {
                            Logger.Log(name + " attempted to build with a " + Diff.ToString() + " distance offset", LogType.SuspiciousActivity);
                            GlobalMessageOps("To Ops &f-" + color + name + "&f- attempted to build with a " + Diff.ToString() + " distance offset");
                            Kick("Hacked client.");
                            return;
                        }
                        SendMessage("You cant build that far away.");
                        SendBlockchange(x, y, z, targetBlockType); return;
                    }

                    // Make sure kids aren't anti tunnelling
                    if (Properties.AntiTunnelEnabled)
                    {
                        if (y < level.depth / 2 - Properties.MaxDepth)     //Anti tunneling countermeasure
                        {
                            SendMessage("You're not allowed to build this far down!");
                            SendBlockchange(x, y, z, targetBlockType); return;
                        }
                    }
                }

                // More anti hax - Check to see if we are deleting adminium
                if (targetBlockType == Block.blackrock)    //Check for client hacker trying to delete adminium
                {
                    // Ops are allowed to delete adminium
                    if (!checkOp())
                    {
                        Logger.Log(name + " attempted to delete an adminium block.", LogType.SuspiciousActivity);
                        GlobalMessageOps("To Ops &f-" + color + name + "&f- attempted to delete an adminium block.");
                        Kick("Hacked client.");
                        return;
                    }
                }

                // Special Blocks that only operators can delete
                if (targetBlockType >= 100 && !doors.doorBlocks.Contains(targetBlockType))
                {
                    if (!checkOp())
                    {
                        SendMessage("You're not allowed to destroy this block!");
                        SendBlockchange(x, y, z, targetBlockType);
                        return;
                    }
                    //Special blocks that should never be replaced until they are finished
                    if (targetBlockType >= 200)
                    {
                        SendMessage("Block is active, you cant disturb it!");
                        SendBlockchange(x, y, z, targetBlockType);
                        return;
                    }
                }

                // If the block is not placable by the current player, deny them
                if (!Block.Placable(newBlockType))
                {
                    SendMessage("You can't place this block type!");
                    SendBlockchange(x, y, z, targetBlockType); return;
                }

                // Validate the action coming from the client
                if (action > 1)
                {
                    Kick("Unknown block action!");
                }

                newBlockType = bindings[newBlockType];
                

                //Ignores updating blocks that are the same and send block only to the player
                // If we are trying to place or paint a target block that is the same as the block
                // that is there, then we ignore it
                if (targetBlockType == (byte) ((painting || action == 1) ? newBlockType : 0))
                {
                    if (painting || message[7] != newBlockType)
                    {
                        SendBlockchange(x, y, z, targetBlockType);
                    }
                    return;
                }
                
                // Action Processing
                // 0 == Deletion
                // 1 == Placement

                // Delete block requests only go through if we are not painting
                if (!painting && action == 0)
                {
                    // Warn the server if the player places a block around the spawn area
                    if ((x == level.spawnx) && (y == level.spawny - 1) && (z == level.spawnz)) // if player deletes the spawn block or adjacent-ish
                    {
                        Player.GlobalChat(this, (this.name + " has deleted a spawn block."), false);
                        IRCBot.Say("Global: " + (this.name + " has deleted a spawn block."));
                    }
                    if ((x == level.spawnx) && (y == level.spawny - 2) && (z == level.spawnz))
                    {
                        Player.GlobalChat(this, (this.name + " has deleted a spawn block."), false);
                        IRCBot.Say("Global: " + (this.name + " has deleted a spawn block."));
                    }
                    deleteBlock(targetBlockType, newBlockType, x, y, z);
                }
                else    //player is placing a block
                {
                    if ((x == level.spawnx) && (y == level.spawny - 1) && (z == level.spawnz))// if player deletes the spawn block or adjacent-ish
                    {
                        Player.GlobalChat(this, (this.name + " has deleted a spawn block."), false);
                        IRCBot.Say("Global: " + (this.name + " has deleted a spawn block."));
                    }
                    if ((x == level.spawnx) && (y == level.spawny - 2) && (z == level.spawnz))
                    {
                        Player.GlobalChat(this, (this.name + " has deleted a spawn block."), false);
                        IRCBot.Say("Global: " + (this.name + " has deleted a spawn block."));
                    }
                    placeBlock(targetBlockType, newBlockType, x, y, z);
                }
            }
            catch (Exception e)
            {
                Logger.Log(name + " has triggered a block change error", LogType.Error);
                Logger.Log(e.Message, LogType.ErrorMessage);
                GlobalMessageOps(name + " has triggered a block change error");
                IRCBot.Say(name + " has triggered a block change error");
            }
        }

        #endregion


        #region == DISCONNECTING ==

        /// <summary>
        /// Handles a player disconnection. Cleans up the player from other players on the server
        /// as well as handles cleaning up the player from the system.
        /// </summary>
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

        /// <summary>
        /// Kicks the player with a given message
        /// </summary>
        /// <param name="message">The reason the player was kicked</param>
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

        /// <summary>
        /// A kick function that works for the new SMP clients. That way they
        /// have a friendly way of being removed from the server instead of
        /// "Invalid packet id 2"
        /// </summary>
        /// <param name="message">The message to kick a SMP player with</param>
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



        #region == OUTGOING ==

        public void SendRaw (int id)
        {
            SendRaw(id, new byte[0]);
        }
        public void SendRaw (int id, byte[] send)
        {
            byte[] buffer = new byte[send.Length + 1];
            buffer[0] = (byte) id;
            Buffer.BlockCopy(send, 0, buffer, 1, send.Length);
            try
            {
                socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, delegate(IAsyncResult result) { }, null);
            }
            catch (SocketException)
            {
                Disconnect();
            }
        }

        public void SendRaw (byte[] send)
        {
            try
            {
                socket.BeginSend(send, 0, send.Length, SocketFlags.None, delegate(IAsyncResult result) { }, null);
            }
            catch (SocketException)
            {
                Disconnect();
            }
        }



        public void SendMessage (string message)
        {
            unchecked
            {
                SendMessage((byte) -1, message);
            }
        }
        public void SendChat (Player p, string message)
        {
            SendMessage(p.id, message);
        }
        public void SendMessage (byte id, string message)
        {
            byte[] buffer = new byte[65];
            unchecked { buffer[0] = id; }
            foreach (string line in Wordwrap(message))
            {
                StringFormat(line, 64).CopyTo(buffer, 1);
                SendRaw(13, buffer);
            }
        }
        public void SendMotd ()
        {
            byte[] buffer = new byte[130];
            buffer[0] = MinecraftClassicProtocolVersion;
            StringFormat(Properties.ServerName, 64).CopyTo(buffer, 1);
            StringFormat(Properties.ServerMOTD, 64).CopyTo(buffer, 65);
            if (checkOp())
                buffer[129] = 100;
            else
                buffer[129] = 0;
            SendRaw(0, buffer);
        }
        public void SendMap ()
        {
            SendRaw(2);
            byte[] buffer = new byte[level.blocks.Length + 4];
            BitConverter.GetBytes(IPAddress.HostToNetworkOrder(level.blocks.Length)).CopyTo(buffer, 0);
            for (int i = 0; i < level.blocks.Length; ++i)
            {
                buffer[4 + i] = Block.Convert(level.blocks[i]);
            }
            buffer = GZip(buffer);
            int number = (int) Math.Ceiling(((double) buffer.Length) / 1024);
            for (int i = 1; buffer.Length > 0; ++i)
            {
                short length = (short) Math.Min(buffer.Length, 1024);
                byte[] send = new byte[1027];
                HTNO(length).CopyTo(send, 0);
                Buffer.BlockCopy(buffer, 0, send, 2, length);
                byte[] tempbuffer = new byte[buffer.Length - length];
                Buffer.BlockCopy(buffer, length, tempbuffer, 0, buffer.Length - length);
                buffer = tempbuffer;
                send[1026] = (byte) (i * 100 / number);
                SendRaw(3, send);
                Thread.Sleep(10);
            } buffer = new byte[6];
            HTNO((short) level.width).CopyTo(buffer, 0);
            HTNO((short) level.depth).CopyTo(buffer, 2);
            HTNO((short) level.height).CopyTo(buffer, 4);
            SendRaw(4, buffer);
        }
        public void SendSpawn (byte id, string name, ushort x, ushort y, ushort z, byte rotx, byte roty)
        {
            pos = new ushort[3] { x, y, z }; // This could be remove and not effect the server :/
            rot = new byte[2] { rotx, roty };
            byte[] buffer = new byte[73]; buffer[0] = id;
            StringFormat(name, 64).CopyTo(buffer, 1);
            HTNO(x).CopyTo(buffer, 65);
            HTNO(y).CopyTo(buffer, 67);
            HTNO(z).CopyTo(buffer, 69);
            buffer[71] = rotx; buffer[72] = roty;
            SendRaw(7, buffer);
        }
        public void SendPos (byte id, ushort x, ushort y, ushort z, byte rotx, byte roty)
        {
            pos = new ushort[3] { x, y, z };
            rot = new byte[2] { rotx, roty };
            byte[] buffer = new byte[9]; buffer[0] = id;
            HTNO(x).CopyTo(buffer, 1);
            HTNO(y).CopyTo(buffer, 3);
            HTNO(z).CopyTo(buffer, 5);
            buffer[7] = rotx; buffer[8] = roty;
            SendRaw(8, buffer);
        }
        public void SendDie (byte id) { SendRaw(0x0C, new byte[1] { id }); }
        public void SendBlockchange (ushort x, ushort y, ushort z, byte type)
        {
            byte[] buffer = new byte[7];
            HTNO(x).CopyTo(buffer, 0);
            HTNO(y).CopyTo(buffer, 2);
            HTNO(z).CopyTo(buffer, 4);
            buffer[6] = Block.Convert(type);
            SendRaw(6, buffer);
        }
        void SendKick (string message) { SendRaw(14, StringFormat(message, 64)); }
        void SendPing () { /*pingDelay = 0; pingDelayTimer.Start();*/ SendRaw(1); }
        void UpdatePosition ()
        {

            //pingDelayTimer.Stop();

            // Shameless copy from JTE's Server
            byte changed = 0;   //Denotes what has changed (x,y,z, rotation-x, rotation-y)
            // 0 = no change - never happens with this code.
            // 1 = position has changed
            // 2 = rotation has changed
            // 3 = position and rotation have changed
            // 4 = Teleport Required (maybe something to do with spawning)
            // 5 = Teleport Required + position has changed
            // 6 = Teleport Required + rotation has changed
            // 7 = Teleport Required + position and rotation has changed
            //NOTE: Players should NOT be teleporting this often. This is probably causing some problems.
            if (oldpos[0] != pos[0] || oldpos[1] != pos[1] || oldpos[2] != pos[2])
            {
                changed |= 1;
            }
            if (oldrot[0] != rot[0] || oldrot[1] != rot[1])
            {
                changed |= 2;
            }
            if (Math.Abs(pos[0] - basepos[0]) > 32 || Math.Abs(pos[1] - basepos[1]) > 32 || Math.Abs(pos[2] - basepos[2]) > 32)
            {
                changed |= 4;
            }
            if ((oldpos[0] == pos[0] && oldpos[1] == pos[1] && oldpos[2] == pos[2]) && (basepos[0] != pos[0] || basepos[1] != pos[1] || basepos[2] != pos[2]))
            {
                changed |= 4;
            }

            byte[] buffer = new byte[0]; byte msg = 0;
            if ((changed & 4) != 0)
            {
                msg = 8; //Player teleport - used for spawning or moving too fast
                buffer = new byte[9]; buffer[0] = id;
                HTNO(pos[0]).CopyTo(buffer, 1);
                HTNO(pos[1]).CopyTo(buffer, 3);
                HTNO(pos[2]).CopyTo(buffer, 5);
                buffer[7] = rot[0]; buffer[8] = rot[1];
            }
            else if (changed == 1)
            {
                try
                {
                    msg = 10; //Position update
                    buffer = new byte[4]; buffer[0] = id;
                    Buffer.BlockCopy(System.BitConverter.GetBytes((sbyte) (pos[0] - oldpos[0])), 0, buffer, 1, 1);
                    Buffer.BlockCopy(System.BitConverter.GetBytes((sbyte) (pos[1] - oldpos[1])), 0, buffer, 2, 1);
                    Buffer.BlockCopy(System.BitConverter.GetBytes((sbyte) (pos[2] - oldpos[2])), 0, buffer, 3, 1);
                }
                catch
                {

                }
            }
            else if (changed == 2)
            {
                msg = 11; //Orientation update
                buffer = new byte[3]; buffer[0] = id;
                buffer[1] = rot[0]; buffer[2] = rot[1];
            }
            else if (changed == 3)
            {
                try
                {
                    msg = 9; //Position and orientation update
                    buffer = new byte[6]; buffer[0] = id;
                    Buffer.BlockCopy(System.BitConverter.GetBytes((sbyte) (pos[0] - oldpos[0])), 0, buffer, 1, 1);
                    Buffer.BlockCopy(System.BitConverter.GetBytes((sbyte) (pos[1] - oldpos[1])), 0, buffer, 2, 1);
                    Buffer.BlockCopy(System.BitConverter.GetBytes((sbyte) (pos[2] - oldpos[2])), 0, buffer, 3, 1);
                    buffer[4] = rot[0]; buffer[5] = rot[1];
                }
                catch
                {

                }
            }

            if (changed != 0)
                foreach (Player p in players)
                {
                    if (p != this && p.level == level)
                    {
                        p.SendRaw(msg, buffer);
                    }
                }
            oldpos = pos; oldrot = rot;
        }

        #endregion


        #region == Host <> Network ==

        /// <summary>
        /// Converts a ushort to network byte order
        /// </summary>
        /// <param name="x">The ushort value to convert</param>
        /// <returns>A byte array containing the converted value's bytes</returns>
        byte[] HTNO (ushort x)
        {
            byte[] y = BitConverter.GetBytes(x); Array.Reverse(y); return y;
        }

        /// <summary>
        /// Converts a ushort to host byte order
        /// </summary>
        /// <param name="x">The array containing the number</param>
        /// <param name="offset">The offset in the array that the number starts at</param>
        /// <returns>The ushort value</returns>
        ushort NTHO (byte[] x, int offset)
        {
            byte[] y = new byte[2];
            Buffer.BlockCopy(x, offset, y, 0, 2); Array.Reverse(y);
            return BitConverter.ToUInt16(y, 0);
        }

        /// <summary>
        /// Converts a short to network byte order
        /// </summary>
        /// <param name="x">The short value to convert</param>
        /// <returns>A byte array containing the converted value's bytes</returns>
        byte[] HTNO (short x)
        {
            byte[] y = BitConverter.GetBytes(x); Array.Reverse(y); return y;
        }

        #endregion


    }
}
