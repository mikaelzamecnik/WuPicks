using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Wu17Picks.Data;
using Wu17Picks.Data.Interfaces;
using Wu17Picks.Web.Models;

namespace Wu17Picks.Web.Controllers
{
    public class ImageController : Controller
    {
        private readonly IConfiguration _config;
        private IImage _imageService;
        private ICategory _categoryService;
        private readonly string[] _supportedMimeTypes = { "image/png", "image/jpeg", "image/jpg" };

        private string AzureConnectionString { get; }

        public ImageController(IConfiguration config, IImage imageService, ICategory categoryService)
        {
            _imageService = imageService;
            _categoryService = categoryService;
            _config = config;
            AzureConnectionString = _config["AzureStorageConnectionString"];
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

            var container = _imageService.GetBlobContainer(AzureConnectionString, "images");

            var content = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
            var fileName = content.FileName.Trim('"');

            // Get Ref to a block blob
            var blockBlob = container.GetBlockBlobReference(fileName);
            await blockBlob.UploadFromStreamAsync(file.OpenReadStream());
            await _imageService.SetImage(title, tags, categoryid, blockBlob.Uri);

            return RedirectToAction("Index", "Gallery");
        }
    }
}