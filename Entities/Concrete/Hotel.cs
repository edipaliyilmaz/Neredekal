using Core.Entities;
using System;
using System.Collections.Generic;

namespace Entities.Concrete
{

    public class Hotel : IEntity
    {
        public Guid Id { get; set; } 
        public string ManagerFirstName { get; set; } 
        public string ManagerLastName { get; set; } 
        public string CompanyName { get; set; }
        public ICollection<Contact> Contacts { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;

    }

}