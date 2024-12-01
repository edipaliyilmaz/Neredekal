using Core.Entities;
using Core.Enums;
using System;

namespace Entities.Concrete
{
    public class Contact : IEntity
    {
        public Guid Id { get; set; } 
        public Guid HotelId { get; set; } 
        public ContactType Type { get; set; } 
        public string Value { get; set; } 
        public Hotel Hotel { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
    }


}