using System;
using System.Collections.Generic;
using System.Text;

using System.IO;

namespace MCSharp.Scripts
{
    class BotScript
    {
        public static List<Variable> variables = new List<Variable>();
        public static List<Function> functions = new List<Function>();

        public BotScript(string filename)
        {
            List<string> lines = new List<string>(File.ReadAllLines(filename));

            foreach (string s in lines)
            {
                string key = s.Split(':')[0];
                string value = s.Split(':')[1].Trim();

                switch (key)
                {
                    case "INT":
                        variables.Add(new Variable(value.Split('=')[0].Trim(),value.Split('=')[1].Trim(),"int"));
                        break;
                    case "BOOL":
                        variables.Add(new Variable(value.Split('=')[0].Trim(),value.Split('=')[1].Trim(),"bool"));
                        break;
                    case "FUNCTION":
                        break;
                        


                }
            }
        }
    }

    class Function
    {
        List<string> commands = new List<string>();

        public Function(List<string> commands)
        {
            this.commands = commands;
        }

        public bool Execute()
        {
            try
            {
                foreach (string s in commands)
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, LogType.ErrorMessage);
                return false;
            }
        }
    }

    class Variable
    {
        string name, value, type;
        public Variable(string n, string v, string t)
        {
            name = n;
            value = v;
            type = t;
        }
        public object GetValue()
        {
            switch (type)
            {
                case "int":
                    return Convert.ToInt32(value);
                case "bool":
                    return (value.ToLower() == "true") ? true : false;
                case "byte":
                    return Convert.ToByte(value);
                default:
                    return null;
            }
        }
    }
}
