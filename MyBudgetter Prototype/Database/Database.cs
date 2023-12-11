using MyBudgetter_Prototype.Model;
using System.Data.SQLite;

namespace MyBudgetter_Prototype.Database
{
    public static class Database
    {
        static string connectionString = "Data Source=IBudgetterDB/IBudgetter.db;Version=3;";

        static void InitiateDatabase()
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                // Open the connection
                connection.Open();

                // Create a table (if it doesn't exist)
                // Create the IncomeRecord table
                string createTableQuery = "CREATE TABLE IF NOT EXISTS IncomeRecord (" +
                                         "ID INTEGER PRIMARY KEY AUTOINCREMENT," +
                                         "Category TEXT," +
                                         "Date DATETIME," +
                                         "Amount DOUBLE);";

                using (SQLiteCommand createTableCommand = new SQLiteCommand(createTableQuery, connection))
                {
                    createTableCommand.ExecuteNonQuery();
                }

                createTableQuery = "CREATE TABLE IF NOT EXISTS ExpenseRecord (" +
                                         "ID INTEGER PRIMARY KEY AUTOINCREMENT," +
                                         "Category TEXT," +
                                         "Date DATETIME," +
                                         "Amount DOUBLE);";

                using (SQLiteCommand createTableCommand = new SQLiteCommand(createTableQuery, connection))
                {
                    createTableCommand.ExecuteNonQuery();
                }
            }
        }

        static void Insert(DataEntry data, string table)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                // Open the connection
                connection.Open();

                // Insert data into the IncomeRecord table
                string insertDataQuery = $"INSERT INTO {table} (Category, Day, Money) VALUES (@Category, @Day, @Money);";

                using (SQLiteCommand insertDataCommand = new SQLiteCommand(insertDataQuery, connection))
                {
                    // Set parameters for the insert query
                    insertDataCommand.Parameters.AddWithValue("@Category", "Salary");
                    insertDataCommand.Parameters.AddWithValue("@Day", new DateTime(2023, 1, 1)); // Use specific date
                    insertDataCommand.Parameters.AddWithValue("@Money", 2000.00);

                    // Execute the insert query
                    insertDataCommand.ExecuteNonQuery();
                }
            }
        }
    }
}
