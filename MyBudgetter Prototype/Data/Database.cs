using MyBudgetter_Prototype.Model;
using System.Data.SQLite;
using System.Globalization;

namespace MyBudgetter_Prototype.Data
{
    public class Database
    {
        public static string connectionString = "Data Source=IBudgetterDB/IBudgetter.db;Version=3;";

        public static void InitiateDatabase()
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                // Open the connection
                connection.Open();

                // Create a table (if it doesn't exist)
                // Create the IncomeRecord table
                string createTableQuery = "CREATE TABLE IF NOT EXISTS IncomeRecord (" +
                                         "ID INTEGER PRIMARY KEY AUTOINCREMENT," +
                                         "Category TEXT NOT NULL," +
                                         "Date DATETIME NOT NULL," +
                                         "Amount DOUBLE NOT NULL," +
                                         "Frequency TEXT," +
                                         "Source TEXT);";

                using (SQLiteCommand createTableCommand = new SQLiteCommand(createTableQuery, connection))
                {
                    createTableCommand.ExecuteNonQuery();
                }

                createTableQuery = "CREATE TABLE IF NOT EXISTS ExpenseRecord (" +
                                         "ID INTEGER PRIMARY KEY AUTOINCREMENT," +
                                         "Category TEXT NOT NULL," +
                                         "Date DATETIME NOT NULL," +
                                         "Amount DOUBLE NOT NULL," +
                                         "Frequency TEXT," +
                                         "Recurring INTEGER," +
                                         "Notes TEXT);";

                using (SQLiteCommand createTableCommand = new SQLiteCommand(createTableQuery, connection))
                {
                    createTableCommand.ExecuteNonQuery();
                }
            }
        }
        public static void InsertIncome(Income data)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                // Open the connection
                connection.Open();

                // Insert data into the IncomeRecord table
                string insertDataQuery = $"INSERT INTO IncomeRecord (Category, Date, Amount, Frequency, Source) VALUES (@Category, @Day, @Money, @Frequency, @Source);";

                using (SQLiteCommand insertDataCommand = new SQLiteCommand(insertDataQuery, connection))
                {
                    // Set parameters for the insert query
                    insertDataCommand.Parameters.AddWithValue("@Category", data.Category);
                    insertDataCommand.Parameters.AddWithValue("@Day", data.Date); // Use specific date
                    insertDataCommand.Parameters.AddWithValue("@Money", data.Amount);
                    insertDataCommand.Parameters.AddWithValue("@Source", data.Source);

                    // Convert frequency enum to string
                    if (data.Frequency is not null)
                    {
                        var frequency = FrequencyMethods.ConvertToString((Frequency) data.Frequency);
                        insertDataCommand.Parameters.AddWithValue("@Frequency", frequency);
                    }
                    else
                    {
                        insertDataCommand.Parameters.AddWithValue("@Frequency", null);
                    }

                    // Execute the insert query
                    insertDataCommand.ExecuteNonQuery();
                }
            }
        }
        public static void Delete(int ID, string table)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                string deleteDataQuery = "DELETE FROM IncomeRecord WHERE ID = @ID;";

                using (SQLiteCommand deleteDataCommand = new SQLiteCommand(deleteDataQuery, connection))
                {
                    // Set parameter for the delete query
                    deleteDataCommand.Parameters.AddWithValue("@ID", ID); // Specify the condition to delete

                    // Execute the delete query
                    int rowsAffected = deleteDataCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Data deleted from the IncomeRecord table successfully.");
                    }
                    else
                    {
                        Console.WriteLine("No matching records found for deletion.");
                    }
                }
            }

        }
    }
}