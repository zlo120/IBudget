using MyBudgetter_Prototype.Model;
using System.Data.SQLite;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                                         "Notes TEXT);";

                using (SQLiteCommand createTableCommand = new SQLiteCommand(createTableQuery, connection))
                {
                    createTableCommand.ExecuteNonQuery();
                }

                createTableQuery = "CREATE TABLE IF NOT EXISTS Tags (" +
                                         "ID INTEGER PRIMARY KEY AUTOINCREMENT," +
                                         "Name TEXT UNIQUE);";

                using (SQLiteCommand createTableCommand = new SQLiteCommand(createTableQuery, connection))
                {
                    createTableCommand.ExecuteNonQuery();
                }

                createTableQuery = "CREATE TABLE IF NOT EXISTS ExpenseTags (" +
                                         "ExpenseID INTEGER," +
                                         "TagID INTEGER," +
                                         "PRIMARY KEY (ExpenseID, TagID)," +
                                         "FOREIGN KEY (ExpenseID) REFERENCES ExpenseRecord (ID)," +
                                         "FOREIGN KEY (TagID) REFERENCES Tags (ID));";

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
                string insertDataQuery = $"INSERT INTO IncomeRecord (Category, Date, Amount, Frequency, Source) VALUES (@Category, @Date, @Amount, @Frequency, @Source);";

                using (SQLiteCommand insertDataCommand = new SQLiteCommand(insertDataQuery, connection))
                {
                    // Set parameters for the insert query
                    insertDataCommand.Parameters.AddWithValue("@Category", data.Category);
                    insertDataCommand.Parameters.AddWithValue("@Date", data.Date); // Use specific date
                    insertDataCommand.Parameters.AddWithValue("@Amount", data.Amount);
                    insertDataCommand.Parameters.AddWithValue("@Source", data.Source);

                    // Convert frequency enum to string
                    if (data.Frequency is not null)
                    {
                        var frequency = FrequencyMethods.ConvertToString((Frequency)data.Frequency);
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

        public static void InsertExpense(Expense data)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                // Open the connection
                connection.Open();

                // Insert data into the ExpenseRecord table
                string insertDataQuery = $"INSERT INTO ExpenseRecord (Category, Date, Amount, Frequency, Notes) VALUES (@Category, @Date, @Amount, @Frequency, @Notes);";

                using (SQLiteCommand insertDataCommand = new SQLiteCommand(insertDataQuery, connection))
                {
                    // Set parameters for the insert query
                    insertDataCommand.Parameters.AddWithValue("@Category", data.Category);
                    insertDataCommand.Parameters.AddWithValue("@Date", data.Date); // Use specific date
                    insertDataCommand.Parameters.AddWithValue("@Amount", data.Amount);
                    insertDataCommand.Parameters.AddWithValue("@Notes", data.Notes);

                    // Converting frequency enum to string
                    if (data.Frequency is not null)
                    {
                        var frequency = FrequencyMethods.ConvertToString((Frequency)data.Frequency);
                        insertDataCommand.Parameters.AddWithValue("@Frequency", frequency);
                    }
                    else
                    {
                        insertDataCommand.Parameters.AddWithValue("@Frequency", null);
                    }

                    // Execute the insert query
                    insertDataCommand.ExecuteNonQuery();
                    int expenseRecordID = (int) connection.LastInsertRowId;

                    // Insert associated tags
                    InsertTags(expenseRecordID, data.Tags);
                }
            }
        }
        public static void InsertTags(int expenseRecordID, List<string> tags)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                // Open the connection
                connection.Open();

                // Search if tag exists
                foreach(var tag in tags)
                {
                    string selectTagQuery = "SELECT * FROM Tags WHERE LOWER(Name) = @TagName COLLATE NOCASE;";

                    bool tagExists = false;
                    int tagID = -1;

                    using (SQLiteCommand selectTagCommand = new SQLiteCommand(selectTagQuery, connection))
                    {
                        // Set parameter for the select query
                        selectTagCommand.Parameters.AddWithValue("@TagName", tag);

                        // Execute the select query
                        using (SQLiteDataReader reader = selectTagCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Process each row of the result set
                                tagID = reader.GetInt32(0);
                                tagExists = true;
                                break;
                            }
                        }
                    }

                    if (!tagExists)
                    {
                        // Create tag
                        string insertTagQuery = $"INSERT INTO Tags (Name) VALUES (@Name);";
                        using (SQLiteCommand insertDataCommand = new SQLiteCommand(insertTagQuery, connection))
                        {
                            // Set parameters for the insert query
                            insertDataCommand.Parameters.AddWithValue("@Name", tag);

                            // Execute the insert query
                            insertDataCommand.ExecuteNonQuery();
                            tagID = (int) connection.LastInsertRowId;
                        }
                    }

                    // Create a relationship between tag and expense
                    string insertExpenseTagQuery = $"INSERT INTO ExpenseTags (ExpenseID, TagID) VALUES (@ExpenseID, @TagID);";
                    using (SQLiteCommand insertDataCommand = new SQLiteCommand(insertExpenseTagQuery, connection))
                    {
                        // Set parameters for the insert query
                        insertDataCommand.Parameters.AddWithValue("@ExpenseID", expenseRecordID);
                        insertDataCommand.Parameters.AddWithValue("@TagID", tagID);

                        // Execute the insert query
                        insertDataCommand.ExecuteNonQuery();
                    }
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