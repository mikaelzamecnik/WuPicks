using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Wu17Picks.Data;
using Wu17Picks.Data.Models;

namespace Wu17Picks.Services
{
    public class ImageService : IImage
    {
        private readonly ApplicationDbContext _ctx;
        public ImageService(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }
        public IEnumerable<GalleryImage> GetAll()
        {
            return _ctx.GalleryImages
                .Include(img => img.Tags);
        }

        public GalleryImage GetById(int id)
        {
            return _ctx.GalleryImages.Find(id);
        }

        public IEnumerable<GalleryImage> GetWithTags(string tag)
        {
            return GetAll()
                .Where(img => img.Tags
                .Any(t => t.Description == tag));
        }
    }
}
