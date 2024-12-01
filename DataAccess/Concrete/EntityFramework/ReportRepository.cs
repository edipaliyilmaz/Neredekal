
using System;
using System.Linq;
using Core.DataAccess.EntityFramework;
using Entities.Concrete;
using DataAccess.Concrete.EntityFramework.Contexts;
using DataAccess.Abstract;
namespace DataAccess.Concrete.EntityFramework
{
    public class ReportRepository : EfEntityRepositoryBase<Report, ProjectDbContext>, IReportRepository
    {
        public ReportRepository(ProjectDbContext context) : base(context)
        {
        }
    }
}
