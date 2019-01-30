using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wu17Picks.Data.Entities;
using Wu17Picks.Data.Models;

namespace Wu17Picks.Web.Models
{
    public class GalleryIndexModel
    {
        public IEnumerable<GalleryImage> Images { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public int CategoryId { get; set; }
        public string SearchQuery { get; set; }
    }
}
