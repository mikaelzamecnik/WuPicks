﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wu17Picks.Infrastructure.Interfaces;

namespace Wu17Picks.Web.ViewComponents
{
    public class CategoryViewComponent : ViewComponent
    {
        private readonly IImage _imageService;

        public CategoryViewComponent(IImage imageService)
        {
            _imageService = imageService;
        }

        public IViewComponentResult Invoke()
        {
            ViewBag.SelectedCategory = RouteData?.Values["SelectedCategory"];
            var categories = _imageService.Categories.Select(x => x.Name).OrderBy(c => c);
            return View(categories);
        }


    }
}
