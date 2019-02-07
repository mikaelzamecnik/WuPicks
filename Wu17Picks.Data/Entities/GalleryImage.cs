using System;
using System.Collections.Generic;
using Wu17Picks.Data.Entities;

namespace Wu17Picks.Data.Models
{
    public class GalleryImage
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Created { get; set; }
        public string FileName { get; set; }
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public virtual IEnumerable<ImageTag> Tags { get; set; }
    }
}
