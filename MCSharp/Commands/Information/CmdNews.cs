using System;
using System.IO;
using System.Collections.Generic;

namespace MCSharp
{
    public class CmdNews : Command
    {
        // Constructor
        public CmdNews(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/news - Displays the latest changes and additions");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (message == "") 
            {
                try
                {
                    if (File.Exists("news.txt"))
                    {
                        List<string> news = new List<string>();
                        StreamReader wm = File.OpenText("news.txt");
                        while (!wm.EndOfStream)
                            news.Add(wm.ReadLine());

                        wm.Close();

                        foreach (string w in news)
                            p.SendMessage(w);
                    }
                    else
                    {
                        p.SendMessage("No news today!");
                    }
                }
                catch
                {
                    Logger.Log("Error reading news.txt");
                }
            }
            else
            { 
                Help(p); 
            }
        }
    }
}
