using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wu17Picks.Data.Entities;
using Wu17Picks.Infrastructure.Interfaces;

namespace Wu17Picks.Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategory _categoryService;
        public CategoryController(ICategory categoryService)
        {
            _categoryService = categoryService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category vm)
        {
            await _categoryService.AddCategory(vm);
            return RedirectToAction("Index", "Gallery");
        }

    }
}