using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Entities;

namespace WebApplication1.Models
{
    public class ResourceCreationModel
    {
        public string Name { get; set; }

        public ResourceType Type { get; set; }

        public byte[] Data { get; set; }
    }
}
