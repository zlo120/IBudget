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

if (!Directory.Exists("Chunks"))
{
    Directory.CreateDirectory("Chunks");
    Directory.CreateDirectory("Chunks/Input");
    Directory.CreateDirectory("Chunks/Outputs");
}

var UI = new MainUI();