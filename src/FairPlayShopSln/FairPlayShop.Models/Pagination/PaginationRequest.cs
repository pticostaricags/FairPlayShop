namespace FairPlayShop.Models.Pagination
{
    public class PaginationRequest
    {
        public int StartIndex { get; set; }
        public SortingItem[]? SortingItems { get; set; }
    }

    public class SortingItem
    {
        public string? PropertyName { get; set; }
        public SortType SortType { get; set; }
    }

    public enum SortType
    {
        Ascending,
        Descending
    }
}
