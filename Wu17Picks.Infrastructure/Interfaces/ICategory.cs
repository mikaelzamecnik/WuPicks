using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wu17Picks.Data.Entities;

namespace Wu17Picks.Infrastructure.Interfaces
{
    public interface ICategory
    {
        Category GetById(int id);
        Task <IEnumerable<Category>> GetAll();
        Task AddCategory(Category vm);
    }
}
