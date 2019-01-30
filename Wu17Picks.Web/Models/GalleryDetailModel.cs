using System;
using System.Collections.Generic;

namespace Wu17Picks.Web.Models
{
    public class GalleryDetailModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Url { get; set; }
        public int CategoryId { get; set; }
        public List<string> Tags { get; set; }

        public GalleryDetailModel()
        {
            Id = Id;
        }
    }
}
