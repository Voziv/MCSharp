// This is a commit to generate a test
// This is the 2nd test
using System;
using System.Collections.Generic;

namespace MCSharp 
{
	public class CmdHelp : Command 
    {
		// Constructor
        public CmdHelp(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/help [command] - Shows a list of commands or more detail for a specific command.");
        }

        // Code to run when used by a player
		public override void Use(Player p, string message)  
        {
			message.ToLower();
            int intCounter;
            int[] intCategoryValues = (int[])Enum.GetValues(typeof(CommandGroup));
            string strColor = "";
            string[] strCategories = Enum.GetNames(typeof(CommandGroup));
            bool[] blnHasCommands = new bool[intCategoryValues.Length];

            List<Command> cmdList = Command.all.All();

            intCounter = 0;
            foreach (string category in strCategories)
            {
                
                cmdList.ForEach(delegate(Command cmd)
                {
                    if (Enum.GetName(typeof(CommandGroup), cmd.Category) == category)
                    {
                        if (cmd.CanUse(p))
                        {
                            blnHasCommands[intCounter] = true;
                        }
                    }
                    
                });
                intCounter++;
            }
            if (message == "") 
            {
                p.SendMessage("---Welcome to the help system---");
                p.SendMessage("Any &4red &e categories can be browsed using /help <&4number&e>");
                p.SendMessage("Use /help all to see available commands for your rank");
                intCounter = 0;
                foreach (string category in strCategories)
                {
                    strColor = (blnHasCommands[intCounter]) ? "&4 " : "&e ";
                    intCounter++;
                    p.SendMessage(intCounter + ". " + strColor + category + "&e commands");
                }


                /*
                string cmdName = "No Commands Parsed Yet";
                bool cmdFailed = false;
                p.group.commands.All().ForEach(delegate(Command cmd) 
                {
                    try
                    {
                        if (!cmdFailed)
                        {
                            message += ", " + cmd.Name;
                            cmdName = cmd.Name;
                        }
                    }
                    catch
                    {
                        message = "  /help Failed just after " + cmdName;
                        cmdFailed = true;
                    }
                    
                });
				p.SendMessage("Available commands: " + message.Remove(0,2)+". For more info about a specific command write \"/help <command>\".");
                 * */
			}
            else if (message == "all")
            {
                string allAvailableCommands = "";
                cmdList.ForEach(delegate(Command cmd)
                {
                    if (cmd.CanUse(p))
                    {
                        allAvailableCommands += cmd.Name + ", ";
                    }
                });
                p.SendMessage("Commands available for " + p.group.Color + p.group.Name + "s" + c.yellow + ": ");
                allAvailableCommands = allAvailableCommands.Remove(allAvailableCommands.Length - 2);
                p.SendMessage(allAvailableCommands);
            }
            else
            {
                int messageint = -1;
                try
                {
                    messageint = Convert.ToInt16(message);
                }
                catch { } // Do nothing on failure

                if (messageint > 0 && messageint <= strCategories.Length)
                {
                    messageint--;
                    string commands = "";
                    if (blnHasCommands[messageint])
                    {
                        cmdList.ForEach(delegate(Command cmd)
                        {
                            if (cmd.Category == (CommandGroup)Enum.Parse(typeof(CommandGroup), messageint.ToString()))
                            {
                                if (cmd.CanUse(p))
                                {
                                    commands += cmd.Name + ", ";
                                }
                            }
                        });
                        p.SendMessage("Commands available for &4" + strCategories[messageint] + " &e: ");
                        commands = commands.Remove(commands.Length - 2);
                        p.SendMessage(commands);
                    }
                    else
                    {
                        p.SendMessage("Sorry there are no commands in this category that you can use.");
                    }
                }
                else
                {
                    Command cmd = Command.all.Find(message);
                    if (cmd == null)
                    {
                        p.SendMessage("There is no command called \"" + message + "\"");
                    }
                    else
                    {
                        cmd.Help(p);
                    }
                }


            }
		} 
	}
}