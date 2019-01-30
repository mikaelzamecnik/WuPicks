using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wu17Picks.Data.Interfaces;
using Wu17Picks.Web.Models;

namespace Wu17Picks.Web.Controllers
{
    public class GalleryController : Controller
    {
        private readonly IImage _imageService;
        private ICategory _categoryService;
        public GalleryController(IImage imageService, ICategory categoryService)
        {
            _categoryService = categoryService;
            _imageService = imageService;
        }
        public IActionResult Index()
        {
            var cat = _categoryService.GetAll();
            var imageList = _imageService.GetAll();
            var model = new GalleryIndexModel()
            {
                Images = imageList,
                SearchQuery = "",
                Categories = cat
            };

            return View(model);
        }

        public IActionResult Detail(int id)
        {
            var image = _imageService.GetById(id);

            var model = new GalleryDetailModel()
            {
                Id = image.Id,
                Title = image.Title,
                CreatedOn = image.Created,
                Url = image.Url,
                CategoryId = image.CategoryId,
                Tags = image.Tags.Select(t => t.Description).ToList()
            };

            return View(model);
        }

    }
}