using Core.Entities;
using Core.Enums;
using System;

namespace Entities.Dtos
{
    public class ContactDto : IDto
    {
        public ContactType Type { get; set; } 
        public string Value { get; set; } 
    }

}