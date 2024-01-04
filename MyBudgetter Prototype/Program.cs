// Create the SQLite db if it doesn't exist
using Core.Chunk;
using Core.Data;
using MyBudgetter_Prototype.UserInterface;

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
    Directory.CreateDirectory("Chunks/Input/Completed");
    Directory.CreateDirectory("Chunks/Outputs");
}

string[] fileNames = Directory.GetFiles("Chunks\\Input");
foreach (var file in fileNames)
{
    ChunkParser.ReadFile(file);
    var fileName = Path.GetFileName(file);
    fileName += DateTime.Now;

    var fileNameNoExtension = Path.GetFileNameWithoutExtension(file);    
    var newDest = Path.Combine("Chunks\\Input\\Completed", fileNameNoExtension + DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss tt") + ".json");
    File.Move(file, newDest);
}

var UI = new MainUI();