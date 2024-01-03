using System.Text.Json;

namespace MyBudgetter_Prototype.Chunk
{
    public class ChunkParser
    {
        public static void ReadFile(string filePath)
        {
            string jsonFromFile = File.ReadAllText(filePath);

        }
    }
}