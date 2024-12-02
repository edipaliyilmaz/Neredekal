using Core.Entities;
using Entities.Dtos;
using System;
using System.Collections.Generic;

namespace Entities.Concrete
{

    public class HotelDto 
    {
        public string ManagerFirstName { get; set; } 
        public string ManagerLastName { get; set; } 
        public string CompanyName { get; set; }
        public ICollection<ContactDto> Contacts { get; set; }

    }

}