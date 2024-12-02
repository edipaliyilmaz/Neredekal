
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.DataAccess;
using Entities.Concrete;
namespace DataAccess.Abstract
{
    public interface IHotelRepository : IEntityRepository<Hotel>
    {
        IQueryable<Hotel> GetHotelWithContactAsync();
    }
}