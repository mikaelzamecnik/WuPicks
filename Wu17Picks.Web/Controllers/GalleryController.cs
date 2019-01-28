using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wu17Picks.Data;
using Wu17Picks.Data.Models;
using Wu17Picks.Web.Models;

namespace Wu17Picks.Web.Controllers
{
    public class GalleryController : Controller
    {
        private readonly IImage _imageService;
        public GalleryController(IImage imageService)
        {
            _imageService = imageService;
        }
        public IActionResult Index()
        {
            var imageList = _imageService.GetAll();
            var model = new GalleryIndexModel()
            {
                Images = imageList,
                SearchQuery = ""
            };

            return View(model);
        }
    }
}