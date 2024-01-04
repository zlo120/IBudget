using Core.Data;
using Core.Model;
using Newtonsoft.Json;

namespace Core.Chunk
{
    public static class ChunkParser
    {
        public static void ReadFile(string filePath)
        {
            Console.WriteLine(filePath);
            string jsonFromFile = File.ReadAllText(filePath);

            // Accessing properties from the JsonDocument
            var root = JsonConvert.DeserializeObject<RootObject>(jsonFromFile);

            // check if tags exist in the db
            var tags = root.Tags;
            var tagsFromDB = new List<Tag>();
            foreach (var tag in tags)
            {
                // if the tag exists in the db
                if (Database.TagExistsInDB(tag)) continue;

                // if tag does not exist, create it in the db, returns the Tag's ID
                var tagID = Database.CreateSingularTag(tag.Name);

                tagsFromDB.Add(new Tag
                {
                    ID = tagID.Value,
                    Name = tag.Name
                });
            }

            // Insert income records
            var incomeRecords = root.IncomeRecords;
            foreach (var incomeRecord in incomeRecords)
                Database.InsertIncome(incomeRecord);

            // Insert expense records
            var expenseRecords = root.ExpenseRecords;
            foreach ( var expenseRecord in expenseRecords)
                Database.InsertExpense(expenseRecord);

        }
    }
}