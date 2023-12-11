using MyBudgetter_Prototype.Model;

// Create the SQLite db if it doesn't exist
if (!Directory.Exists("IBudgetterDB"))
{
    Directory.CreateDirectory("IBudgetterDB");
}

if (!File.Exists("IBudgetterDB/IBudgetter.db"))
{
    File.Create("IBudgetterDB/IBudgetter.db");
}