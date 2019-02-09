using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Wu17Picks.Infrastructure.Extensions;
using Wu17Picks.Infrastructure.Interfaces;
using Wu17Picks.Web.Models;

namespace Wu17Picks.Web.Controllers
{
    public class GalleryController : Controller
    {
        private readonly IImage _imageService;
        private readonly ICategory _categoryService;
        private readonly AppConfigHelper _appConfig;
        private string filePath = "";
        public GalleryController(IImage imageService,
            ICategory categoryService,
            IOptions<AppConfigHelper> appConfig)
        {
            _categoryService = categoryService;
            _imageService = imageService;
            _appConfig = appConfig.Value;
        }


        public async Task<IActionResult> Index(string selectedCategory)
        {

            bool fileExist = _imageService.URLExists(_appConfig.BasePath);
            if (fileExist) { filePath = _appConfig.BasePath; }
            if(!fileExist) { filePath = _appConfig.AuxPath; }

            int categoryId = 0;
            if (!string.IsNullOrEmpty(selectedCategory))
            {
                var cat = _imageService.Categories.FirstOrDefault(c => c.Name.Equals(selectedCategory, StringComparison.InvariantCultureIgnoreCase));
                if (cat != null) categoryId = cat.Id;
            }

            var categorytext = await _categoryService.GetAll();
            var imageList = _imageService.GalleryImages
                .Where(p => selectedCategory == null ||
                p.Category.Name.Equals(selectedCategory, StringComparison.InvariantCultureIgnoreCase))
                .OrderByDescending(i=> i.Created);

            var model = new GalleryIndexModel()
            {
                Images = imageList,
                Categories = categorytext,
                SelectedCategory = selectedCategory,
                FilePath = filePath
            };

            return View(model);
        }
        public IActionResult Detail(int id)
        {

            bool fileExist = _imageService.URLExists(_appConfig.BasePath);
            if (fileExist) { filePath = _appConfig.BasePath; }
            if (!fileExist) { filePath = _appConfig.AuxPath; }

            var image = _imageService.GetById(id);

                var model = new GalleryDetailModel()
                {
                    Id = image.Id,
                    Title = image.Title,
                    CreatedOn = image.Created,
                    FileName = image.FileName,
                    CategoryId = image.CategoryId,
                    FilePath = filePath

                    // Need to redo tags service dont work atm
                    //Tags = image.Tags.Select(t => t.Description).ToList()
                };
                return View(model);
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}