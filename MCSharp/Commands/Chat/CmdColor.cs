using System;

namespace MCSharp
{
    public class CmdColor : Command
    {
        // Constructor
        public CmdColor (CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help (Player p)
        {
            p.SendMessage("/color [player] <color> - Changes the nick color.");
            p.SendChat(p, "&0black &1navy &2green &3teal &4maroon &5purple &6gold &7silver");
            p.SendChat(p, "&8gray &9blue &alime &baqua &cred &dpink &eyellow &fwhite");
        }

        // Code to run when used by a player
        public override void Use (Player p, string message)
        {
            //p.SendMessage("Sorry this command is temporarily disabled");
            
            if (message != "" && message.Split(' ').Length <= 2)
            {
                int pos = message.IndexOf(' ');
                if (pos != -1)
                {
                    Player target = Player.Find(message.Substring(0, pos));
                    if (target == null)
                    {
                        p.SendMessage("There is no player \"" + message.Substring(0, pos) + "\"!");
                    }
                    else
                    {

                        string color = ChatColor.Parse(message.Substring(pos + 1));
                        if (color == "")
                        {
                            p.SendMessage("There is no color \"" + message + "\".");
                        }
                        else if (color == target.color)
                        {
                            p.SendMessage(target.name + " already has that color.");
                        }
                        else
                        {
                            Player.GlobalChat(target, target.color + "*" + FormatName(target.name) + " color changed to " + color + ChatColor.Name(color) + "&e.", false);
                            target.color = color;

                            Player.GlobalDie(target, false);
                            Player.GlobalSpawn(target, target.pos[0], target.pos[1], target.pos[2], target.rot[0], target.rot[1], false);
                        }
                    }
                }
                else
                {
                    string color = ChatColor.Parse(message);
                    if (color == "") { p.SendMessage("There is no color \"" + message + "\"."); }
                    else if (color == p.color) { p.SendMessage("You already have that color."); }
                    else
                    {
                        Player.GlobalChat(p, p.color + "*" + FormatName(p.name) +
                                          " color changed to " + color +
                                          ChatColor.Name(color) + "&e.", false);
                        p.color = color; Player.GlobalDie(p, false);
                        Player.GlobalSpawn(p, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1], false);
                    }
                }
            }
            else
            {
                Help(p);
            }
        }


        /// <summary>
        /// Formats the name with the new color
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        static string FormatName (string name)
        {
            string ch = name[name.Length - 1].ToString().ToLower();
            if (ch == "s" || ch == "x") { return name + "&e'"; }
            else { return name + "&e's"; }
        }
    }
}