using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Wu17Picks.Data.Entities;
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
        public GalleryController(IImage imageService,
            ICategory categoryService,
            IOptions<AppConfigHelper> appConfig)
        {
            _categoryService = categoryService;
            _imageService = imageService;
            _appConfig = appConfig.Value;
        }
        public IActionResult Index(string selectedCategory)
        {

            var first = _appConfig.BasePath;
            var second = _appConfig.AuxPath;

            if (first == null)
            {
                ViewData["FilePath"] = second;
            } else
            {
                ViewData["FilePath"] = first;
            }
            int categoryId = 0;
            if (!string.IsNullOrEmpty(selectedCategory))
            {
                var cat = _imageService.Categories.FirstOrDefault(c => c.Name.Equals(selectedCategory, StringComparison.InvariantCultureIgnoreCase));
                if (cat != null) categoryId = cat.Id;
            }

            var categorytext = _categoryService.GetAll();
            var imageList = _imageService.GalleryImages
                .Where(p => selectedCategory == null ||
                p.Category.Name.Equals(selectedCategory, StringComparison.InvariantCultureIgnoreCase))
                .OrderByDescending(i=> i.Created);
            var model = new GalleryIndexModel()
            {
                Images = imageList,
                SearchQuery = "",
                Categories = categorytext,
                SelectedCategory = selectedCategory
            };

            return View(model);
        }
        public IActionResult Detail(int id)
        {

            var first = _appConfig.BasePath;
            var second = _appConfig.AuxPath;

            if (first == null)
            {
                ViewData["FilePath"] = second;
            }
            else
            {
                ViewData["FilePath"] = first;
            }
            var image = _imageService.GetById(id);
            try
            {
                var model = new GalleryDetailModel()
                {
                    Id = image.Id,
                    Title = image.Title,
                    CreatedOn = image.Created,
                    FileName = image.FileName,
                    CategoryId = image.CategoryId
                    // Need to redo tags service dont work atm
                    //Tags = image.Tags.Select(t => t.Description).ToList()
                };
                return View(model);
            }
            catch (Exception e)
            {

                throw e;
            }  
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}