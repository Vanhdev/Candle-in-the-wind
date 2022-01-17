using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CandleInTheWind.Data;
using CandleInTheWind.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Authorization;

namespace CandleInTheWind.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hosting;


        public CategoriesController(ApplicationDbContext context, IWebHostEnvironment hosting)
        {
            _context = context;
            _hosting = hosting;
        }
        // GET: Categories
        public async Task<IActionResult> Index(int page = 1, string search = "", Status status = Status.None, string message = "")
        {
            IQueryable<Category> categories = _context.Categories;
            if (!String.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                if (int.TryParse(search, out int id)) /// Get by Id
                {
                    var category = categories.FirstOrDefault(p => p.Id == id);
                    return View(Models.PaginatedList<Category>.GetList(category, page, Constants.PageSize));
                }
                else
                {
                    categories = categories.Where(p => p.Name.Contains(search));
                }
            }
            ViewBag.Filter = search;
            ViewBag.ChangeStatus = status;
            ViewBag.Message = message;
            return View(await Models.PaginatedList<Category>.GetListAsync(categories, page, Constants.PageSize));
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return PartialView("_Create");
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Category category)
        {
            string mess;
            if (ModelState.IsValid)
            {
                // TODO: Check exist category.
                var cat = _context.Categories.FirstOrDefault(c => c.Name.Equals(category.Name.Trim()));
                Status status = Status.Success;
                if (cat != null)
                {
                    mess = "Tên hạng mục sản phẩm đã tồn tại [<strong>Mã hạng mục = " + cat.Id + "</strong>]";
                    status = Status.Fail;
                }
                else
                {
                    mess = "Hạng mục sản phẩm được tạo <strong> Thành công </strong> !";
                    _context.Add(category);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index), new { status = status, message = mess });
            }
            //mess = "Yêu cầu tạo Hạng mục sản phẩm <strong>Thất bại</strong>. Xem chi tiết bên dưới!";
            return PartialView("_Create", category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return PartialView("_Edit", category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Category category)
        {
            if (category == null || id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var cat = _context.Categories.FirstOrDefault(c => c.Name.Equals(category.Name.Trim()));
                string mess;
                Status status = Status.Fail;
                if (cat != null)
                {
                    mess = "Tên hạng mục sản phẩm đã tồn tại [<strong>Mã hạng mục = " + cat.Id + "</strong>]";
                }
                else
                {
                    try
                    {
                        _context.Update(category);
                        await _context.SaveChangesAsync();
                        mess = "Cập nhật hạng mục sản phẩm <strong>Thành công</strong> !";
                        status = Status.Success;
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!CategoryExists(category.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                return RedirectToAction(nameof(Index), new { status = status, message = mess });
            }
            return PartialView("_Edit", category);
        }

        // POST: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var category = await _context.Categories.FindAsync(id);
            var pathList = _context.Products.Where(p => p.CategoryId == id).Select(p => p.ImageUrl);
            foreach (var url in pathList)
            {
                if (url != null)
                {
                    //TODO: _hosting.ContentRootPath
                    System.IO.File.Delete(System.IO.Path.Combine(_hosting.ContentRootPath, "Images", url));
                }
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { status = Status.Success, message = "Xoá hạng mục sản phẩm <strong>Thành công</strong> !" });
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}