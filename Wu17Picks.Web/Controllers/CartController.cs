using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public CartController(IImage imageService, IHostingEnvironment hostingEnvironment)
        {
            _imageService = imageService;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var cart = SessionHelper.Get<List<Item>>(HttpContext.Session, "cart");
            ViewBag.cart = cart;
            if (cart != null)
            {
                ViewBag.total = cart.Sum(item => item.Quantity);
                return View();
            }
            ViewBag.total = 0;
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
            return RedirectToAction("Index");
        }

        // TODO download as Zip

        public IActionResult DownloadAsZip()
        {
            var cart = HttpContext.Session.Get<List<Item>>("cart")
                .ConvertAll(item => item.GalleryImage.Url);

            if (cart == null)
                return BadRequest();
            byte[] bytes;
            DateTime fileName = DateTime.Now;
            using (var ms = new MemoryStream())
            {
                using (var imageCompression = new ZipArchive(ms, ZipArchiveMode.Create, true))
                    foreach (var image in cart)
                        // Does Looping Trough strings of Url dresses work ?
                        imageCompression.CreateEntry(image, CompressionLevel.Fastest);
                ms.Position = 0;
                bytes = ms.ToArray();
            }

            return File(bytes, "application/zip", $"{fileName}.zip");
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