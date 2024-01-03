// Create the SQLite db if it doesn't exist
using MyBudgetter_Prototype.UserInterface;
using MyBudgetter_Prototype.Data;
using MyBudgetter_Prototype.Chunk;

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

string[] fileNames = Directory.GetFiles("Chunks/Input");
foreach (var file in fileNames)
{
    ChunkParser.ReadFile("Chunks/Input/" + file);
}

var UI = new MainUI();