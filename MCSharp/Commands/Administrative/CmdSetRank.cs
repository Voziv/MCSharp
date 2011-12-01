using System;

namespace MCSharp
{
    public class CmdSetRank : Command
    {

        // Constructor
        public CmdSetRank(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }
        
        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/setrank <player> <rank> - Sets or returns a players rank.");
            p.SendMessage("You may use /rank as a shortcut");
            p.SendMessage("Valid Ranks are: guest, builder, adv, advbuilder, mod, moderator, op, operator");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {      
            int number = message.Split(' ').Length;
            if (number > 2) {  }

            if (number == 2) // Change the players rank
            {
                
                // Seperate message string
                int pos = message.IndexOf(' ');
                string tempName = message.Substring(0, pos).ToLower();
                string newRankmsg = message.Substring(pos + 1).ToLower();

                Player target = Player.Find(tempName);
                bool validRank = true;
                GroupEnum rank = GroupEnum.Null;

                // Ensure we catch a valid rank
                switch (newRankmsg)
                {
                    case "operator":
                    case "op":
                        rank = GroupEnum.Operator;
                        break;
                    case "moderator":
                    case "mod":
                        rank = GroupEnum.Moderator;
                        break;
                    case "advbuilder":
                    case "adv":
                        rank = GroupEnum.AdvBuilder;
                        break;
                    case "builder":
                        rank = GroupEnum.Builder;
                        break;
                    case "guest":
                        rank = GroupEnum.Guest;
                        break;
                    default:
                        validRank = false;
                        break;
                }
                // Make sure the rank is valid
                if (validRank)
                {
                    // Validate target players name
                    if (Player.ValidName(tempName))
                    {
                        // Can't set your own rank
                        if (p.name != tempName)
                        {
                            // Player must be a lower rank than yourself
                            if (p.Rank > Player.GetRank(tempName))
                            {
                                // Cannot set a banned player's rank
                                if (Player.GetRank(tempName) != GroupEnum.Banned)
                                {

                                    if (rank < p.Rank)
                                    {

                                        if (target != null)
                                        {
                                            Player.GlobalMessage("[Server]: " + target.color + target.name + "&e is now a " + Group.Find(rank).Color + Group.Find(rank).Name);
                                            Player.ChangeRank(target, rank);

                                            target.SendMessage("You are now ranked " + target.group.Color + target.group.Name + "&e, type /help for your new set of commands.");
                                        }
                                        else
                                        {
                                            Player.GlobalMessage("[Server]: " + tempName + " &f(offline)&e is now a " + Group.Find(rank).Color + Group.Find(rank).Name);
                                            Player.ChangeRank(tempName, rank);
                                        }
                                        Logger.Log(tempName + " was set to " + rank + " by " + p.name);
                                    }
                                    else
                                    {
                                        p.SendMessage("You cannot set someone to the same or higher rank than you!");
                                    }

                                }
                                else
                                {
                                    p.SendMessage("You must unban this player before you can change his rank!");
                                }
                            }
                            else
                            {
                                p.SendMessage("You cannot change the rank of someone who is higher rank than you!");
                            }
                        }
                        else
                        {
                            p.SendMessage("You cannot change your own rank");
                        }
                    }
                    else
                    {
                        p.SendMessage("Invalid name \"" + message + "\".");
                    }
                }
                else
                {
                    p.SendMessage("The rank \"" + newRankmsg + "\" is invalid!");
                }
            }
            else if (message != "") // Return the players current rank
            {
                if (Player.ValidName(message))
                {
                    Group rank = MCSharp.Group.Find(Player.GetRank(message));
                    p.SendMessage(message + "'s rank is: " + rank.Color + rank.Name);
                }
            }
            else // Return usage
            {
                Help(p);
            }
        }
    }
}