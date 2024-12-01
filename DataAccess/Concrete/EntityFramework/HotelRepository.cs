
using System;
using System.Linq;
using Core.DataAccess.EntityFramework;
using Entities.Concrete;
using DataAccess.Concrete.EntityFramework.Contexts;
using DataAccess.Abstract;
namespace DataAccess.Concrete.EntityFramework
{
    public class HotelRepository : EfEntityRepositoryBase<Hotel, ProjectDbContext>, IHotelRepository
    {
        public HotelRepository(ProjectDbContext context) : base(context)
        {
        }
    }
}
