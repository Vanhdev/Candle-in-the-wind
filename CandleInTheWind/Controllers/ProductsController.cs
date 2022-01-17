using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CandleInTheWind.Data;
using CandleInTheWind.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Authorization;

namespace CandleInTheWind.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hosting;
        public ProductsController(ApplicationDbContext context, IWebHostEnvironment hosting)
        {
            _context = context;
            _hosting = hosting;
        }

        // GET: Products
        public async Task<IActionResult> Index(int page = 1, string search = "", Status status = Status.None)
        {
            //var products_categories = _context.Products.Include(p => p.Category);
            IQueryable<Product> products = _context.Products.Include(p => p.Category);
            if (!String.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                if (int.TryParse(search, out int id)) /// Get by Id
                {
                    var product = products.FirstOrDefault(p => p.Id == id);
                    return View(Models.PaginatedList<Product>.GetList(product, page, Constants.PageSize));
                }
                else
                {
                    products = products.Where(p => p.Name.Contains(search) || p.Category.Name.Contains(search));
                }
            }
            ViewBag.Filter = search;
            ViewBag.ChangeStatus = status;
            return View(await Models.PaginatedList<Product>.GetListAsync(products, page, Constants.PageSize));
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return PartialView("_Details", product);
            //return View(product);
        }

        // GET: Products/Create
        public IActionResult Create(Status status = Status.None)
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewBag.CreateStatus = status;
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,Stock,Image,CategoryId")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.ImageUrl = UploadImage(product);
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Create), new { status = Status.Success });
            }
            ViewBag.CreateStatus = Status.Fail;
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,Stock,ImageUrl,Image,CategoryId")] Product product)
        {
            if (product == null || id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (product.Image != null)
                    {
                        //TODO: _hosting.ContentRootPath
                        if (product.ImageUrl != null)
                            System.IO.File.Delete(Path.Combine(_hosting.ContentRootPath, "Images", product.ImageUrl));
                        product.ImageUrl = UploadImage(product);
                    }
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { status = Status.Success });
            }
            ViewBag.ChangeStatus = Status.Fail;
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }


        // POST: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();
            if (product.ImageUrl != null)
            {
                //TODO: _hosting.ContentRootPath
                System.IO.File.Delete(Path.Combine(_hosting.ContentRootPath, "Images", product.ImageUrl));
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { status = Status.Success });
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
        private string UploadImage(Product product)
        {
            string filename = null;
            if (product.Image != null)
            {
                filename = Guid.NewGuid().ToString() + "_" + product.Image.FileName;
                // TODO: _hosting.ContentRootPath
                string imageUrl = Path.Combine(_hosting.ContentRootPath, "Images", filename);
                using (FileStream fs = new FileStream(imageUrl, FileMode.Create))
                {
                    product.Image.CopyTo(fs);
                }
            }
            return filename;
        }
    }
}