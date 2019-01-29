using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wu17Picks.Data.Entities;

namespace Wu17Picks.Data.Interfaces
{
    public interface ICategory
    {
        Category GetById(int id);
        IEnumerable<Category> GetAll();
        Task Add(Category vm);

        // of use?
        //Task Delete(int id);
    }
}
