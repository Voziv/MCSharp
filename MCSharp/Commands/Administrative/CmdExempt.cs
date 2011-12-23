using System;

namespace MCSharp
{
    public class CmdExempt : Command
    {

        // Constructor
        public CmdExempt(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/exempt <player> - Adds or removes players from grief exemption");
            p.SendMessage("Players exempted from grief will not be kicked for clicking fast");

        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (message != "") // Exempt or remove from exempt
            {

                // Seperate message string
                //int pos = message.IndexOf(' ');
                string tempName = message.ToLower();

                // If the name is in the file, remove it
                if (Server.griefExempted.Contains(tempName))
                {
                    Server.griefExempted.Remove(tempName);
                    p.SendMessage(tempName + " was " + ChatColor.Red + "removed" + ChatColor.Yellow + " from anti-grief exemption");
                }
                else
                {
                    Server.griefExempted.Add(tempName);
                    p.SendMessage(tempName + " was " + ChatColor.Green + "added" + ChatColor.Yellow + " to anti-grief exemption");
                }

            }
            else // Return usage
            {
                Help(p);
            }
        }
    }
}