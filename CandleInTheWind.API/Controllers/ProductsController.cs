using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CandleInTheWind.Data;
using CandleInTheWind.Models;
using CandleInTheWind.API.Models.Products;

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
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            var products = await _context.Products.Include(product => product.Category).ToListAsync();
            var productResponse = products.Select(products => ToProductDTO(products));
            return Ok(productResponse);
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            var product = await _context.Products.Include(product => product.Category).FirstOrDefaultAsync(product => product.Id == id);

            if (product == null)
            {
                return NotFound();
            }
            
            return ToProductDTO(product);
        }

        // GET: api/Products/Category?categoryId=4&pageSize=2&pageIndex=1
        [HttpGet("Category")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductByCategory([FromQuery]int categoryId, [FromQuery]int pageSize = 8, [FromQuery]int pageIndex = 1)
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
            var productResponse = products.Select(product => ToProductDTO(product));
            return Ok(new ProductFilterDTO()
            {
                ProductDTOs = productResponse,
                TotalPages = totalPages,
                PageSize = pageSize,
                PageIndex = pageIndex,
            });
        }

        // GET: api/Products/Filter?searchText=TinhDau
        [HttpGet("Filter")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductFilter([FromQuery]string searchText, [FromQuery] int pageSize = 8, [FromQuery] int pageIndex = 1)
        {
            if (pageSize <= 0) pageSize = 8;
            if (pageIndex <= 0) pageIndex = 1;

            var products_search = _context.Products
                                         .Include(product => product.Category)
                                         .Where(product => product.Name.Contains(searchText) || product.Description.Contains(searchText));

            int count = products_search.Count();
            int totalPages = count / pageSize + ((count % pageSize == 0) ? 0 : 1);
            if (pageIndex > totalPages)
                pageIndex = 1;


            var products = await products_search.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToListAsync();

            if (products.Count == 0)
                return NotFound();
            var productResponse = products.Select(product => ToProductDTO(product));
            return Ok(new ProductFilterDTO()
            {
                ProductDTOs = productResponse,
                TotalPages = totalPages,
                PageSize = pageSize,
                PageIndex = pageIndex,
            });
        }

        private ProductDTO ToProductDTO(Product product)
        {
            return new ProductDTO()
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                ImageUrl = product.ImageUrl,
                CategoryId = product.Category.Id,
                CategoryName = product.Category.Name,
            };
        }
    }
}
