namespace API.RequestHelpers
{
    public class MetaData // pagination のメタデータを管理する
    {
        // 現在のページ番号
        public int CurrentPage { get; set; }
        // 何ページあるか
        public int TotalPages { get; set; }
        // 1 ページに何個項目が入っているか
        public int PageSize { get; set; }
        // 全部で項目が何個あるか
        public int TotalCount { get; set; }
    }
}