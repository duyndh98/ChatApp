using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.Entities;
using WebApplication1.Helpers;

namespace WebApplication1.Services
{
    public interface IResourceService
    {
        IEnumerable<Resource> GetAll();
        Resource GetById(int id);
        Resource Create(Resource resource);
        void Delete(int id);
    }

    public class ResourceService : IResourceService
    {
        private WebApplication1Context _context;

        public ResourceService(WebApplication1Context context)
        {
            _context = context;
        }

        public IEnumerable<Resource> GetAll()
        {
            return _context.Resources;
        }

        public Resource GetById(int id)
        {
            var resource = _context.Resources.Find(id);
            if (resource == null)
                throw new Exception("Resource not found");

            return resource;
        }

        public Resource Create(Resource resource)
        {
            // Check
            if (string.IsNullOrEmpty(resource.Name))
                throw new Exception("Name is required");

            if (string.IsNullOrEmpty(resource.Code))
                throw new Exception("Code is required");

            // Add
            _context.Resources.Add(resource);
            _context.SaveChanges();

            // Ok
            return resource;
        }

        public void Delete(int id)
        {
            // Find
            var resource = _context.Resources.Find(id);
            if (resource == null)
                throw new Exception("Resource not found");

            // Delete
            _context.Resources.Remove(resource);
            _context.SaveChanges();
        }
    }
}
