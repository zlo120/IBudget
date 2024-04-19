// Create the SQLite db if it doesn't exist
using Core.Interfaces;
using Core.Services;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using MyBudgetter_Prototype.UserInterface;

var folder = Environment.SpecialFolder.LocalApplicationData;
var rootDir = Path.Combine(Environment.GetFolderPath(folder), "IBudgetter");
var dbDir = Path.Join(rootDir, "IBudgetterDB");
var chunksDir = Path.Join(rootDir, "Chunks");

if (!Directory.Exists(rootDir)) Directory.CreateDirectory(rootDir);
if (!Directory.Exists(dbDir)) Directory.CreateDirectory(dbDir);

if (!File.Exists(Path.Join(dbDir, "IBudgetter.db")))
{
    var dbFile = File.Create(Path.Join(dbDir, "IBudgetter.db"));
    dbFile.Close();
    //Database.InitiateDatabase();
}

if (!Directory.Exists(chunksDir))
{
    Directory.CreateDirectory(chunksDir);
    Directory.CreateDirectory(Path.Join(chunksDir, "Input"));
    Directory.CreateDirectory(Path.Join(chunksDir, "Input/Completed"));
    Directory.CreateDirectory(Path.Join(chunksDir, "Outputs"));
}

//string[] fileNames = Directory.GetFiles(Path.Join(chunksDir, "Input"));
//foreach (var file in fileNames)
//{
//    ChunkParser.ReadFile(file);
//    var fileName = Path.GetFileName(file);
//    fileName += DateTime.Now;

//    var fileNameNoExtension = Path.GetFileNameWithoutExtension(file);
//    var newDest = Path.Combine(Path.Join(chunksDir, "Input/Completed"), fileNameNoExtension + DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss tt") + ".json");
//    File.Move(file, newDest);
//}

var services = new ServiceCollection();
services.AddTransient<IIncomeService, IncomeService>();
services.AddTransient<IIncomeRepository, IncomeRepository>();
services.AddTransient<IExpenseService, ExpenseService>();
services.AddTransient<IExpenseRepository, ExpenseRepository>();
services.AddTransient<ISummaryService, SummaryService>();
services.AddTransient<ISummaryRepository, SummaryRepository>();
services.AddTransient<ITagService, TagService>();
services.AddTransient<ITagRepository, TagRepository>();
services.AddTransient<Context>();
using var serviceProvider = services.BuildServiceProvider();

var mainMenu = new MainMenu(serviceProvider);