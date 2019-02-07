using System;
using System.Collections.Generic;
using Wu17Picks.Data.Entities;
using Wu17Picks.Data.Models;

namespace Wu17Picks.Web.Models
{
    public class GalleryDetailModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedOn { get; set; }
        public string FileName { get; set; }
        public int CategoryId { get; set; }
        public GalleryImage Images { get; set; }
        public Category Categories { get; set; }
        public List<string> Tags { get; set; }

        public GalleryDetailModel()
        {
            Id = Id;
        }
    }
}
