using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Entities
{
    public enum ContactStatus
    {
        Pending = 0,
        Accepted = 1,
        Declined = 2
    };

    public class Contact
    {
        [Key]
        public int FromUserId { get; set; }

        [Key]
        public int ToUserId { get; set; }

        public ContactStatus Status { get; set; }
    }
}
