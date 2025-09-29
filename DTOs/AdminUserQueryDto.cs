namespace DemiTicket.DTOs
{
    public class AdminUserQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; } = "asc";
        public string? Search { get; set; }
        public string? Role { get; set; }
        public bool? IsRequestingAuthorization { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
