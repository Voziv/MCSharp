using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.ComponentModel;


namespace MCSharp
{
	class AutoSaver
	{
		static int _interval;
		

		static int count = 1;
		public AutoSaver(int interval)
		{
			_interval = interval * 1000;

			System.Timers.Timer runner = new System.Timers.Timer(_interval);
			runner.Elapsed += delegate
			{
				Exec();
			};
			Exec();
			runner.Start();
		}

		static void Exec()
		{
				Run();
		}

		static void Run()
		{

			try
			{
				count--;

				Server.levels.ForEach(delegate(Level l)
				{
					try
					{
                        // Attempt a save on the level
                        l.Save();

                        // Every so often try and do a backup
                        if (count == 0)
                        {
                            l.Backup();
                        }

                        // Unload the level if it's not main
                        if (l.name != "main")
                        {
                            int intPlayers = 0;
                            foreach (Player p in Player.players)
                            {
                                if (p.level == l)
                                    intPlayers++;
                            }

                            // Unload the level if players is 0 for more than 3 times
                            if (intPlayers == 0 && Properties.AutoUnloadEnabled == true)
                            {
                                l.emptyCount++;
                                if (l.emptyCount > 3)
                                {
                                    Command.all.Find("unload").Use(l.name);
                                }
                            }
                            else
                            {
                                l.emptyCount = 0;
                            }
                        }
					}
					catch (Exception e)
					{
						Logger.Log("Backup for " + l.name + " has caused an error.", LogType.Error);
                        Logger.Log(e.Message, LogType.ErrorMessage);
					}
                    
				});

				if (count <= 0)
				{
					count = 3;
				}
			}
			catch (Exception e)
            {
                Logger.Log("Error during autosaver", LogType.Error);
                Logger.Log(e.Message, LogType.ErrorMessage);
            }
		}
	}
}
