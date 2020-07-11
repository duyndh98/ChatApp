using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Entities
{
    public enum ResourceType
    {
        Unknown = 0,
        Image = 1
    };

    public class Resource
    {
        [Key]
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public ResourceType Type { get; set; }
    }
}
