using System;
using System.Linq;
using System.Threading.Tasks;
using Core.DataAccess.EntityFramework;
using Entities.Concrete;
using DataAccess.Concrete.EntityFramework.Contexts;
using DataAccess.Abstract;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DataAccess.Concrete.EntityFramework
{
    public class HotelRepository : EfEntityRepositoryBase<Hotel, ProjectDbContext>, IHotelRepository
    {
        public HotelRepository(ProjectDbContext context) : base(context)
        {
        }

        public IQueryable<Hotel> GetHotelWithContactAsync()
        {
            return Context.Set<Hotel>()
                          .Include(h => h.Contacts);
        }
    }
}
