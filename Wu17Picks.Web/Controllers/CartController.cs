using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wu17Picks.Data.Entities;
using Wu17Picks.Services.Helpers;
using Wu17Picks.Services.Interfaces;

namespace Wu17Picks.Web.Controllers
{
    public class CartController : Controller
    {
        // TODO Refactor this entire controller

        private IImage _imageService;

        public CartController(IImage imageService)
        {
            _imageService = imageService;
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
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            int index = Exists(cart, id);
            if(id > 0)
            cart.RemoveAt(index);
            if (id == 0)
            cart.Clear();
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            return RedirectToAction("Index");
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