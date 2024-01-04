using Core.Data;
using Newtonsoft.Json;

namespace Core.Chunk
{
    public static class ChunkWriter
    {
        public static void WriteToJSON(DateTime startDate, DateTime endDate)
        {
            var chunkData = Database.GetChunkDataByDateRange(startDate, endDate);
            chunkData.DateRange = new List<DateTime> { startDate, endDate };

            string json = JsonConvert.SerializeObject(chunkData);
            var fileName = $"{DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss tt")} Chunk.json";
            File.WriteAllText(Path.Combine("Chunks\\Outputs", fileName), json);
        }
    }
}