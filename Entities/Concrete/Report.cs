using Core.Entities;
using Core.Enums;
using System;
using System.Collections.Generic;

namespace Entities.Concrete
{

    public class Report : IEntity
    {
        public Guid Id { get; set; } 
        public ReportStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Location { get; set; }
        public int HotelCount { get; set; } 
        public int PhoneCount { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;

    }



}