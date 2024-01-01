// Create the SQLite db if it doesn't exist
using MyBudgetter_Prototype.UserInterface;
using MyBudgetter_Prototype.Data;

if (!Directory.Exists("IBudgetterDB"))
{
    Directory.CreateDirectory("IBudgetterDB");
}

if (!File.Exists("IBudgetterDB/IBudgetter.db"))
{
    var dbFile = File.Create("IBudgetterDB/IBudgetter.db");
    dbFile.Close();
    Database.InitiateDatabase();
}

var UI = new MainUI();