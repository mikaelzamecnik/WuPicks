using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wu17Picks.Data;
using Wu17Picks.Data.Entities;
using Wu17Picks.Data.Interfaces;

namespace Wu17Picks.Services
{
    public class CategoryService : ICategory
    {
        private readonly ApplicationDbContext _ctx;
        public CategoryService(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }
        public IEnumerable<Category> GetAll()
        {
            return _ctx.Categories;
        }

        public Category GetById(int id)
        {
            return GetAll().Where(img => img.Id == id).First();
        }
        public async Task Add(Category vm)
        {
            var cat = new Category
            {
                Name = vm.Name
            };

            _ctx.Add(cat);
            await _ctx.SaveChangesAsync();
        }

        // Of use?

        //public Task Delete(int id)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
