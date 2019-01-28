using System;
using System.Collections.Generic;
using System.Text;
using Wu17Picks.Data.Models;

namespace Wu17Picks.Data
{
    public interface IImage
    {
        IEnumerable<GalleryImage> GetAll();
        IEnumerable<GalleryImage> GetWithTags(string tag);
        GalleryImage GetById(int id);
    }
}
