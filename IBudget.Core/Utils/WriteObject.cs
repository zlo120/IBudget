namespace IBudget.Core.Utils
{
    public static class WriteObject
    {
        public static string ToJsonString(this object obj)
        {
            return System.Text.Json.JsonSerializer.Serialize(obj, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });
        }
    }
}
