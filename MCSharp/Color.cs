using System;

namespace MCSharp {
	public static class c {
		public const string black = "&0";
		public const string navy = "&1";
		public const string green = "&2";
		public const string teal = "&3";
		public const string maroon = "&4";
		public const string purple = "&5";
		public const string gold = "&6";
		public const string silver = "&7";
		public const string gray = "&8";
		public const string blue = "&9";
		public const string lime = "&a";
		public const string aqua = "&b";
		public const string red = "&c";
		public const string pink = "&d";
		public const string yellow = "&e";
		public const string white = "&f";

		public static string Parse(string str) {
            string strColor = "";
            switch (str.ToLower()) {
                case "black": strColor = black; break;
                case "navy": strColor = navy; break;
                case "green": strColor = green; break;
                case "teal": strColor = teal; break;
                case "maroon": strColor = maroon; break;
                case "purple": strColor = purple; break;
                case "gold": strColor = gold; break;
                case "silver": strColor = silver; break;
                case "gray": strColor = gray; break;
                case "blue": strColor = blue; break;
                case "lime": strColor = lime; break;
                case "aqua": strColor = aqua; break;
                case "red": strColor = red; break;
                case "pink": strColor = pink; break;
                case "yellow": strColor = yellow; break;
                case "white": strColor = white; break;
			}
            return strColor;
		} public static string Name(string str) {
            string strColorName = "";
			switch (str) {
                case black: strColorName = "black"; break;
                case navy: strColorName = "navy"; break;
                case green: strColorName = "green"; break;
                case teal: strColorName = "teal"; break;
                case maroon: strColorName = "maroon"; break;
                case purple: strColorName = "purple"; break;
                case gold: strColorName = "gold"; break;
                case silver: strColorName = "silver"; break;
                case gray: strColorName = "gray"; break;
                case blue: strColorName = "blue"; break;
                case lime: strColorName = "lime"; break;
                case aqua: strColorName = "aqua"; break;
                case red: strColorName = "red"; break;
                case pink: strColorName = "pink"; break;
                case yellow: strColorName = "yellow"; break;
                case white: strColorName = "white"; break;
			}
            return strColorName;
		}
	}
}