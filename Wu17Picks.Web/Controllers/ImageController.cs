using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Wu17Picks.Infrastructure.Extensions;
using Wu17Picks.Infrastructure.Interfaces;
using Wu17Picks.Web.Models;

namespace Wu17Picks.Web.Controllers
{
    public class ImageController : Controller
    {
        private readonly IDistributedCache _cache;
        private readonly IImage _imageService;
        private readonly ICategory _categoryService;
        private readonly string[] _supportedMimeTypes = { "image/png", "image/jpeg", "image/jpg" };

        private string AzureConnectionString { get; }

        public ImageController(IImage imageService, ICategory categoryService, IDistributedCache cache)
        {
            _imageService = imageService;
            _categoryService = categoryService;
            _cache = cache;
        }

        public IActionResult Upload()
        {
            var cat = _categoryService.GetAll();
            var model = new UploadImageModel()
            {
                Categories = cat
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UploadNewImage(IFormFile file, string tags, string title, int categoryid)
        {
            if (!_supportedMimeTypes.Contains(file.ContentType.ToString().ToLower()))
            {
                throw new NotSupportedException("Only jpeg and png are supported");
            }

            var container = _imageService.GetBlobContainer("images");

            var content = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            var blockBlob = container.GetBlockBlobReference(fileName);
            await blockBlob.UploadFromStreamAsync(file.OpenReadStream());
            await _imageService.SetImage(title, tags, categoryid, blockBlob.Uri);

            return RedirectToAction("Index", "Gallery");
        }
    }
}