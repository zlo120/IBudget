namespace IBudget.Core.Model
{
    public class ExportFile<T>
    {
        public required string CollectionName { get; set; }
        public required List<T> Data { get; set; }
    }
}
