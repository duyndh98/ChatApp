using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Entities;

namespace WebApplication1.Models
{
    public class ContactUpdateModel
    {
        public int ToUserId { get; set; }
        public ContactStatus Status { get; set; }
    }
}
