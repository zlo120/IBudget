namespace IBudget.Core.DatabaseModel
{
    public class PaginatedResponse<T>
    {
        public required bool HasMoreData { get; set; }
        public required IReadOnlyList<T> Data { get; set; }
    }
}
