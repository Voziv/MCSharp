using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;


namespace MCSharp
{
    public sealed partial class Player
    {


        #region == INCOMING ==

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
        void HandleLogin (byte[] message)
        {
            try
            {
                //byte[] message = (byte[])m;
                if (loggedIn)
                    return;

                byte version = message[0];
                name = enc.GetString(message, 1, 64).Trim();
                string verify = enc.GetString(message, 65, 32).Trim();
                byte type = message[129];

                if (Server.banned.Contains(name)) { Kick("You're banned!"); return; }
                if (Player.players.Count >= Properties.MaxPlayers) { Kick("Server full!"); return; }
                if (version != Server.version) { Kick("Wrong version!"); return; }
                if (name.Length > 16 || !ValidName(name)) { Kick("Illegal name!"); return; }

                if (Properties.VerifyNames)
                {
                    if (verify == "--" || verify != BitConverter.ToString(
                        md5.ComputeHash(enc.GetBytes(Server.salt + name))).
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

                Server.s.PlayerListUpdate();

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

        void HandleBlockchange (byte[] message)
        {
            int section = 0;
            try
            {

                if (group.Name == "bots") { return; } //connected bots cant do block changes

                if (!loggedIn)
                    return;
                if (CheckBlockSpam())
                    return;

                section++;
                ushort x = NTHO(message, 0);
                ushort y = NTHO(message, 2);
                ushort z = NTHO(message, 4);
                byte action = message[6];
                byte type = message[7];

                section++;
                if (type > 49)
                {
                    Kick("Unknown block type!");
                    return;
                }
                section++;
                byte b = level.GetTile(x, y, z);
                if (b == Block.Zero) { return; }
                section++;
                if (group.Permission < level.permissionbuild)
                {
                    SendMessage("Your not allowed to edit this map.");
                    SendBlockchange(x, y, z, b);
                    return;
                }
                section++;
                if (Blockchange != null)    //Blockchange actions now have priority, allowing people to /about blocks they cant change
                {
                    Blockchange(this, x, y, z, type);
                    return;
                }
                section++;
                if (group.Permission == LevelPermission.Guest)
                {
                    if (Rank == GroupEnum.Banned) //Just let them think theyre are griefing instead.
                    {
                        return;
                    }

                    int Diff = 0;

                    Diff = Math.Abs((int) (pos[0] / 32) - x);
                    Diff += Math.Abs((int) (pos[1] / 32) - y);
                    Diff += Math.Abs((int) (pos[2] / 32) - z);

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
                        SendBlockchange(x, y, z, b); return;
                    }

                    if (Properties.AntiTunnelEnabled)
                    {
                        if (y < level.depth / 2 - Properties.MaxDepth)     //Anti tunneling countermeasure
                        {
                            SendMessage("You're not allowed to build this far down!");
                            SendBlockchange(x, y, z, b); return;
                        }
                    }
                }
                section++;
                if (b == 7)    //Check for client hacker trying to delete adminium
                {
                    if (!checkOp())
                    {
                        Logger.Log(name + " attempted to delete an adminium block.", LogType.SuspiciousActivity);
                        GlobalMessageOps("To Ops &f-" + color + name + "&f- attempted to delete an adminium block.");
                        Kick("Hacked client.");
                        return;
                    }
                }
                section++;
                if (b >= 100 && !doors.doorBlocks.Contains(b))    //Special blocks only deletable by ops
                {
                    if (!checkOp())
                    {
                        SendMessage("You're not allowed to destroy this block!");
                        SendBlockchange(x, y, z, b);
                        return;
                    }
                    if (b >= 200)    //Special blocks that should never be replaced until they are finished
                    {
                        SendMessage("Block is active, you cant disturb it!");
                        SendBlockchange(x, y, z, b);
                        return;
                    }
                }

                section++;
                if (!Block.Placable(type))
                { // :3
                    SendMessage("You can't place this block type!");
                    SendBlockchange(x, y, z, b); return;
                }
                if (action > 1) { Kick("Unknown block action!"); }
                type = bindings[type];
                section++;
                //Ignores updating blocks that are the same and send block only to the player
                if (b == (byte) ((painting || action == 1) ? type : 0))
                {
                    if (painting || message[7] != type) { SendBlockchange(x, y, z, b); } return;
                }
                section++;
                //else

                if (!painting && action == 0)   //player is deleting a block
                {
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
                    deleteBlock(b, type, x, y, z);
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
                    placeBlock(b, type, x, y, z);
                }
                section++;

            }
            catch (Exception e)
            {
                Logger.Log(name + " has triggered a block change error", LogType.Error);
                Logger.Log(e.Message, LogType.ErrorMessage);
                GlobalMessageOps(name + " has triggered a block change error");
                IRCBot.Say(name + " has triggered a block change error");
                Player.GlobalMessage("An error occurred in section " + section + " : " + e.Message);
            }
        }

        private bool checkOp ()
        {
            bool isOp = false;
            switch (Rank)
            {
                case GroupEnum.Administrator:
                case GroupEnum.Operator:
                case GroupEnum.Moderator:
                    isOp = true;
                    break;
            }
            return isOp;
        }

        private void deleteBlock (byte b, byte type, ushort x, ushort y, ushort z)
        {
            // Don't bother with buildop here yet, deleted op_material should not turn into op_air.
            // That would be annoying. 

            /*switch (b)
            {
                case Block.door_tree: //Door
                    if (level.physics != 0)
                    { level.Blockchange(this, x, y, z, (byte)(Block.doorair_tree)); }
                    else
                    { SendBlockchange(x, y, z, b); }
                    break;
                case Block.door_obsidian:   //Door2
                    if (level.physics != 0)
                    { level.Blockchange(this, x, y, z, (byte)(Block.doorair_obsidian)); }
                    else
                    { SendBlockchange(x, y, z, b); }
                    break;
                case Block.door_glass:   //Door3
                    if (level.physics != 0)
                    { level.Blockchange(this, x, y, z, (byte)(Block.doorair_glass)); }
                    else
                    { SendBlockchange(x, y, z, b); }
                    break;
                case Block.door_white:
                    if (level.physics != 0)
                    { level.Blockchange(this, x, y, z, (byte)(Block.doorair_white)); }
                    else
                    { SendBlockchange(x, y, z, b); }
                    break;
                case Block.doorair_tree:   //Door_air
                case Block.doorair_obsidian:
                case Block.doorair_glass:
                case Block.doorair_white:
                    break;
                default:
                    level.Blockchange(this, x, y, z, (byte)(Block.air));
                    deletedBlocks += 1;
                    break;
            }*/
            bool doorCheck = false;
            for (int i = 0; i < doors.doorBlocks.Length; i++)
            {
                if (b.Equals(doors.doorBlocks[i]))
                {
                    //this.SendMessage("block checked ok");
                    doorCheck = true;
                    if (level.physics != 0)
                    { level.Blockchange(this, x, y, z, (doors.doorAirBlocks[i])); }
                    else
                    { SendBlockchange(x, y, z, b); /*this.SendMessage("break1 out of loop");*/ }
                }
                else if (b.Equals(doors.doorAirBlocks[i]))
                {
                    doorCheck = true;
                    break;
                }

            }
            if (!doorCheck) //if the block hasn't been changed, add air
            {
                //this.SendMessage("loop failed, regular delete");
                level.Blockchange(this, x, y, z, (byte) (Block.air));
                deletedBlocks += 1;
            }
        }

        private void placeBlock (byte b, byte type, ushort x, ushort y, ushort z)
        {
            switch (BlockAction)
            {
                case 0:     //normal
                    if (level.physics == 0)
                    {
                        switch (type)
                        {
                            case Block.dirt: //instant dirt to grass
                                level.Blockchange(this, x, y, z, (byte) (Block.grass));
                                break;
                            case Block.staircasestep:    //stair handler
                                if (level.GetTile(x, (ushort) (y - 1), z) == Block.staircasestep)
                                {
                                    SendBlockchange(x, y, z, Block.air);    //send the air block back only to the user.
                                    //level.Blockchange(this, x, y, z, (byte)(Block.air));
                                    level.Blockchange(this, x, (ushort) (y - 1), z, (byte) (Block.staircasefull));
                                    break;
                                }
                                //else
                                level.Blockchange(this, x, y, z, type);
                                break;
                            default:
                                level.Blockchange(this, x, y, z, type);
                                break;
                        }
                    }
                    else
                    {
                        level.Blockchange(this, x, y, z, type);
                    }
                    if (!Block.LightPass(type))
                    {
                        if (level.GetTile(x, (ushort) (y - 1), z) == Block.grass)
                        {
                            level.Blockchange(x, (ushort) (y - 1), z, Block.dirt);
                        }
                    }

                    break;
                case 1:     //solid
                    if (b == Block.blackrock) { SendBlockchange(x, y, z, b); return; }
                    level.Blockchange(this, x, y, z, (byte) (Block.blackrock));
                    break;
                case 2:     //lava
                    if (b == Block.lavastill) { SendBlockchange(x, y, z, b); return; }
                    level.Blockchange(this, x, y, z, (byte) (Block.lavastill));
                    break;
                case 3:     //water
                    if (b == Block.waterstill) { SendBlockchange(x, y, z, b); return; }
                    level.Blockchange(this, x, y, z, (byte) (Block.waterstill));
                    break;
                case 4:     //ACTIVE lava
                    if (b == Block.lava) { SendBlockchange(x, y, z, b); return; }
                    level.Blockchange(this, x, y, z, (byte) (Block.lava));
                    BlockAction = 0;
                    break;
                case 5:     //ACTIVE water
                    if (b == Block.water) { SendBlockchange(x, y, z, b); return; }
                    level.Blockchange(this, x, y, z, (byte) (Block.water));
                    BlockAction = 0;
                    break;
                case 6:     //OpGlass
                    if (b == Block.op_glass) { SendBlockchange(x, y, z, b); return; }
                    level.Blockchange(this, x, y, z, (byte) (Block.op_glass));
                    break;
                case 7:    // sapling >> tree
                    if (type == Block.shrub)
                    {
                        Random rand = new System.Random();
                        AddTree2(this.level, x, y, z, rand);
                    }
                    else
                    {
                        goto case 0;
                    }
                    break;
                case 8:
                    // BuildOP
                    break;
                case 9:
                    // BuildDoor
                    break;
                default:
                    if (BlockAction != 8 && BlockAction != 9) // Yea it's ugly, I know.
                    {
                        Logger.Log(name + " is breaking something", LogType.Debug);   // Should fix annoying log spam with buildop + builddoor
                        BlockAction = 0;
                    }
                    break;
            }
            #region === Buildop + Builddoor ===
            if (BlockAction == 8) //buildop
            {
                switch (type)
                {
                    case Block.air:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_air));
                        break;
                    case Block.rock:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_stone));
                        break;
                    case Block.dirt:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_dirt));
                        break;
                    case Block.stone:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_cobblestone));
                        break;
                    case Block.wood:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_wood));
                        break;
                    case Block.shrub:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_shrub));
                        break;
                    case Block.sand:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_sand));
                        break;
                    case Block.gravel:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_gravel));
                        break;
                    case Block.goldrock:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_goldrock));
                        break;
                    case Block.ironrock:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_ironrock));
                        break;
                    case Block.coal:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_coal));
                        break;
                    case Block.trunk:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_trunk));
                        break;
                    case Block.leaf:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_leaf));
                        break;
                    case Block.sponge:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_sponge));
                        break;
                    case Block.glass:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_glass));
                        break;
                    case Block.red:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_red));
                        break;
                    case Block.orange:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_orange));
                        break;
                    case Block.yellow:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_yellow));
                        break;
                    case Block.lightgreen:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_lightgreen));
                        break;
                    case Block.green:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_green));
                        break;
                    case Block.aquagreen:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_aquagreen));
                        break;
                    case Block.cyan:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_cyan));
                        break;
                    case Block.lightblue:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_lightblue));
                        break;
                    case Block.blue:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_blue));
                        break;
                    case Block.purple:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_purple));
                        break;
                    case Block.lightpurple:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_lightpurple));
                        break;
                    case Block.pink:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_pink));
                        break;
                    case Block.darkpink:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_darkpink));
                        break;
                    case Block.darkgrey:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_darkgrey));
                        break;
                    case Block.lightgrey:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_lightgrey));
                        break;
                    case Block.white:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_white));
                        break;
                    case Block.yellowflower:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_yellowflower));
                        break;
                    case Block.redflower:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_redflower));
                        break;
                    case Block.mushroom:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_mushroom));
                        break;
                    case Block.redmushroom:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_redmushroom));
                        break;
                    case Block.goldsolid:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_goldsolid));
                        break;
                    case Block.iron:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_iron));
                        break;
                    case Block.staircasefull:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_staircasefull));
                        break;
                    case Block.staircasestep:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_staircasestep));
                        break;
                    case Block.brick:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_brick));
                        break;
                    case Block.tnt:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_tnt));
                        break;
                    case Block.bookcase:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_bookcase));
                        break;
                    case Block.stonevine:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_stonevine));
                        break;
                    case Block.obsidian:
                        level.Blockchange(this, x, y, z, (byte) (Block.opsidian));
                        break;
                    default:
                        break;
                }
            }
            if (BlockAction == 9) //builddoor
            {
                switch (type)
                {
                    case Block.air:
                        break;
                    case Block.rock:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_stone));
                        break;
                    case Block.dirt:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_dirt));
                        break;
                    case Block.stone:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_cobblestone));
                        break;
                    case Block.wood:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_wood));
                        break;
                    case Block.shrub:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_shrub));
                        break;
                    case Block.sand:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_sand));
                        break;
                    case Block.gravel:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_gravel));
                        break;
                    case Block.goldrock:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_goldrock));
                        break;
                    case Block.ironrock:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_ironrock));
                        break;
                    case Block.coal:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_coal));
                        break;
                    case Block.trunk:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_tree));
                        break;
                    case Block.leaf:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_leaf));
                        break;
                    case Block.sponge:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_sponge));
                        break;
                    case Block.glass:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_glass));
                        break;
                    case Block.red:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_red));
                        break;
                    case Block.orange:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_orange));
                        break;
                    case Block.yellow:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_yellow));
                        break;
                    case Block.lightgreen:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_lightgreen));
                        break;
                    case Block.green:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_green));
                        break;
                    case Block.aquagreen:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_aquagreen));
                        break;
                    case Block.cyan:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_cyan));
                        break;
                    case Block.lightblue:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_lightblue));
                        break;
                    case Block.blue:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_blue));
                        break;
                    case Block.purple:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_purple));
                        break;
                    case Block.lightpurple:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_lightpurple));
                        break;
                    case Block.pink:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_pink));
                        break;
                    case Block.darkpink:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_darkpink));
                        break;
                    case Block.darkgrey:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_darkgrey));
                        break;
                    case Block.lightgrey:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_lightgrey));
                        break;
                    case Block.white:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_white));
                        break;
                    case Block.yellowflower:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_yellowflower));
                        break;
                    case Block.redflower:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_redflower));
                        break;
                    case Block.mushroom:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_mushroom));
                        break;
                    case Block.redmushroom:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_redmushroom));
                        break;
                    case Block.goldsolid:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_goldsolid));
                        break;
                    case Block.iron:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_iron));
                        break;
                    case Block.staircasefull:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_staircasefull));
                        break;
                    case Block.staircasestep:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_staircasestep));
                        break;
                    case Block.brick:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_brick));
                        break;
                    case Block.tnt:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_tnt));
                        break;
                    case Block.bookcase:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_bookcase));
                        break;
                    case Block.stonevine:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_stonevine));
                        break;
                    case Block.obsidian:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_obsidian));
                        break;
                    default:
                        break;
                }
            }
            #endregion === Buildop + Builddoor ===  // (blockaction 8 and 9)

            placedBlocks += 1;
        }

        void AddTree2 (Level Lvl, ushort x, ushort z, ushort y, Random Rand)
        {
            byte height = (byte) Rand.Next(4, 7);
            for (ushort zz = 0; zz < height; zz++)
            {
                if (Lvl.GetTile(x, (ushort) (z + zz), y) == Block.air)   //Not likly to trigger anyway
                {
                    Lvl.Blockchange(x, (ushort) (z + zz), y, Block.trunk);
                }
                else
                {
                    height = (byte) zz;
                }
            }

            short top = (short) (height - 3);

            for (short xx = (short) -top; xx <= top; ++xx)
            {
                for (short yy = (short) -top; yy <= top; ++yy)
                {
                    for (short zz = (short) -top; zz <= top; ++zz)
                    {
                        if (Lvl.GetTile((ushort) (x + xx), (ushort) (z + zz + height), (ushort) (y + yy)) == Block.air)   //Not likly to trigger anyway
                        {
                            //short Dist = (short)(Math.Abs(xx) + Math.Abs(yy) + Math.Abs(zz));
                            short Dist = (short) (Math.Sqrt(xx * xx + yy * yy + zz * zz));
                            if (Dist < top + 1)
                            {
                                if (Rand.Next((int) (Dist)) < 2)
                                {
                                    Lvl.Blockchange((ushort) (x + xx), (ushort) (z + zz + height), (ushort) (y + yy), Block.leaf);
                                }
                            }
                        }
                    }
                }
            }
        } // taken from map generator

        //void 

        void HandleInput (object m)
        {
            byte[] message = (byte[]) m;
            if (!loggedIn)
                return;

            byte thisid = message[0];
            ushort x = NTHO(message, 1);
            ushort y = NTHO(message, 3);
            ushort z = NTHO(message, 5);
            byte rotx = message[7];
            byte roty = message[8];
            pos = new ushort[3] { x, y, z };
            rot = new byte[2] { rotx, roty };
        }
        void HandleChat (byte[] message)
        {
            try
            {
                if (!loggedIn)
                    return;
                if (!group.CanChat)
                    return;

                //byte[] message = (byte[])m;
                string text = enc.GetString(message, 1, 64).Trim();

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
        void HandleCommand (string cmd, string message)
        {
            if (cmd.Equals("operators")) { cmd = "ops"; }
            if (cmd.Equals("moderators")) { cmd = "mods"; }
            if (cmd.Equals("z")) { cmd = "cuboid"; }
            if (cmd.Equals("p")) { cmd = "paint"; }
            if (cmd.Equals("r")) { cmd = "replace"; }
            if (cmd.Equals("a")) { cmd = "abort"; }
            if (cmd.Equals("rank")) { cmd = "setrank"; }
            if (cmd.Equals("sendlvl")) { cmd = "sendlevel"; }
            if (cmd.Equals("bd")) { cmd = "builddoor"; }
            if (cmd.Equals("bo")) { cmd = "buildop"; }
            if (cmd.Equals("l")) { cmd = "lock"; }
            if (cmd.Equals("u")) { cmd = "unlock"; }
            if (cmd.Equals("ov")) { cmd = "opview"; }
            if (cmd.Equals("uov")) { cmd = "unopview"; }
            if (cmd.Equals("d")) { cmd = "door"; }
            if (cmd.Equals("ud")) { cmd = "undoor"; }
            if (cmd.Equals("dv")) { cmd = "doorview"; }
            if (cmd.Equals("udv")) { cmd = "undoorview"; }
            if (cmd.Equals("w") || cmd.Equals("whisper")) { cmd = "whisperchat"; }

            Command command = Command.all.Find(cmd);
            if (command != null)
            {
                if (command.CanUse(this))
                {
                    Logger.Log(name + " uses /" + cmd + " " + message, LogType.UserCommand);
                    command.Use(this, message);
                }
                else { SendMessage("You are not allowed to use \"" + cmd + "\"!"); }
            }
            else { SendMessage("Unknown command \"" + cmd + "\"!"); }
        }
        void HandleQuery (string to, string message)
        {
            Player p = Find(to);
            if (p == this) { SendMessage("Trying to talk to yourself, huh?"); return; }
            if (p != null && !p.hidden)
            {
                Logger.Log(name + " @" + p.name + ": " + message, LogType.PrivateChat);
                p.SendChat(this, "&e[<] " + color + name + ": &f" + message);
                SendChat(this, "&9[>] " + p.color + p.name + ": &f" + message);
            }
            else { SendMessage("Player \"" + to + "\" doesn't exist!"); }
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
            buffer[0] = Server.version;
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

        byte[] HTNO (ushort x)
        {
            byte[] y = BitConverter.GetBytes(x); Array.Reverse(y); return y;
        }
        ushort NTHO (byte[] x, int offset)
        {
            byte[] y = new byte[2];
            Buffer.BlockCopy(x, offset, y, 0, 2); Array.Reverse(y);
            return BitConverter.ToUInt16(y, 0);
        }
        byte[] HTNO (short x)
        {
            byte[] y = BitConverter.GetBytes(x); Array.Reverse(y); return y;
        }

        #endregion


    }
}
