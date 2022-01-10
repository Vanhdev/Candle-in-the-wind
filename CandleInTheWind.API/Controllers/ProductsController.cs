using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CandleInTheWind.Data;
using CandleInTheWind.Models;
using CandleInTheWind.API.Models.Products;
using CandleInTheWind.API.Extensions;

namespace CandleInTheWind.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult> GetProducts()
        {
            var products = await _context.Products.Include(product => product.Category).ToListAsync();
            var productResponse = products.Select(product => product.ToDTO());
            return Ok(productResponse);
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetProduct(int id)
        {
            var product = await _context.Products
                                        .Include(product => product.Category)
                                        .FirstOrDefaultAsync(product => product.Id == id);

            if (product == null)
            {
                return NotFound();
            }
            
            return Ok(product.ToDTO());
        }

        // GET: api/Products/Category?categoryId=4&pageSize=2&pageIndex=1
        [HttpGet("Category")]
        public async Task<ActionResult> GetProductByCategory([FromQuery]int categoryId, [FromQuery]int pageSize = 8, [FromQuery]int pageIndex = 1)
        {
            if (pageSize <= 0) pageSize = 8;
            if (pageIndex <= 0) pageIndex = 1;

            var products_category = _context.Products
                                         .Include(products => products.Category)
                                         .Where(product => product.Category.Id == categoryId);

            int count = products_category.Count();
            int totalPages = count / pageSize + ((count % pageSize == 0) ? 0 : 1);
            if (pageIndex > totalPages)
                pageIndex = 1;
                
            var products = await products_category.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToListAsync();

            if (products.Count == 0)
                return NotFound();
            var productResponse = products.Select(product => product.ToDTO());

            return Ok(new ProductFilterDTO()
            {
                Products = productResponse,
                TotalPages = totalPages,
                PageSize = pageSize,
                PageIndex = pageIndex,
            });
        }

        // GET: api/Products/Filter?searchText=Tinh%20Dau&pageSize=8&pageIndex=1
        [HttpGet("Filter")]
        public async Task<ActionResult> GetProductFilter([FromQuery]string searchText, [FromQuery] int pageSize = 8, [FromQuery] int pageIndex = 1)
        {
            if (pageSize <= 0) pageSize = 8;
            if (pageIndex <= 0) pageIndex = 1;

            var products_search = _context.Products
                                         .Include(product => product.Category)
                                         .Where(product => product.Name.Contains(searchText) || 
                                                product.Description.Contains(searchText));

            int count = products_search.Count();
            int totalPages = count / pageSize + ((count % pageSize == 0) ? 0 : 1);
            if (pageIndex > totalPages)
                pageIndex = 1;


            var products = await products_search.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToListAsync();

            if (products.Count == 0)
                return NotFound();

            var productResponse = products.Select(product => product.ToDTO());
            return Ok(new ProductFilterDTO()
            {
                Products = productResponse,
                TotalPages = totalPages,
                PageSize = pageSize,
                PageIndex = pageIndex,
            });
        }

        [HttpGet("SpecialProduct")]
        public async Task<ActionResult> GetSpecialProduct()
        {
            var ids = new[]{ 18, 13, 12, 8};
            var products = await _context.Products
                                        .Include(product => product.Category)
                                        .Where(product => ids.Contains(product.Id)).ToListAsync();

            var productResponse = products.Select(product => product.ToDTO());
            return Ok(productResponse);
        }
    }
}
