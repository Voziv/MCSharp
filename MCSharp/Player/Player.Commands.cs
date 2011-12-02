
namespace MCSharp
{
    public sealed partial class Player
    {

        void HandleCommand (string cmd, string message)
        {
            if (cmd.Equals("operators")) { cmd = "ops"; }
            if (cmd.Equals("moderators")) { cmd = "mods"; }
            if (cmd.Equals("z")) { cmd = "cuboid"; }
            if (cmd.Equals("p")) { cmd = "paint"; }
            if (cmd.Equals("r")) { cmd = "replace"; }
            if (cmd.Equals("a")) { cmd = "abort"; }
            if (cmd.Equals("rank")) { cmd = "setrank"; }
            if (cmd.Equals("sendlvl")) { cmd = "sendlevel"; }
            if (cmd.Equals("bd")) { cmd = "builddoor"; }
            if (cmd.Equals("bo")) { cmd = "buildop"; }
            if (cmd.Equals("l")) { cmd = "lock"; }
            if (cmd.Equals("u")) { cmd = "unlock"; }
            if (cmd.Equals("ov")) { cmd = "opview"; }
            if (cmd.Equals("uov")) { cmd = "unopview"; }
            if (cmd.Equals("d")) { cmd = "door"; }
            if (cmd.Equals("ud")) { cmd = "undoor"; }
            if (cmd.Equals("dv")) { cmd = "doorview"; }
            if (cmd.Equals("udv")) { cmd = "undoorview"; }
            if (cmd.Equals("w") || cmd.Equals("whisper")) { cmd = "whisperchat"; }

            Command command = Command.all.Find(cmd);
            if (command != null)
            {
                if (command.CanUse(this))
                {
                    Logger.Log(name + " uses /" + cmd + " " + message, LogType.UserCommand);
                    command.Use(this, message);
                }
                else { SendMessage("You are not allowed to use \"" + cmd + "\"!"); }
            }
            else { SendMessage("Unknown command \"" + cmd + "\"!"); }
        }
        
    }
}
