using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.Entities;

namespace WebApplication1.Services
{
    public interface IMessageService
    {
        IEnumerable<Message> GetAll();
        Message GetById(int id);
        Message Create(Message message);
        void Update(Message message);
        void Delete(int id);
    }

    public class MessageService : IMessageService
    {
        private WebApplication1Context _context;

        public MessageService(WebApplication1Context context)
        {
            _context = context;
        }

        public IEnumerable<Message> GetAll()
        {
            return _context.Messages;
        }

        public Message GetById(int id)
        {
            var message = _context.Messages.Find(id);
            if (message == null)
                throw new Exception("Message not found");

            return message;
        }

        public Message Create(Message message)
        {
            // Validate
            if (string.IsNullOrEmpty(message.Content))
                throw new Exception("Content is required");

            message.ArrivalTime = DateTime.Now;

            // Add
            _context.Messages.Add(message);
            _context.SaveChanges();

            // Ok
            return message;
        }

        public void Update(Message message)
        {
            // Find
            var updatedMessage = _context.Messages.Find(message.Id);
            if (updatedMessage == null)
                throw new Exception("Message not found");

            if (!string.IsNullOrEmpty(message.Content))
                throw new Exception("Content is required");

            // Update
            _context.Messages.Update(updatedMessage);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            // Find
            var message = _context.Messages.Find(id);
            if (message == null)
                throw new Exception("Message not found");

            // Delete
            _context.Messages.Remove(message);
            _context.SaveChanges();
        }
    }
}
