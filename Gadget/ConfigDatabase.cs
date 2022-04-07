using System.Reflection;
using Microsoft.Data.Sqlite;

namespace Gadget
{
    class Database
    {
        public static bool TestPing()
        {
            bool success = false;
            string cs = @"Data Source=" + GetDatabasePath();

            try
            {
                using var con = new SqliteConnection(cs);
                con.Open();

                success = true;
            }
            catch (SqliteException e)
            {
                Logger.Write(Discord.LogSeverity.Error, "", e);
            }

            return success;
        }

        public static bool retrieve(string tableName, string columnName, string guildId, out string output)
        {
            output = "";
            bool success = false;
            string cs = @"Data Source=" + GetDatabasePath();
            string cmdTxt = "SELECT " + columnName + " FROM " + tableName + " WHERE guildId = @guildId";

            try
            {
                using var con = new SqliteConnection(cs);
                con.Open();

                using var command = new SqliteCommand(cmdTxt, con);
                command.Parameters.AddWithValue("@guildId", guildId);

                command.Prepare();

                using SqliteDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    output = reader.GetString(0);
                    success = true;
                }
                else
                {
                    Logger.Write(Discord.LogSeverity.Warning, $"Was unable to sucessfully retrieve {columnName} from {tableName} at guildId {guildId}");
                }
            }
            catch (SqliteException e)
            {
                Logger.Write(Discord.LogSeverity.Error, "", e);
            }

            return success;
        }

        private static string GetDatabasePath()
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "", @"Data\gadget.db");
        }
    }
}