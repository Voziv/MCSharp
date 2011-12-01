using System;
using System.IO;
using System.Collections.Generic;

namespace MCSharp
{
    public sealed class PlayerList
    {
        // Properties
        public string Filename { get { return strFilename; } }
        
        // Variables
        List<string> players = new List<string>();
        string strFilename;

        // Constructor
        public PlayerList(string f)
        {
            strFilename = f;
            Load();
        }
        
        // Add a player to the list
        public void Add(string p)
        {
            // No point in adding the player twice
            if (!Contains(p))
            {
                players.Add(p.ToLower());
                Save();
            }
        }
        
        // Remove a player from the list
        public void Remove(string p)
        {
            if (Contains(p))
            {
                players.Remove(p.ToLower());
                Save();
            }
        }
        
        // Check to see if a player is in the list
        public bool Contains(string p) {
            if (p != null)
            {
                return players.Contains(p.ToLower());
            }
            return false;
        }
        
        // Return all the players
        public List<string> All() {
            return new List<string>(players);
        }
        
        // Save the rank file
        private void Save()
        {
            StreamWriter file = File.CreateText("ranks/" + strFilename);
            players.ForEach(delegate(string p) { file.WriteLine(p); });
            file.Close();
            Logger.Log("SAVED: ranks/" + strFilename);
        }

        private void Load()
        {
            string path = "ranks/" + strFilename;
            if (!Directory.Exists("ranks"))
            {
                Directory.CreateDirectory("ranks");
            }
            if (File.Exists(path))
            {
                Logger.Log("LOADED: " + path);
                foreach (string line in File.ReadAllLines(path)) { players.Add(line); }
            }
            else 
            { 
                File.Create(path).Close();
                Logger.Log("CREATED NEW: " + path);
            }
        }
    }
}