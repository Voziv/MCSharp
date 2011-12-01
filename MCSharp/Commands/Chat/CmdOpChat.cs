using System;

namespace MCSharp
{
    public class CmdOpChat : Command
    {
        // Constructor
        public CmdOpChat(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/opchat - Enables or disables operator chat mode.");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (message == "") 
            {
                if (p.isWhisperChat)
                {
                    p.SendMessage("Disable whisper chat before using /opchat"); // Think of maybe disabling it automatically?
                }
                else
                {
                    p.isOpChat = !p.isOpChat;
                    if (p.isOpChat)
                    {
                        p.SendMessage("Operator chat: &aON&e.");
                    }
                    else
                    {
                        p.SendMessage("Operator chat: &cOFF&e.");
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
