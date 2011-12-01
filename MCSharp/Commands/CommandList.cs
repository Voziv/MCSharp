using System;
using System.Collections.Generic;

namespace MCSharp
{
    public sealed class CommandList
    {
        List<Command> commands = new List<Command>();
        
        public CommandList() { }

        public void Add(Command cmd)
        {
            commands.Add(cmd);
        }
        
        public bool Remove(Command cmd)
        {
            return commands.Remove(cmd);
        }
        
        public bool Contains(Command cmd)
        {
            return commands.Contains(cmd);
        }
        
        public bool Contains(string name)
        {
            name = name.ToLower();
            foreach (Command cmd in commands)
            {
                if (cmd.Name == name.ToLower())
                {
                    return true;
                }
            }
            return false;
        }
        public Command Find(string name)
        {
            name = name.ToLower();
            foreach (Command cmd in commands)
            {
                if (cmd.Name == name.ToLower())
                {
                    return cmd; 
                }
            }
            return null;
        } 

        public List<Command> All() 
        {
            return new List<Command>(commands);
        }
    }
}