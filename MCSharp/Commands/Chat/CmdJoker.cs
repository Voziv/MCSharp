using System;
using System.IO;

namespace MCSharp
{
    public class CmdJoker : Command
    {
        // Constructor
        public CmdJoker(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/joker <player> - Replaces user messages with funny messages.");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (message != "")
            {
                if (message == "reload")
                {
                    Server.jokerMessages.Clear();
                    try
                    {
                        if (File.Exists("joker.txt"))
                        {
                            StreamReader r = File.OpenText("joker.txt");
                            while (!r.EndOfStream)
                                Server.jokerMessages.Add(r.ReadLine());
                        }
                        else
                            File.Create("joker.txt").Close();

                        p.SendMessage("Joker Messages Reloaded!");
                    }
                    catch
                    {
                        p.SendMessage("FAILED! Joker Messages NOT loaded!");
                    }
                }
                else
                {
                    if (Server.jokerMessages.Count > 0)
                    {
                        Player target = Player.Find(message);
                        if (target != null)
                        {
                            if (target != p)
                            {
                                if (!target.isJoker && p.Rank > target.Rank)
                                {
                                    target.isJoker = true;
                                    p.SendMessage("*" + target.color + target.name + "&e is now a joker!");
                                    target.SendMessage("You are such a jokester!");
                                }
                                else
                                {
                                    target.isJoker = false;
                                    p.SendMessage("*" + target.color + target.name + "&e is no longer a joker.");
                                    target.SendMessage("Why so serious?! You are offically no longer funny!");
                                }
                            }
                            else
                            {
                                p.SendMessage("You can't joker yourself!");
                            }
                        }
                        else
                        {
                            p.SendMessage("Error! Player not found!");

                        }
                    }
                    else
                    {
                        p.SendMessage("Failed! No messages set up! Check joker.txt");
                    }
                }
            }
            else
            { 
                Help(p);
            }
        }
    }
}
