using CandleInTheWind.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandleInTheWind.API.Controllers
{
    [ApiController]
    [Route("api/category")]
    public class CategoryApiController : ControllerBase
    {
        private ApplicationDbContext _context;

        public CategoryApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var categories = _context.Products.ToList();
            return Ok(categories);
        }
    }
}
