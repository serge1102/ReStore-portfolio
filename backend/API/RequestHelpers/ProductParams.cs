namespace API.RequestHelpers
{
    public class ProductParams : PaginationParams // 継承しているので PaginationParams のフィールドも使える
    {
        public string OrderBy { get; set; }
        public string SearchTerm { get; set; }
        public string Types { get; set; }
        public string Brands { get; set; }
    }
}