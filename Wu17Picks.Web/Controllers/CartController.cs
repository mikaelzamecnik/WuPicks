using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Wu17Picks.Data.Entities;
using Wu17Picks.Services.Helpers;
using Wu17Picks.Services.Interfaces;
using Wu17Picks.Web.Models;

namespace Wu17Picks.Web.Controllers
{
    public class CartController : Controller
    {
        // TODO Refactor this entire controller

        private readonly IImage _imageService;
        private readonly string _basePath;

        public CartController(IImage imageService)
        {
            _imageService = imageService;
            _basePath = "https://wustore.blob.core.windows.net/images/";
        }

        public IActionResult Index()
        {
            var cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            ViewBag.cart = cart;
            ViewBag.total = cart.Sum(item => item.Quantity);
            return View();
        }
        [Route("add/{id}")]
        public IActionResult AddImage(int id)
        {
            if(SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart") == null)
            {
                var cart = new List<Item>
                {
                    new Item() { GalleryImage = _imageService.GetById(id), Quantity = 1 }
                };
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            else
            {
                var cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
                int index = Exists(cart, id);
                if(index == -1)
                {
                    cart.Add(new Item() { GalleryImage = _imageService.GetById(id), Quantity = 1 });
                }
                else
                {
                    cart[index].Quantity++;
                }
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Remove(int id)
        {
            //test CI
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            int index = Exists(cart, id);
            if(id > 0)
            {
                cart.RemoveAt(index);
            }
            if (id == 0)
            {
                cart.Clear();
            }
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            return RedirectToAction("Index");
        }
        // TODO download single file från cloud storage 
        public IActionResult SingleDownload()
        {
            return Ok("Nope");
        }
        // TODO download as zip
        public IActionResult DownloadAsZip()
            {
                var cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");

            if (cart == null || cart.Count == 0)
            {
                return BadRequest();
            } else

            {

                // Get images from Url in current session ???
                var images = _imageService.GetAll();
                byte[] bytes;

                using (var ms = new MemoryStream())
                {
                    using (var imagezip = new ZipArchive(ms, ZipArchiveMode.Create, true))
                        foreach (var image in images)
                            ms.CanRead.Equals(cart);

                            ms.Position = 0;
                    bytes = ms.ToArray();
                }
                return File(bytes, "application/zip", "images.zip");
            } 
            }
        private int Exists(List<Item> cart, int id)
        {
            for(int i = 0; i< cart.Count; i++)
            {
                if(cart[i].GalleryImage.Id == id)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}