using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Threading.Tasks;

namespace DbTextEditor
{
    /// <summary>
    /// Class contains methods for accessing and modifying database records (files) related to Text Editor.
    /// All database-related methods are working asynchronously to eliminate the possibility of blocking WPF UI thread.
    /// </summary>
    class FileRepository
    {
        // Default connection string, located in App.config
        private static string defaultConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        /// <summary>
        /// Saves byte array to database
        /// </summary>
        /// <param name="name">File name</param>
        /// <param name="file">File bytes</param>
        public static async Task SaveFileAsync(string name, byte[] file)
        {
            string connectionString = defaultConnectionString;

            using (var connection = new OleDbConnection(connectionString))
            {
                try
                {
                    // Try to open connection to Db
                    await connection.OpenAsync();

                    string query;
                    // If there is a record with same name, use another query to overwrite its data
                    if (await FileExists(name, connection))
                        query = "UPDATE Files SET FileData = @data WHERE Name = @name;";
                    else
                        query = "INSERT INTO Files ([Name],[FileData]) VALUES (@name,@data);";

                    using (var command2 = new OleDbCommand(query, connection))
                    {
                        // Queries are parameterized as a measure to prevent SQL injection
                        command2.Parameters.AddWithValue("@name", name);
                        command2.Parameters.AddWithValue("@data", file);

                        // Nonquery method is good when there is nothing to return
                        await command2.ExecuteNonQueryAsync();
                    }

                }
                catch (Exception ex)
                {
                    // Possible more complex exception handling
                    TextEditor.WPFMessageBoxException(ex);
                }
            }
        }

        /// <summary>
        /// Gets a list of all the files stored in the database
        /// </summary>
        public static async Task<List<string>> GetAllFileNamessAsync()
        {
            string connectionString = defaultConnectionString;

            using (var connection = new OleDbConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    string query = "SELECT Name FROM Files;";
                    using (var command = new OleDbCommand(query, connection))
                    {
                        var reader = await command.ExecuteReaderAsync();

                        var list = new List<string>();

                        while (await reader.ReadAsync())
                            list.Add(reader.GetString(0));

                        return list;
                    }
                }
                catch (Exception ex)
                {
                    TextEditor.WPFMessageBoxException(ex);
                    return null;
                }
            }
        }

        /// <summary>
        /// Receives file bytes from database by given file name
        /// </summary>
        /// <param name="name">Name of file to load</param>
        public static async Task<byte[]> LoadFileAsync(string name)
        {
            string connectionString = defaultConnectionString;

            using (var connection = new System.Data.OleDb.OleDbConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    string query = "SELECT FileData FROM Files WHERE name = @name;";

                    using (var command = new System.Data.OleDb.OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", name);

                        return (byte[])await command.ExecuteScalarAsync();
                    }
                }
                catch(Exception ex)
                {
                    TextEditor.WPFMessageBoxException(ex);
                    return null;
                }
            }
        }

        /// <summary>
        /// Checks database for existence of a file name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        private static async Task<bool> FileExists(string name, OleDbConnection connection)
        {
            string query = "SELECT Name FROM Files WHERE name = @name;";

            using (var command = new System.Data.OleDb.OleDbCommand(query, connection))
            {
                command.Parameters.AddWithValue("@name", name);

                var reader = await command.ExecuteReaderAsync();

                // First run of ReadAsync() will return true if there are records matching its query
                return await reader.ReadAsync();
            }
        }

        public static void CreateDatabase(string connectionString)
        {
            try
            {
                // Create DB in form of .mdb file
                ADOX.CatalogClass catalog = new ADOX.CatalogClass();
                catalog.Create(connectionString);

                ADOX.Table table = new ADOX.Table();
                table.Name = "Files"; // Table name

                // Id column
                ADOX.ColumnClass idCol = new ADOX.ColumnClass()
                {
                    Name = "Id",
                    ParentCatalog = catalog,
                    Type = ADOX.DataTypeEnum.adInteger,
                };
                idCol.Properties["AutoIncrement"].Value = true;


                // Name column
                ADOX.ColumnClass nameCol = new ADOX.ColumnClass()
                {
                    Name = "Name",
                    ParentCatalog = catalog,
                    Type = ADOX.DataTypeEnum.adVarWChar,
                    DefinedSize = 16,
                };

                // FileData column (BLOBs)
                ADOX.ColumnClass fileCol = new ADOX.ColumnClass()
                {
                    Name = "FileData",
                    ParentCatalog = catalog,
                    Type = ADOX.DataTypeEnum.adLongVarBinary
                };

                // Add columns to Files table
                table.Columns.Append(idCol);
                table.Columns.Append(nameCol);
                table.Columns.Append(fileCol);

                // Add table to .mdb catalog
                catalog.Tables.Append(table);

                // Close the connection
                ADODB.Connection con = (ADODB.Connection)catalog.ActiveConnection;
                if (con != null && con.State != 0)
                    con.Close();
            }
            catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Checks if database exists and there is no problems with connection
        /// </summary>
        /// <param name="connectionString">Connection string for the database</param>
        /// <returns>True if DB exists and connection can be opened, false otherwise</returns>
        public static bool DbExists(string connectionString)
        {
            try
            {
                using (var connection = new System.Data.OleDb.OleDbConnection(connectionString))
                    connection.Open();

                return true;
            }
            catch (Exception ex)
            {
                // There is no Error Code to rely on, good excuse for this rough method
                if (ex.Message.Contains("Could not find file"))
                    return false;
                else
                // This method only to discover database existence, if there is other exception - throw it
                    throw ex;
            }
        }
    }
}
