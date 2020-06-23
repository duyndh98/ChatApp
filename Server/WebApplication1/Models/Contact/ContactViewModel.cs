using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Entities;

namespace WebApplication1.Models
{
    public class ContactViewModel
    {
        public int FromUserId { get; set; }

        public int ToUserId { get; set; }

        public ContactStatus Status { get; set; }
    }
}
