namespace IBudget.Core.DatabaseModel
{
    public class PaginatedResponse<T>
    {
        public required int PageNumber { get; set; }
        public required int PageSize { get; set; }
        public required int TotalPageCount { get; set; }
        public required int TotalDataCount { get; set; }
        public required IReadOnlyList<T> Data { get; set; }
    }
}
