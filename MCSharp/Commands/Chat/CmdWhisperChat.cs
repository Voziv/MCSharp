using System;

namespace MCSharp
{
    public class CmdWhisperChat : Command
    {
        // Constructor
        public CmdWhisperChat(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/whisper <name> - Enables whisper chat with a player.");
            p.SendMessage("Blank or invalid name disables whisper chat");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (message == "") // blank name, disable whispering
            {
                if (p.isWhisperChat)
                {
                    p.SendMessage("No longer whispering: " + p.whisperTarget + ".");
                    p.isWhisperChat = false;
                }
                else
                {
                    p.SendMessage("Whisper chat is already disabled.");
                }
            }
            else if (p.isOpChat)
            {
                p.SendMessage("Disable opchat before using /whisper");
            }
            else
            {
                p.isWhisperChat = false;
                Player target = Player.Find(message);
                if (target != null)
                {
                    p.whisperTarget = target.name;
                    p.isWhisperChat = true;
                    p.SendMessage("Now whispering: " + target.color + target.name + "&e.");
                }
                else
                {
                    p.SendMessage(LanguageString.NoSuchPlayer);
                }
            }
        }
    }
}