using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Wu17Picks.Data.Entities;
using Wu17Picks.Infrastructure.Extensions;
using Wu17Picks.Infrastructure.Interfaces;

namespace Wu17Picks.Web.Controllers
{
    public class CartController : Controller
    {
        // TODO Refactor this entire controller

        private readonly IImage _imageService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly AppConfigHelper _appConfig;
        private readonly ILogger<CartController> _logger;

        public CartController(
            IImage imageService,
            IHostingEnvironment hostingEnvironment,
            IOptions<AppConfigHelper> appConfig,
            ILogger<CartController> logger)
        {
            _imageService = imageService;
            _hostingEnvironment = hostingEnvironment;
            _appConfig = appConfig.Value;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var first = _appConfig.BasePath;
            var second = _appConfig.AuxPath;

            // TODO Check if exists
            if (first == null)
            {
                ViewData["FilePath"] = second;
            }
            else
            {
                ViewData["FilePath"] = first;
            }
            var cart = SessionHelper.Get<List<Item>>(HttpContext.Session, "cart");
            ViewBag.cart = cart;
            if (cart != null)
            {
                ViewData["total"] = cart.Sum(item => item.Quantity);
                return View();
            }
            ViewData["total"] = 0;
            return RedirectToAction("Index", "Gallery");
        }

        [Route("add/{id}")]
        public IActionResult AddImage(int id)
        {
            if (SessionHelper.Get<List<Item>>(HttpContext.Session, "cart") == null)
            {
                var cart = new List<Item>
                {
                    new Item() { GalleryImage = _imageService.GetById(id), Quantity = 1 }
                };
                SessionHelper.Set(HttpContext.Session, "cart", cart);
            }
            else
            {
                var cart = SessionHelper.Get<List<Item>>(HttpContext.Session, "cart");
                int index = Exists(cart, id);
                if (index == -1)
                {
                    cart.Add(new Item() { GalleryImage = _imageService.GetById(id), Quantity = 1 });
                }
                else
                {
                    cart[index].Quantity++;
                }
                SessionHelper.Set(HttpContext.Session, "cart", cart);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Remove(int id)
        {
            List<Item> cart = SessionHelper.Get<List<Item>>(HttpContext.Session, "cart");
            int index = Exists(cart, id);
            if (id > 0)
            {
                cart.RemoveAt(index);
            }
            if (id == 0)
            {
                cart.Clear();
            }
            SessionHelper.Set(HttpContext.Session, "cart", cart);
            return RedirectToAction("Index", "Gallery");
        }

        public FileResult DownloadAsZip()
        {
            _logger.LogError("||||||||||||||||||DownLoadAsZip Log|||||||||||||");

            // Downloading Images to a folder
            var filePath = _appConfig.BasePath;
            var cart = HttpContext.Session.Get<List<Item>>("cart")
                .ConvertAll(item => item.GalleryImage.FileName);
            string rootPath = _hostingEnvironment.WebRootPath;
            using (WebClient client = new WebClient())
            {
                foreach (var image in cart)
                    client.DownloadFile(filePath + image, rootPath + image);
            }

            // Starting to add the dowloaded files to zip
            string newPath = _hostingEnvironment.WebRootPath + "/images/";
            byte[] bytes;
            DateTime fileName = DateTime.Now;

            using (var ms = new MemoryStream())
            {
                using (var imageCompression = new ZipArchive(ms, ZipArchiveMode.Create, true))
                    foreach (var image in Directory.GetFiles(newPath))
                    {
                        imageCompression.CreateEntryFromFile(image, Path.GetFileName(image), CompressionLevel.Fastest);
                    }
                ms.Position = 0;
                bytes = ms.ToArray();
            }

            // Purging the folder from the temporary images
            DirectoryInfo di = new DirectoryInfo(newPath);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
            
            // Returning Zip file to client
            return File(bytes, "application/zip", $"PicksImages-{fileName}.zip");
        }
        private int Exists(List<Item> cart, int id)
        {
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].GalleryImage.Id == id)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}