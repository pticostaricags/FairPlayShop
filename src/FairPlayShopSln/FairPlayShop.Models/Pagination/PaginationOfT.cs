namespace FairPlayShop.Models.Pagination
{
    public class PaginationOfT<T>
    {
        public int TotalItems { get; set; }
        public T[]? Items { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
    }
}
