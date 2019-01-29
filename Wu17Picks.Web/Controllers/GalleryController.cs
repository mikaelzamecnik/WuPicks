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

        //public async Task<IActionResult> DownloadImage(int id)
        //{
        //    var imageid = await _imageService.GetById(id);

        //    CloudBlockBlob blockBlob;
        //    MemoryStream ms = new MemoryStream();
        //    try
        //    {
        //        CloudBlobContainer container = DebitMemo.GetAzureContainer();
        //        blockBlob = container.GetBlockBlobReference(debitMemo.BlobName);
        //        await container.CreateIfNotExistsAsync();
        //        Save blob contents to a file.
        //        await blockBlob.DownloadToStreamAsync(ms);

        //        Stream blobStream = await blockBlob.OpenReadAsync();

        //        return File(blobStream, blockBlob.Properties.ContentType, debitMemo.BlobName);
        //    }
        //    catch (StorageException)
        //    {
        //        return Content("File does not exist");
        //    }
        //}
    }
}