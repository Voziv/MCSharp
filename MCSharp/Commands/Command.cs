using System;

namespace MCSharp
{
    public class Command
    {
        // Static Members
        public static CommandList all = new CommandList();


        // Properties
        public string Name { get { return strName; } }
        public CommandGroup Category { get { return cmdGroup; } }
        public bool ConsoleSupport { get { return blnConsoleSupported; } }
       
        
        // Variables
        protected bool blnConsoleSupported;
        protected string strName;
        protected CommandGroup cmdGroup;
        protected GroupEnum accessGroup;
        protected Int16 intParams;
        protected Player player;

        // Constructor
         public Command(CommandGroup g, GroupEnum group, string name)
        {
            cmdGroup = g;
            accessGroup = group;
            strName = name;
            blnConsoleSupported = false;
        }

        // Code to run when used by the console
        public virtual void Use(string message)
        {
            Logger.Log("This is not a valid command. Please report what you did to get here to Voziv!", LogType.Error);
        }

        // Code to run when used by a player
        public virtual void Use(Player p, string message)
        {
            p.SendMessage("Uh oh, this shouldn't have happened. Please report what you did to get here to Voziv!");
            Logger.Log("A player tried to use a command that resulted in a catastrophic failure!", LogType.Error);
            Logger.Log("The command name was " + strName, LogType.Error);
        }

        public void ConsoleUse(string message)
        {
            if (ConsoleSupport)
            {
                Use(message);
            }
            else
            {
                Logger.Log("Sorry, \"" + Name + "\" cannot be used by the console.", LogType.Error);
            }
        }

        public void PlayerUse(Player p, string message)
        {
            if (CanUse(p))
            {
                Use(p, message);
            }
            else
            {
                p.SendMessage("You don't have permission to use /" + Name);
            }
        }

        // Override this
        public virtual void Help(Player p)
        {
            p.SendMessage("This is not a valid help command. Please report what you did to get here to Voziv!");
        }

        // Returns true if the player has access to the command
        public bool CanUse(Player p)
        {
            return (p.Rank >= accessGroup);
        }

        // Returns true if a group has access to the command
        public bool CanUse(GroupEnum g)
        {
            return (g >= accessGroup);
        }

