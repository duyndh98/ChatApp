using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.Entities;

namespace WebApplication1.Services
{
    public interface IContactService
    {
        IEnumerable<Contact> GetAll();
        Contact Create(Contact contact);
        void Update(Contact contact);
        void Delete(Contact contact);

        IEnumerable<Contact> GetContacts(int userId);
    }

    public class ContactService : IContactService
    {
        private WebApplication1Context _context;

        public ContactService(WebApplication1Context context)
        {
            _context = context;
        }

        public IEnumerable<Contact> GetAll()
        {
            return _context.Contacts;
        }

        public IEnumerable<Contact> GetContacts(int userId)
        {
            return _context.Contacts.Where(x => x.FromUserId == userId || x.ToUserId == userId);
        }

        public Contact Create(Contact contact)
        {
            // Check
            if (contact.FromUserId == contact.ToUserId)
                throw new Exception("Invalid contact");

            if (_context.Contacts.Any(x => x.FromUserId == contact.FromUserId && x.ToUserId == contact.ToUserId)
                || _context.Contacts.Any(x => x.ToUserId == contact.FromUserId && x.FromUserId == contact.ToUserId))
                throw new Exception("Contact already exist");

            contact.Status = ContactStatus.Pending;

            // Add
            _context.Contacts.Add(contact);
            _context.SaveChanges();

            // Ok
            return contact;
        }

        public void Update(Contact contact)
        {
            // Find
            var foundContact = _context.Contacts.FirstOrDefault(x =>
                (x.FromUserId == contact.FromUserId && x.ToUserId == contact.ToUserId) ||
                (x.ToUserId == contact.FromUserId && x.FromUserId == contact.ToUserId));

            if (foundContact == null)
                throw new Exception("Contact not found");

            // Update
            _context.Contacts.Update(contact);
            _context.SaveChanges();
        }

        public void Delete(Contact contact)
        {
            // Find
            if (!_context.Contacts.Any(x => x.FromUserId == contact.FromUserId && x.ToUserId == contact.ToUserId)
                && !_context.Contacts.Any(x => x.ToUserId == contact.FromUserId && x.FromUserId == contact.ToUserId))
                throw new Exception("Contact not found");

            // Delete
            _context.Contacts.Remove(contact);
            _context.SaveChanges();
        }
    }
}
