using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wu17Picks.Data.Entities;

namespace Wu17Picks.Web.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}
