using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using API.Controllers;
using API.Data;
using API.Entities;
using API.Extensions;
using API.RequestHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.API.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly StoreContext _context;
        public ProductsController(StoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<Product>>> GetProducts([FromQuery] ProductParams productParams)
        // コントローラーの戻り値は ActionResult 型である必要がある
        // 属性で引数を指定していない場合はクエリストリングから取得される // api/products?orderBy="price"
        {
            var query = _context.Products
                            .Sort(productParams.OrderBy) // Sort() は API.Extensions で自分で作成したメソッド。以下全部同じ
                            .Search(productParams.SearchTerm)
                            .Filter(productParams.Brands, productParams.Types)
                            .AsQueryable();
            // products は MetaData 付きの product の List
            var products = await PagedList<Product>.ToPagedList(query, productParams.PageNumber, productParams.PageSize);

            Response.AddPaginationHeader(products.MetaData);

            return products;
        }

        [HttpGet("{id}")] // api/products/3 (3 が id)
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null) return NotFound();

            return product;
        }

        [HttpGet("filters")]
        public async Task<IActionResult> GetFilters()
        {
            var brands = await _context.Products.Select(p => p.Brand).Distinct().ToListAsync();
            var types = await _context.Products.Select(p => p.Type).Distinct().ToListAsync();

            return Ok(new { brands, types });
        }
    }
}