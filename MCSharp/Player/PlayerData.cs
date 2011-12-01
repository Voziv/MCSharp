using System;
using System.Collections.Generic;
using System.Text;

using System.Data.SQLite;

namespace Minecraft_Server
{
	class PlayerData
	{
		/*
		 * player:
		 * pid (primary)
		 * name
		 * ip
		 * rank
		 * last online
		 * total playtime
		 * 
		 * reports: 
		 * rid (primary)
		 * pid (foreign)
		 * report
		 * 
		 * history:
		 * hid (primary)
		 * pid (foreign)
		 * history (events like log in/out)
		 * 
		 */
		static SQLiteConnection cnn;

		public PlayerData()
		{
			cnn = new SQLiteConnection("Data Source=player.data;Version=3;");
			cnn.Open();
		}
		~PlayerData()
		{
			cnn.Clone();
		}
		public static bool playerUpdate(string player, byte type, string param)
		{
			using (SQLiteCommand cmd = cnn.CreateCommand())
			{
				cmd.CommandText = @"UPDATE [player] SET [Value] = [Value] + 1
                      WHERE [Customer] LIKE @lookupValue";
				SQLiteParameter lookupValue = new SQLiteParameter("@lookupValue");
				cmd.Parameters.Add(lookupValue);

				for (int i = 0; i < 100; i++)
				{
					//lookupValue.Value = getSomeLookupValue(i);
					cmd.ExecuteNonQuery();

				}
			}
			/*
			 * using (DbTransaction dbTrans = cnn.BeginTransaction())

  {

    using (DbCommand cmd = cnn.CreateCommand())

    {

      cmd.CommandText = "INSERT INTO TestCase(MyValue) VALUES(?)";

      DbParameter Field1 = cmd.CreateParameter();

      cmd.Parameters.Add(Field1);

      for (int n = 0; n < 100000; n++)

      {

        Field1.Value = n + 100000;

        cmd.ExecuteNonQuery();

      }

    }

    dbTrans.Commit();

  }
			 */
			return false;
		}


	}
}
