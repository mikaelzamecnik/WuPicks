using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wu17Picks.Data;
using Wu17Picks.Data.Entities;
using Wu17Picks.Infrastructure.Extensions;
using Wu17Picks.Infrastructure.Interfaces;

namespace Wu17Picks.Infrastructure.Services
{
    public class CategoryService : ICategory
    {
        private const string _key = "categories";
        private readonly ApplicationDbContext _ctx;
        private IDistributedCache _cache;
        public CategoryService(ApplicationDbContext ctx, IDistributedCache cache)
        {
            _ctx = ctx;
            _cache = cache;
        }

        private List<Category> GetCache()
            => _cache.GetValue<List<Category>>(_key);

        private void SetCache(List<Category> toCache)
            => _cache.SetValue(_key, toCache);

        public IEnumerable<Category> GetAll()
        {
            var result = GetCache();

            if (result != null)
                return result;

            result = _ctx.Categories
                .OrderBy(x => x.Name)
                .Select(x => new Category()
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToList();

            SetCache(result);
            return result;

        }

        public Category GetById(int id)
        {
            return GetAll().Where(img => img.Id == id).First();
        }
        public async Task AddCategory(Category vm)
        {
            var cat = new Category
            {
                Id = vm.Id,
                Name = vm.Name
            };

            _ctx.Add(cat);
            await _ctx.SaveChangesAsync();

            var cached = GetCache();

            if (cached != null)
            {
                cached.Add(new Category()
                {
                    Id = vm.Id,
                    Name = vm.Name
                });

                SetCache(cached);
            }
        }
    }
}
