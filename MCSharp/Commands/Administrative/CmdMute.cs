using System;

namespace MCSharp
{
    public class CmdMute : Command
    {
        
        // Constructor
        public CmdMute(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/mute <player> - Toggles Mute on a player so they cannot chat");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (message != "")
            {
                Player target = Player.Find(message);

                if (target != null)
                {
                    if (p != target)
                    {
                        if (p.Rank > target.Rank)
                        {
                            // Toggle the mute variable
                            target.isMuted = !target.isMuted;

                            // Send messaged based on mute status
                            if (target.isMuted)
                            {
                                Player.GlobalMessageOps("-" + p.color + p.name + "&e just muted " + target.color + target.name);
                                target.SendMessage("You have been muted!");
                            }
                            else
                            {
                                Player.GlobalMessageOps("-" + p.color + p.name + "&e just un-muted " + target.color + target.name);
                                target.SendMessage("You are un-muted and clear to chat now!");
                            }
                        }
                        else
                        {
                            p.SendMessage("You can't use mute on someone of equal or higher rank!");
                        }
                    }
                    else
                    {
                        p.SendMessage("You can't mute yourself!");
                    }
                }
                else
                {
                    p.SendMessage("There is no such player!");
                }
            }
            else
            {
                Help(p);
            }
        }
    }
}