        public static void InitAll()
        {
            // Administrative commands
            all.Add(new CmdBan(CommandGroup.Admin, GroupEnum.Moderator, "ban"));
            all.Add(new CmdBanip(CommandGroup.Admin, GroupEnum.Moderator, "banip"));
            all.Add(new CmdBanned(CommandGroup.Admin, GroupEnum.Operator, "banned"));
            all.Add(new CmdBannedip(CommandGroup.Admin, GroupEnum.Operator, "bannedip"));
            all.Add(new CmdExempt(CommandGroup.Admin, GroupEnum.Moderator, "exempt"));
            all.Add(new CmdFollow(CommandGroup.Admin, GroupEnum.Moderator, "follow"));
            all.Add(new CmdHide(CommandGroup.Admin, GroupEnum.Moderator, "hide"));
            all.Add(new CmdKick(CommandGroup.Admin, GroupEnum.Moderator, "kick"));
            all.Add(new CmdKickban(CommandGroup.Admin, GroupEnum.Moderator, "kickban"));
            all.Add(new CmdMute(CommandGroup.Admin, GroupEnum.Moderator, "mute"));
            all.Add(new CmdReveal(CommandGroup.Admin, GroupEnum.Moderator, "reveal"));
            all.Add(new CmdSetRank(CommandGroup.Admin, GroupEnum.Moderator, "setrank"));
            all.Add(new CmdSpawn(CommandGroup.Admin, GroupEnum.Guest, "spawn"));
            all.Add(new CmdSummon(CommandGroup.Admin, GroupEnum.Moderator, "summon"));
            all.Add(new CmdTp(CommandGroup.Admin, GroupEnum.Builder, "tp"));
            all.Add(new CmdUnban(CommandGroup.Admin, GroupEnum.Moderator, "unban"));
            all.Add(new CmdUnbanip(CommandGroup.Admin, GroupEnum.Moderator, "unbanip"));

            // Bot Related
            all.Add(new CmdBotAdd(CommandGroup.Bot, GroupEnum.Disabled, "botadd")); // Disabled
            all.Add(new CmdBotRemove(CommandGroup.Bot, GroupEnum.Disabled, "botremove")); // Disabled
            all.Add(new CmdBotSummon(CommandGroup.Bot, GroupEnum.Disabled, "botsummon")); // Disabled



            // Chat
            all.Add(new CmdAfk(CommandGroup.Chat, GroupEnum.Guest, "afk"));
            all.Add(new CmdColor(CommandGroup.Chat, GroupEnum.Operator, "color"));
            all.Add(new CmdFakeMsg(CommandGroup.Chat, GroupEnum.Operator, "fakemsg"));
            all.Add(new CmdJoker(CommandGroup.Chat, GroupEnum.Operator, "joker"));
            all.Add(new CmdMe(CommandGroup.Chat, GroupEnum.Guest, "me"));
            all.Add(new CmdOpChat(CommandGroup.Chat, GroupEnum.Moderator, "opchat"));
            all.Add(new CmdSay(CommandGroup.Chat, GroupEnum.Moderator, "say"));
            all.Add(new CmdWhisperChat(CommandGroup.Chat, GroupEnum.Guest, "whisperchat"));

            // Block Commands
            all.Add(new CmdAbort(CommandGroup.Block, GroupEnum.AdvBuilder, "abort"));
            all.Add(new CmdAbout(CommandGroup.Block, GroupEnum.Guest, "about"));
            all.Add(new CmdActiveLava(CommandGroup.Block, GroupEnum.AdvBuilder, "activelava"));
            all.Add(new CmdActiveWater(CommandGroup.Block, GroupEnum.AdvBuilder, "activewater"));
            all.Add(new CmdAid(CommandGroup.Block, GroupEnum.Guest, "aid"));
            all.Add(new CmdBind(CommandGroup.Block, GroupEnum.AdvBuilder, "bind"));
            all.Add(new CmdBuildDoor(CommandGroup.Block, GroupEnum.AdvBuilder, "builddoor"));
            all.Add(new CmdBuildOp(CommandGroup.Block, GroupEnum.Moderator, "buildop"));
            all.Add(new CmdCircle(CommandGroup.Block, GroupEnum.AdvBuilder, "circle"));
            all.Add(new CmdCopy(CommandGroup.Block, GroupEnum.AdvBuilder, "copy"));
            all.Add(new CmdCuboid(CommandGroup.Block, GroupEnum.AdvBuilder, "cuboid"));
            all.Add(new CmdDoor(CommandGroup.Block, GroupEnum.AdvBuilder, "door"));
            all.Add(new CmdDoorView(CommandGroup.Block, GroupEnum.AdvBuilder, "doorview"));
            all.Add(new CmdJail(CommandGroup.Block, GroupEnum.Moderator, "jail"));
            all.Add(new CmdLava(CommandGroup.Block, GroupEnum.Builder, "lava"));
            all.Add(new CmdLock(CommandGroup.Block, GroupEnum.Moderator, "lock"));
            all.Add(new CmdOpView(CommandGroup.Block, GroupEnum.Moderator, "opview"));
            all.Add(new CmdPaint(CommandGroup.Block, GroupEnum.Builder, "paint"));
            all.Add(new CmdPaste(CommandGroup.Block, GroupEnum.AdvBuilder, "paste"));
            all.Add(new CmdReplace(CommandGroup.Block, GroupEnum.AdvBuilder, "replace"));
            all.Add(new CmdSolid(CommandGroup.Block, GroupEnum.Moderator, "solid"));
            all.Add(new CmdTree(CommandGroup.Block, GroupEnum.AdvBuilder, "tree"));
            all.Add(new CmdUnDoor(CommandGroup.Block, GroupEnum.AdvBuilder, "undoor"));
            all.Add(new CmdUnDoorView(CommandGroup.Block, GroupEnum.AdvBuilder, "undoorview"));
            all.Add(new CmdUnlock(CommandGroup.Block, GroupEnum.Moderator, "unlock"));
            all.Add(new CmdUnOpView(CommandGroup.Block, GroupEnum.Moderator, "unopview"));
            all.Add(new CmdWater(CommandGroup.Block, GroupEnum.Builder, "water"));


            // Information commands
            all.Add(new CmdAdmins(CommandGroup.Information, GroupEnum.Guest, "admins"));
            all.Add(new CmdClones(CommandGroup.Information, GroupEnum.Moderator, "clones"));
            all.Add(new CmdHelp(CommandGroup.Information, GroupEnum.Guest, "help"));
            all.Add(new CmdHidden(CommandGroup.Information, GroupEnum.Moderator, "hidden"));
            all.Add(new CmdInfo(CommandGroup.Information, GroupEnum.Guest, "info"));
            all.Add(new CmdLastSeen(CommandGroup.Information, GroupEnum.Disabled, "lastseen")); // Disabled
            all.Add(new CmdMods(CommandGroup.Information, GroupEnum.Guest, "mods"));
            all.Add(new CmdNews(CommandGroup.Information, GroupEnum.Guest, "news"));
            all.Add(new CmdOps(CommandGroup.Information, GroupEnum.Guest, "ops"));
            all.Add(new CmdPlayers(CommandGroup.Information, GroupEnum.Guest, "players"));
            all.Add(new CmdPLevels(CommandGroup.Information, GroupEnum.Disabled, "plevels")); // Disabled
            all.Add(new CmdRatio(CommandGroup.Information, GroupEnum.Moderator, "ratio"));
            all.Add(new CmdRules(CommandGroup.Information, GroupEnum.Banned, "rules"));
            all.Add(new CmdServerReport(CommandGroup.Information, GroupEnum.Operator, "serverreport"));
            all.Add(new CmdTime(CommandGroup.Information, GroupEnum.Guest, "time"));
            all.Add(new CmdWhois(CommandGroup.Information, GroupEnum.Guest, "whois"));
            all.Add(new CmdWhowas(CommandGroup.Information, GroupEnum.Guest, "whowas"));


            // IRC
            all.Add(new CmdResetBot(CommandGroup.IRC, GroupEnum.Operator, "resetbot"));

            // Level Related           
            all.Add(new CmdDeleteLvl(CommandGroup.Level, GroupEnum.Administrator, "deletelvl"));
            all.Add(new CmdGoto(CommandGroup.Level, GroupEnum.Guest, "goto"));            
            all.Add(new CmdLevels(CommandGroup.Level, GroupEnum.Guest, "levels"));
            all.Add(new CmdLoad(CommandGroup.Level, GroupEnum.Moderator, "load"));            
            all.Add(new CmdMapInfo(CommandGroup.Level, GroupEnum.Guest, "mapinfo"));
            all.Add(new CmdNewLvl(CommandGroup.Level, GroupEnum.Moderator, "newlvl"));
            all.Add(new CmdPermissionBuild(CommandGroup.Level, GroupEnum.Operator, "perbuild"));
            all.Add(new CmdPermissionVisit(CommandGroup.Level, GroupEnum.Operator, "pervisit"));
            all.Add(new CmdPhysics(CommandGroup.Level, GroupEnum.Moderator, "physics"));
            all.Add(new CmdRestore(CommandGroup.Level, GroupEnum.Moderator, "restore"));
            all.Add(new CmdSave(CommandGroup.Level, GroupEnum.Moderator, "save"));
            all.Add(new CmdSendLevel(CommandGroup.Level, GroupEnum.Moderator, "sendlevel"));
            all.Add(new CmdSetspawn(CommandGroup.Level, GroupEnum.Moderator, "setspawn"));
            all.Add(new CmdUnload(CommandGroup.Level, GroupEnum.Moderator, "unload"));
        }
    }
}