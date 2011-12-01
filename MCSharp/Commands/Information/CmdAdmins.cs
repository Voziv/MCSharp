using System;
using System.Collections.Generic;
using System.Text;

namespace MCSharp
{
    public class CmdAdmins : Command
    {
        // Constructor
        public CmdAdmins(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help 
        public override void Help(Player p)
        {
            p.SendMessage("/admins - List the admins of the server");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (message == "")
            {
                string strOperators, strModerators, strAdministrator;
                strAdministrator = Properties.ServerAdministrator;
                strOperators = "";
                strModerators = "";

                // Operators
                Server.operators.All().ForEach(delegate(string name)
                {
                    strOperators += name + ", ";
                });
                if (strOperators == "")
                {
                    strOperators = "No Operators";
                }
                else
                {
                    strOperators = strOperators.Remove(strOperators.Length - 2);
                }

                // Moderators
                Server.moderators.All().ForEach(delegate(string name)
                {
                    strModerators += name + ", ";
                });
                if (strModerators == "")
                {
                    strModerators = "No Moderators";
                }
                else
                {
                    strModerators = strModerators.Remove(strModerators.Length - 2);
                }



                // Send out the message
                p.SendMessage("Server Administrator: " + strAdministrator);
                p.SendMessage("Operator Team: " + strOperators);
                p.SendMessage("Moderator Team: " + strModerators);
            }
            else
            { 
                Help(p);
            }
        }
    }
}
