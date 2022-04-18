using System.Reflection;
using Microsoft.Data.Sqlite;
using Discord.Commands;
using Discord.WebSocket;

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

                con.Dispose();
            }
            catch (SqliteException e)
            {
                Logger.Write(Discord.LogSeverity.Error, "Database", "", e);
            }

            return success;
        }

        public static bool Retrieve(string tableName, string columnName, string guildId, out string output)
        {
            output = "";
            bool success = false;
            string cs = @"Data Source=" + GetDatabasePath();
            string cmdTxt = "SELECT " + columnName + " FROM " + tableName + " WHERE GuildId = @guildId";

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
                    con.Dispose();
                }
                else
                {
                    Logger.Write(Discord.LogSeverity.Warning, "Database", $"Was unable to sucessfully retrieve {columnName} from {tableName} at guildId {guildId}");
                }
            }
            catch (SqliteException e)
            {
                Logger.Write(Discord.LogSeverity.Error, "Database", "", e);
            }

            return success;
        }

        public static bool ValidateCommands(IReadOnlyCollection<SocketGuild> guilds, IEnumerable<CommandInfo> commands)
        {
            bool result = false;
            List<string> commandNames = commands.Select(c => c.Name.ToLower()).ToList();
            List<string> guildIds = guilds.Select(g => g.Id.ToString()).ToList();

            string defaultCommandData = "empty";

            result =
            ValidateTable("CommandStates", " INTEGER NOT NULL DEFAULT 0", commandNames, guildIds) &&
            ValidateTable("CommandData", $" TEXT NOT NULL DEFAULT '{defaultCommandData}'", commandNames, guildIds);

            return result;
        }

        private static bool ValidateTable(string tableName, string typeAndDefault, List<string> columnNames, List<string> rowNames)
        {
            GetRowAndColumnList(tableName, out List<string> columnNamesDB, out List<string> rowNamesDB);

            //=======================================================
            //Columns
            columnNamesDB.Remove("GuildId"); //Remove this because it's the GuildId
            var addColumns = columnNames.Except(columnNamesDB);
            var removeColumns = columnNamesDB.Except(columnNames);

            if (addColumns.Count() != 0)
            {
                //ALTER TABLE table_name ADD column_1 INTEGER DEFAULT 0, column_2 INTEGER DEFAULT 0, column_3 INTEGER DEFAULT 0

                string cs = @"Data Source=" + GetDatabasePath();
                string additions = "";

                foreach (var addition in addColumns)
                {
                    string cmdTxt = "ALTER TABLE " + tableName + " ADD COLUMN " + addition + " " + typeAndDefault;

                    try
                    {
                        using var con = new SqliteConnection(cs);
                        con.Open();

                        using var command = new SqliteCommand(cmdTxt, con);
                        command.Parameters.AddWithValue("@addition", addition);
                        command.Prepare();

                        command.ExecuteNonQuery();

                        con.Close();

                        additions += addition + "\n";
                    }
                    catch (SqliteException e)
                    {
                        Logger.Write(Discord.LogSeverity.Error, "Database", "", e);
                    }
                }

                if (!string.IsNullOrEmpty(additions))
                {
                    Logger.Write(Discord.LogSeverity.Info, "Database", $"Added the following columns to the database in {tableName}: \n" + additions);
                }
            }

            if (removeColumns.Count() != 0)
            {
                //ALTER TABLE table_name DROP column_1, DROP column_2, DROP column_3

                string cs = @"Data Source=" + GetDatabasePath();
                string cmdTxt = "ALTER TABLE " + tableName + " DROP ";
                string removed = "";

                foreach (var rem in removeColumns)
                {
                    cmdTxt += rem + ", ";
                    removed += rem + "\n";
                }

                cmdTxt = cmdTxt.Remove(cmdTxt.Length - 2);

                try
                {
                    using var con = new SqliteConnection(cs);
                    con.Open();

                    using var command = new SqliteCommand(cmdTxt, con);
                    command.Prepare();

                    command.ExecuteNonQuery();

                    con.Close();
                }
                catch (SqliteException e)
                {
                    Logger.Write(Discord.LogSeverity.Error, "Database", "", e);
                }

                if (!string.IsNullOrEmpty(removed))
                {
                    Logger.Write(Discord.LogSeverity.Info, "Database", $"Removed the following columns from the database in {tableName}: \n" + removed);
                }
            }

            //=======================================================
            //Rows
            var addRows = rowNames.Except(rowNamesDB);
            var removeRows = rowNamesDB.Except(rowNames);

            if (addRows.Count() != 0)
            {
                //INSERT INTO table_name (GuildId) VALUES (guildId)

                string cs = @"Data Source=" + GetDatabasePath();
                string cmdTxt = "INSERT INTO " + tableName + " (GuildId) VALUES ";
                string additions = "";

                foreach (var row in addRows)
                {
                    cmdTxt += "(" + row + "), ";
                    additions += row + "\n";
                }

                cmdTxt = cmdTxt.Remove(cmdTxt.Length - 2);

                try
                {
                    using var con = new SqliteConnection(cs);
                    con.Open();

                    using var command = new SqliteCommand(cmdTxt, con);
                    command.Prepare();

                    command.ExecuteNonQuery();

                    con.Close();
                }
                catch (SqliteException e)
                {
                    Logger.Write(Discord.LogSeverity.Error, "Database", "", e);
                }

                if (!string.IsNullOrEmpty(additions))
                {
                    Logger.Write(Discord.LogSeverity.Info, "Database", $"Added the following rows into the database in {tableName}: \n" + additions);
                }
            }

            if (removeRows.Count() != 0)
            {
                //DELETE FROM table_name WHERE GuildId = @guildId

                string cs = @"Data Source=" + GetDatabasePath();
                string cmdTxt = "DELETE FROM " + tableName + " WHERE GuildId IN (";
                string removed = "";

                foreach (var rem in removeRows)
                {
                    cmdTxt += rem + ", ";
                    removed += rem + "\n";
                }

                cmdTxt = cmdTxt.Remove(cmdTxt.Length - 2) + ")";

                try
                {
                    using var con = new SqliteConnection(cs);
                    con.Open();

                    using var command = new SqliteCommand(cmdTxt, con);
                    command.Prepare();

                    command.ExecuteNonQuery();

                    con.Close();
                }
                catch (SqliteException e)
                {
                    Logger.Write(Discord.LogSeverity.Error, "Database", "", e);
                }

                if (!string.IsNullOrEmpty(removed))
                {
                    Logger.Write(Discord.LogSeverity.Info, "Database", $"Removed the following rows from the database in {tableName}: \n" + removed);
                }
            }

            return true;
        }

        private static bool GetRowAndColumnList(string tableName, out List<string> columnNames, out List<string> rowNames)
        {
            columnNames = new List<string>();
            rowNames = new List<string>();
            bool success = false;
            string cs = @"Data Source=" + GetDatabasePath();
            string cmdTxt = "SELECT * FROM " + tableName;

            try
            {
                using var con = new SqliteConnection(cs);
                con.Open();

                using var command = new SqliteCommand(cmdTxt, con);
                command.Prepare();

                using SqliteDataReader reader = command.ExecuteReader();

                for (var i = 0; i < reader.FieldCount; i++)
                {
                    columnNames.Add(reader.GetName(i));
                }

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        rowNames.Add(reader.GetString(0));
                    }
                }

                success = true;

                con.Close();
            }
            catch (SqliteException e)
            {
                Logger.Write(Discord.LogSeverity.Error, "Database", "", e);
            }

            return success;
        }

        private static string GetDatabasePath()
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "", @"Data\gadget.db");
        }
    }
}