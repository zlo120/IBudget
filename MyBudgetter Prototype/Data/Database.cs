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
        public static void Insert(DataEntry data, string table)
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
                    insertDataCommand.Parameters.AddWithValue("@Category", data.Category);
                    insertDataCommand.Parameters.AddWithValue("@Day", data.Date); // Use specific date
                    insertDataCommand.Parameters.AddWithValue("@Money", data.Amount);

                    // Execute the insert query
                    insertDataCommand.ExecuteNonQuery();
                }
            }
        }
        public static Week GetWeek(DateTime date, Week week)
        {

            CultureInfo ci = CultureInfo.InvariantCulture;

            var targetWeek = ci.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday);

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string selectDataQuery = "SELECT * FROM IncomeRecord WHERE strftime('%W', Day) = @TargetWeek;";

                using (SQLiteCommand selectDataCommand = new SQLiteCommand(selectDataQuery, connection))
                {
                    // Set parameter for the select query
                    selectDataCommand.Parameters.AddWithValue("@TargetWeek", targetWeek);

                    // Execute the select query
                    using (SQLiteDataReader reader = selectDataCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Process each row of the result set
                            int id = reader.GetInt32(0);
                            string category = reader.GetString(1);
                            DateTime day = reader.GetDateTime(2);
                            double money = reader.GetDouble(3);

                            var incomeRecord = new Income
                            {
                                Category = category,
                                Date = day,
                                Amount = money
                            };

                            week.Income.Add(incomeRecord);
                        }
                    }
                }

                selectDataQuery = "SELECT * FROM ExpenseRecord WHERE strftime('%W', Day) = @TargetWeek;";
                using (SQLiteCommand selectDataCommand = new SQLiteCommand(selectDataQuery, connection))
                {
                    // Set parameter for the select query
                    selectDataCommand.Parameters.AddWithValue("@TargetWeek", targetWeek);

                    // Execute the select query
                    using (SQLiteDataReader reader = selectDataCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Process each row of the result set
                            int id = reader.GetInt32(0);
                            string category = reader.GetString(1);
                            DateTime day = reader.GetDateTime(2);
                            double money = reader.GetDouble(3);

                            var expenseRecord = new Expense
                            {
                                Category = category,
                                Date = day,
                                Amount = money
                            };

                            week.Expenses.Add(expenseRecord);
                        }
                    }
                }
            }

            return week;
        }
        public static Month GetMonth(Month month)
        {
            var weeks = Calendar.GetAllWeeks(month.MonthNum, month.Year.YearNumber);

            foreach (var weekDateTime in weeks)
            {
                var label = $"{weekDateTime.ToShortDateString()} - {weekDateTime.AddDays(6).ToShortDateString()}";
                var week = new Week(month, label);
                month.Weeks.Add(GetWeek(weekDateTime, week));
            }

            return month;
        }
        public static Year GetYear(Year year)
        {
            for (int i = 1; i <= 12; i++)
            {
                var month = new Month(i, year);
                year.Months.Add(GetMonth(month));
            }

            return year;
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