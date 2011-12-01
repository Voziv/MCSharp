using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MCSharp
{
    class CmdRules : Command
    {
        // Constructor
        public CmdRules(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }
        
        // Command usage help
        public override void Help(Player p)
        {
            if (p.Rank < GroupEnum.Moderator)
            {
                p.SendMessage("/rules - Displays server rules");
            }
            else
            {
                p.SendMessage("/rules [player]- Displays server rules to a player");
            }
        }
        
        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (File.Exists("rules.txt"))
            {

            }
            else
            {
                p.SendMessage("There is no rules.txt file to show.");
            }

            if (message != "")
            {
                if (p.Rank >= GroupEnum.Moderator)
                {
                    Player target = Player.Find(message);
                    if (target != null)
                    {
                        // Send the rules to the target player
                        SendRules(target);
                    }
                    else
                    {
                        p.SendMessage(LanguageString.NoSuchPlayer);
                    }
                }
                else
                {
                    p.SendMessage("You don't have permission to send /rules to another player!");
                }
            }
            else
            {
                // Send the rules to the player
                SendRules(p);
            }
        }

        /// <summary>
        /// Sends the rules to a target player
        /// </summary>
        /// <param name="target">Target player that rules are being sent to.</param>
        private void SendRules(Player target)
        {
            try
            {
                if (target.level == Server.mainLevel && Server.mainLevel.permissionbuild == LevelPermission.Guest)
                {
                    target.SendMessage("You are currently on the guest map where anyone can build");
                }

                List<string> rules = new List<string>();

                // Read in rules
                StreamReader r = File.OpenText("rules.txt");
                while (!r.EndOfStream)
                    rules.Add(r.ReadLine());
                r.Close();

                // Send the user the rules
                target.SendMessage("Server Rules:");
                foreach (string rule in rules)
                    target.SendMessage(rule);

            }
            catch
            {
                Logger.Log("Error reading rules.txt in SendRules()", LogType.Error);
            }
        }
    }
}
