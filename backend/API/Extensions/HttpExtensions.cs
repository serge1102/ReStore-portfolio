using System.Text.Json;
using API.RequestHelpers;
using Microsoft.AspNetCore.Http;

namespace API.Extensions
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse response, MetaData metaData)
        {
            var options = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
            // レスポンスのヘッダーとして Pagination を加え、値は MetaData の json にする
            response.Headers.Add("Pagination", JsonSerializer.Serialize(metaData, options));
            // CORS 用のヘッダーを追加する。ヘッダー名はこれ以外ダメ。Pagination ヘッダーをクライアントが見れるようになる
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}