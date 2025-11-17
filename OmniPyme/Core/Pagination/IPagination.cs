namespace OmniPyme.Web.Core.Pagination
{
    public interface IPagination
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int RecordsPerPage { get; set; }
        public int TotalRecords { get; set; }
        public bool HasPrevius => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
        public string? Filter { get; set; }
        public List<int> Pages { get; }
    }
}
