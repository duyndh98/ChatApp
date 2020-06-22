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
        IEnumerable<ConversationUser> GetAll();
        ConversationUser Create(ConversationUser conversationUser);
        void Delete(ConversationUser conversationUser);
    }

    public class ContactService : IContactService
    {
        private WebApplication1Context _context;

        public ContactService(WebApplication1Context context)
        {
            _context = context;
        }

        public IEnumerable<ConversationUser> GetAll()
        {
            return _context.ConversationUsers;
        }

        public ConversationUser Create(ConversationUser conversationUser)
        {
            // Check
            if (!_context.Conversations.Any(x => x.Id == conversationUser.ConversationId))
                throw new Exception("Conversation not found");

            if (!_context.Users.Any(x => x.Id == conversationUser.UserId))
                throw new Exception("User not found");

            if (_context.ConversationUsers.Any(
                x => x.ConversationId == conversationUser.ConversationId && x.UserId == conversationUser.UserId))
                throw new Exception("Member already exist");

            // Add
            _context.ConversationUsers.Add(conversationUser);
            _context.SaveChanges();

            // Ok
            return conversationUser;
        }

        public void Delete(ConversationUser conversationUser)
        {
            // Find
            if (!_context.ConversationUsers.Any(
                x => x.ConversationId == conversationUser.ConversationId && x.UserId == conversationUser.UserId))
                throw new Exception("Member not found");

            // Delete
            _context.ConversationUsers.Remove(conversationUser);
            _context.SaveChanges();
        }
    }
}
