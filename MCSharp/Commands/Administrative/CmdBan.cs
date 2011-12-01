using System;

namespace MCSharp
{
    public class CmdBan : Command
    {
        // Constructor
        public CmdBan(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command Usage Help
        public override void Help(Player p)
        {
            p.SendMessage("/ban <player> - Bans a player without kicking him.");
            p.SendMessage("Add # before name to stealth ban.");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (message == "")
            {
                Help(p);
            }
            else
            {
                bool stealth = false;
                if (message[0] == '#')
                {
                    message = message.Remove(0, 1).Trim();
                    stealth = true;
                    Logger.Log("Stealth Ban Atempted");
                }

                // Ensure the name is valid
                if (Player.ValidName(message))
                {
                    // Ensure the player isn't banned already
                    if (Player.GetRank(message) != GroupEnum.Banned)
                    {
                        if (p.Rank > Player.GetRank(message))
                        {

                            // Check to see if the player is online
                            // Send appropriate message based on status and stealth option
                            if (Player.IsOnline(message))
                            {
                                if (stealth)
                                {

                                }
                                else
                                {
                                    Player.GlobalMessage("[Server]:" + p.color + p.name + "&e has banned " + message);
                                }
                            }
                            else
                            {
                                Player.GlobalMessage("[Server]:" + p.color + p.name + "&e has banned " + message + "(offline)");
                            }

                            // Actually get around to banning the player
                            Player.Ban(message);
                            
                            IRCBot.Say(message + " was banned");
                        }
                        else
                        {
                            p.SendMessage("You can't ban someone of equal or higher rank!");
                        }
                    }
                    else
                    {
                        p.SendMessage(message + " is already banned.");
                    }
                }
                else
                {
                    p.SendMessage("Invalid name \"" + message + "\".");
                }
            }
        }
    }
}