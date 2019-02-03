using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
        private readonly IHostingEnvironment _hostingEnvironment;
        public CartController(IImage imageService, IHostingEnvironment environment)
        {
            _imageService = imageService;
            _hostingEnvironment = environment;
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
                    cart.Add(new Item() { GalleryImage = _imageService.GetById(id), Quantity = 1, IsSelected = true });
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