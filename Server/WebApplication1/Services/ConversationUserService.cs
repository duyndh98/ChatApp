using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.Entities;

namespace WebApplication1.Services
{
    public interface IConversationUserService
    {
        IEnumerable<ConversationUser> GetAll();
        ConversationUser Create(ConversationUser conversationUser);
        void Delete(ConversationUser conversationUser);

        void UpdateSeen(ConversationUser conversationUser, Message message);
    }

    public class ConversationUserService : IConversationUserService
    {
        private WebApplication1Context _context;

        public ConversationUserService(WebApplication1Context context)
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
                x => (x.ConversationId == conversationUser.ConversationId && x.UserId == conversationUser.UserId)))
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

        public void UpdateSeen(ConversationUser conversationUser, Message message)
        {
            // Check
            if (conversationUser.ConversationId != message.ConversationId)
                throw new Exception("Conversation is not match");

            var updatedConversationUser = conversationUser;
            updatedConversationUser.SeenMessageId = message.Id;

            // Update
            _context.ConversationUsers.Update(conversationUser);
            _context.SaveChanges();
        }
    }
}
