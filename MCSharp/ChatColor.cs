using System;
using System.Collections;
using System.Collections.Generic;

namespace MCSharp {
    
	public static class ChatColor {
		public const string Black = "&0";
		public const string Navy = "&1";
		public const string Green = "&2";
		public const string Teal = "&3";
		public const string Maroon = "&4";
		public const string Purple = "&5";
		public const string Gold = "&6";
		public const string Silver = "&7";
		public const string Gray = "&8";
		public const string Blue = "&9";
		public const string Lime = "&a";
		public const string Aqua = "&b";
		public const string Red = "&c";
		public const string Pink = "&d";
		public const string Yellow = "&e";
		public const string White = "&f";

		public static string Parse(string str) 
        {
            string strColor = "";
            switch (str.ToLower()) {
                case "black": strColor = Black; break;
                case "navy": strColor = Navy; break;
                case "green": strColor = Green; break;
                case "teal": strColor = Teal; break;
                case "maroon": strColor = Maroon; break;
                case "purple": strColor = Purple; break;
                case "gold": strColor = Gold; break;
                case "silver": strColor = Silver; break;
                case "gray": strColor = Gray; break;
                case "blue": strColor = Blue; break;
                case "lime": strColor = Lime; break;
                case "aqua": strColor = Aqua; break;
                case "red": strColor = Red; break;
                case "pink": strColor = Pink; break;
                case "yellow": strColor = Yellow; break;
                case "white": strColor = White; break;
			}
            return strColor;
		}

        public static string Name(string str) {
            string strColorName = "";
			switch (str) {
                case Black: strColorName = "black"; break;
                case Navy: strColorName = "navy"; break;
                case Green: strColorName = "green"; break;
                case Teal: strColorName = "teal"; break;
                case Maroon: strColorName = "maroon"; break;
                case Purple: strColorName = "purple"; break;
                case Gold: strColorName = "gold"; break;
                case Silver: strColorName = "silver"; break;
                case Gray: strColorName = "gray"; break;
                case Blue: strColorName = "blue"; break;
                case Lime: strColorName = "lime"; break;
                case Aqua: strColorName = "aqua"; break;
                case Red: strColorName = "red"; break;
                case Pink: strColorName = "pink"; break;
                case Yellow: strColorName = "yellow"; break;
                case White: strColorName = "white"; break;
			}
            return strColorName;
		}
	}
}