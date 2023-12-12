// Create the SQLite db if it doesn't exist
using MyBudgetter_Prototype.UserInterface;

if (!Directory.Exists("IBudgetterDB"))
{
    Directory.CreateDirectory("IBudgetterDB");
}

if (!File.Exists("IBudgetterDB/IBudgetter.db"))
{
    File.Create("IBudgetterDB/IBudgetter.db");
}

var UI = new MainUI();