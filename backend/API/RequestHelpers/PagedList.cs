using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace API.RequestHelpers
{
    // product に限らず、汎用的に使えるようにジェネリックにする
    // ページ番号とページサイズを渡せば、そのページ番号の項目をすべて List<T> 型にまとめ、
    // MetaData 付きで返す
    public class PagedList<T> : List<T>
    {
        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            MetaData = new MetaData
            {
                TotalCount = count,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                // product が 18 === count が 18
                // PageSize が 10 の場合
                // TotalPage は 18 / 10 = 1.8 の繰り上げで 2 ページ
                TotalPages = (int)Math.Ceiling(count / (double)pageSize),
            };
            AddRange(items); // this.AddRange(items) でも可。this はあってもなくても問題なし
        }

        public MetaData MetaData { get; set; }

        public static async Task<PagedList<T>> ToPagedList(IQueryable<T> query, int pageNumber, int pageSize)
        {
            // Search, Filter, Sort が終わった後のトータルの数を知るために DB にクエリする必要がある
            var count = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}