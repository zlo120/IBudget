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
                        var frequency = FrequencyMethods.ConvertToString(data.Frequency.Value);
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
                        var frequency = FrequencyMethods.ConvertToString(data.Frequency.Value);
                        insertDataCommand.Parameters.AddWithValue("@Frequency", frequency);
                    }
                    else
                    {
                        insertDataCommand.Parameters.AddWithValue("@Frequency", null);
                    }

                    // Execute the insert query
                    insertDataCommand.ExecuteNonQuery();
                    int expenseRecordID = (int)connection.LastInsertRowId;

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
                foreach (var tag in tags)
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
                            tagID = (int)connection.LastInsertRowId;
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
        public static Expense GetExpenseRecord(int ID)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                Expense expense = null;

                string selectExpenseQuery = $"SELECT * FROM ExpenseRecord WHERE ID = @RecordID;";

                using (SQLiteCommand selectCommand = new SQLiteCommand(selectExpenseQuery, connection))
                {
                    selectCommand.Parameters.AddWithValue("@RecordID", ID);

                    using (SQLiteDataReader reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Frequency? frequency;
                            if (!reader.IsDBNull(4))
                            {
                                frequency = FrequencyMethods.ConvertToFrequency(reader.GetString(4));
                            }
                            else
                            {
                                frequency = null;
                            }

                            expense = new Expense()
                            {
                                ID = reader.GetInt32(0),
                                Category = reader.GetString(1),
                                Date = reader.GetDateTime(2),
                                Amount = reader.GetDouble(3),
                                Frequency = frequency,
                                Notes = reader.GetString(5),
                            };
                        }
                    }
                }

                if (expense == null) return null;

                // get all the tags
                if (expense.Tags == null) expense.Tags = new List<string>();

                expense.Tags = GetTags(expense.ID.Value);

                return expense;
            }
        }
        public static Income GetIncomeRecord(int ID)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string selectIncomeQuery = $"SELECT * FROM IncomeRecord WHERE ID = @RecordID;";

                using (SQLiteCommand selectCommand = new SQLiteCommand(selectIncomeQuery, connection))
                {
                    selectCommand.Parameters.AddWithValue("@RecordID", ID);

                    using (SQLiteDataReader reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Frequency? frequency;
                            if (!reader.IsDBNull(4))
                            {
                                frequency = FrequencyMethods.ConvertToFrequency(reader.GetString(4));
                            }
                            else
                            {
                                frequency = null;
                            }

                            return new Income()
                            {
                                ID = reader.GetInt32(0),
                                Category = reader.GetString(1),
                                Date = reader.GetDateTime(2),
                                Amount = reader.GetDouble(3),
                                Frequency = frequency,
                                Source = reader.GetString(5),
                            };
                        }
                    }
                }
            }

            return null;
        }
        public static Week GetWeek(DateTime[] weekRange)
        {
            var weekLabel = Calendar.GetWeekLabel(weekRange[0]);
            var week = new Week(weekLabel);

            Frequency? frequency;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string selectIncomeQuery = "SELECT * FROM IncomeRecord WHERE Date BETWEEN @StartDate AND @EndDate;";

                using (SQLiteCommand selectIncomeCommand = new SQLiteCommand(selectIncomeQuery, connection))
                {
                    // Set parameters for the select query
                    selectIncomeCommand.Parameters.AddWithValue("@StartDate", weekRange[0].ToString("yyyy-MM-dd"));
                    selectIncomeCommand.Parameters.AddWithValue("@EndDate", weekRange[1].ToString("yyyy-MM-dd"));

                    // Execute the select query
                    using (SQLiteDataReader reader = selectIncomeCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(4))
                            {
                                frequency = FrequencyMethods.ConvertToFrequency(reader.GetString(4));
                            }
                            else
                            {
                                frequency = null;
                            }

                            week.Income.Add(new Income()
                            {
                                ID = reader.GetInt32(0),
                                Category = reader.GetString(1),
                                Date = reader.GetDateTime(2),
                                Amount = reader.GetDouble(3),
                                Frequency = frequency,
                                Source = reader.GetString(5)
                            });
                        }
                    }
                }

                var expenses = new List<Expense>();

                string selectExpenseQuery = "SELECT * FROM ExpenseRecord WHERE Date BETWEEN @StartDate AND @EndDate;";

                using (SQLiteCommand selectExpenseCommand = new SQLiteCommand(selectExpenseQuery, connection))
                {
                    // Set parameters for the select query
                    selectExpenseCommand.Parameters.AddWithValue("@StartDate", weekRange[0].ToString("yyyy-MM-dd"));
                    selectExpenseCommand.Parameters.AddWithValue("@EndDate", weekRange[1].ToString("yyyy-MM-dd"));

                    // Execute the select query
                    using (SQLiteDataReader reader = selectExpenseCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(4))
                            {
                                frequency = FrequencyMethods.ConvertToFrequency(reader.GetString(4));
                            }
                            else
                            {
                                frequency = null;
                            }
                            var expense = new Expense()
                            {
                                ID = reader.GetInt32(0),
                                Category = reader.GetString(1),
                                Date = reader.GetDateTime(2),
                                Amount = reader.GetDouble(3),
                                Frequency = frequency,
                                Notes = reader.GetString(5)
                            };

                            expenses.Add(expense);
                        }
                    }
                }

                foreach (var expense in expenses)
                {
                    if (expense.Tags is null) expense.Tags = new List<string>();
                    expense.Tags = GetTags(expense.ID.Value);
                    week.Expenses.Add(expense);
                }

            }

            return week;
        }
        public static List<string> GetTags(int expenseID)
        {
            var tags = new List<string>();
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string selectTagQuery = "SELECT Tags.Name FROM Tags " +
                             "INNER JOIN ExpenseTags ON Tags.ID = ExpenseTags.TagID " +
                             "WHERE ExpenseTags.ExpenseID = @ExpenseRecordID;";

                using (SQLiteCommand selectTagCommand = new SQLiteCommand(selectTagQuery, connection))
                {
                    selectTagCommand.Parameters.AddWithValue("@ExpenseRecordID", expenseID);

                    using (SQLiteDataReader reader = selectTagCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Process each row of the result set
                            tags.Add(reader.GetString(0));
                        }
                    }
                }
            }

            return tags;
        }
        public static Month GetMonth(int monthNum)
        {
            var month = new Month(monthNum);

            foreach (var weekRange in month.WeekRanges)
            {
                var week = GetWeek(weekRange);
                month.Weeks.Add(week);
            }

            return month;
        }
        public static void UpdateIncomeRecord(Income updatedRecord)
        {
            // Check if the record exists first
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string selectIncomeQuery = "SELECT * FROM IncomeRecord WHERE ID = @RecordID;";

                using (SQLiteCommand selectIncomeCommand = new SQLiteCommand(selectIncomeQuery, connection))
                {
                    selectIncomeCommand.Parameters.AddWithValue("@RecordID", updatedRecord.ID);

                    // Execute the select query
                    var result = selectIncomeCommand.ExecuteScalar();

                    if (result == null)
                    {
                        return;
                    }
                }

                string updateQuery = "UPDATE IncomeRecord SET Category = @Category, Date = @Date, Amount = @Amount, Frequency = @Frequency, Source = @Source WHERE ID = @ID;";

                using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                {

                    command.Parameters.AddWithValue("@ID", updatedRecord.ID);

                    if (updatedRecord.Frequency is not null)
                    {
                        var frequency = FrequencyMethods.ConvertToString(updatedRecord.Frequency.Value);
                        command.Parameters.AddWithValue("@Frequency", frequency);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@Frequency", null);
                    }

                    command.Parameters.AddWithValue("@Category", updatedRecord.Category);
                    command.Parameters.AddWithValue("@Date", updatedRecord.Date);
                    command.Parameters.AddWithValue("@Amount", updatedRecord.Amount);
                    command.Parameters.AddWithValue("@Source", updatedRecord.Source);

                    command.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateExpenseRecord(Income updatedRecord)
        {
            // Check if the record exists first
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string selectIncomeQuery = "SELECT * FROM IncomeRecord WHERE ID = @RecordID;";

                using (SQLiteCommand selectIncomeCommand = new SQLiteCommand(selectIncomeQuery, connection))
                {
                    selectIncomeCommand.Parameters.AddWithValue("@RecordID", updatedRecord.ID);

                    // Execute the select query
                    var result = selectIncomeCommand.ExecuteScalar();

                    if (result == null)
                    {
                        return;
                    }
                }

                string updateQuery = "UPDATE IncomeRecord SET Category = @Category, Date = @Date, Amount = @Amount, Frequency = @Frequency, Source = @Source WHERE ID = @ID;";

                using (SQLiteCommand command = new SQLiteCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@Category", updatedRecord.Category);
                    command.Parameters.AddWithValue("@Date", updatedRecord.Date);
                    command.Parameters.AddWithValue("@Amount", updatedRecord.Amount);
                    command.Parameters.AddWithValue("@Frequency", updatedRecord.Frequency);
                    command.Parameters.AddWithValue("@Source", updatedRecord.Source);

                    command.ExecuteNonQuery();
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