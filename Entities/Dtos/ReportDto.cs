using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public class ReportDto
    {
        public string Location { get; set; }
        public int HotelCount { get; set; }
        public int PhoneCount { get; set; }
    }
}
