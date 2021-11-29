using CandleInTheWind.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandleInTheWind.ViewComponents.Template
{
    [ViewComponent]
    public class CategoryDropdownViewComponent : ViewComponent
    {
        private ApplicationDbContext _context;

        public CategoryDropdownViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var categories = _context.Categories.ToList();
            return View("/Views/Components/CategoryDropdown.cshtml", categories);
        }
    }
}
