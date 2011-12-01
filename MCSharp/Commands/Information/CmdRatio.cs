using System;
using System.Collections.Generic;
using System.Text;

namespace MCSharp
{
    public class CmdRatio : Command
    {
        // Constructor
        public CmdRatio(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/ratio <name> - Displays the block ratio of a player (Placed/Removed)");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (message != "")
            {
                Player target = Player.Find(message);
                if (target != null)
                {
                    if (target.placedBlocks > 0 && target.deletedBlocks > 0)
                    {
                        double ratio = ((double)target.placedBlocks / (double)target.deletedBlocks);
                        p.SendMessage(target.name + ": (" + formatRatio(target) + ") (" + target.placedBlocks + "/" + target.deletedBlocks + ")" + " (" + ratio.ToString("N4") + ")");
                    }
                    else
                    {
                        p.SendMessage(target.name + " has not placed/deleted any bricks!");
                    }
                }
                else
                {
                    p.SendMessage(LanguageString.NoSuchPlayer);
                }
            }
            else
            {
                Help(p);
            }
        }

        /// <summary>
        /// Calculates the greatest common divisor for two numbers
        /// </summary>
        /// <param name="a">First Number</param>
        /// <param name="b">Second Number</param>
        /// <returns>The greatest common divisor for the two numbers</returns>
        private int GCD(int a, int b)
        {
            int gcd = 1;
            while (a != 0 && b != 0)
            {
                if (a > b)
                    a %= b;
                else
                    b %= a;
            }
            if (a == 0)
                gcd = b;
            else
                gcd = a;

            return gcd;
        }

        // Formats a ratio string for a player
        private string formatRatio(Player target)
        {
            var gcd = GCD(target.placedBlocks, target.deletedBlocks);
            return string.Format("{0}:{1}", target.placedBlocks / gcd, target.deletedBlocks / gcd);
        }
    }
}
