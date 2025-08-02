namespace OrderService.Models.v1.DTOs
{
    public class PagedResponseDto<T>
    {
        public List<T> Data { get; set; } = new();
        public PaginationMetadata Pagination { get; set; } = new();
        public Dictionary<string, string>? Filters { get; set; }
    }

    public class PaginationMetadata
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
    }
}
