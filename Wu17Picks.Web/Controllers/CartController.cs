using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Wu17Picks.Data.Entities;
using Wu17Picks.Services.Helpers;
using Wu17Picks.Services.Interfaces;

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
                    new Item() { GalleryImage = _imageService.GetById(id), Quantity = 1, IsSelected = true }
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
        // TODO download as Zip

        public IActionResult DownloadAsZip()
        {
            var sessionList = HttpContext.Session.GetObjectFromJson<List<Item>>("cart");
            // Howto loop trough current Urls in localstorage

            Guid guidName = Guid.NewGuid();
            string returnName = guidName.ToString() + ".zip";
            string rootPath = _hostingEnvironment.WebRootPath + "Azure" ;
            string zipPath = _hostingEnvironment.WebRootPath + "/zip/";

            var zip = ZipFile.Open(zipPath + returnName, ZipArchiveMode.Create);
            foreach (var file in sessionList)
            {
                zip.CreateEntryFromFile(rootPath +file, Path.GetFileName(rootPath + file), CompressionLevel.Optimal);
            }

            zip.Dispose();

            var zipReturn = @"~/zip/" + returnName;

            return File(zipReturn, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(zipReturn));
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