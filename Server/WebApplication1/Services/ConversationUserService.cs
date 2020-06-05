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
        ConversationUser GetById(int id);
        ConversationUser Create(ConversationUser conversationUser);
        //void Update(ConversationUser conversationUser);
        void Delete(int id);
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

        public ConversationUser GetById(int id)
        {
            var conversationUser = _context.ConversationUsers.Find(id);
            if (conversationUser == null)
                throw new Exception("ConversationUser not found");

            return conversationUser;
        }

        public ConversationUser Create(ConversationUser conversationUser)
        {
            // Add
            _context.ConversationUsers.Add(conversationUser);
            _context.SaveChanges();

            // Ok
            return conversationUser;
        }

        //public void Update(ConversationUser conversationUser)
        //{
        //    // Find
        //    var updatedConversation = _context.ConversationUsers.Find(conversationUser.Id);
        //    if (updatedConversation == null)
        //        throw new Exception("ConversationUser not found");

        //    // Update
        //    _context.ConversationUsers.Update(updatedConversation);
        //    _context.SaveChanges();
        //}

        public void Delete(int id)
        {
            // Find
            var conversationUser = _context.ConversationUsers.Find(id);
            if (conversationUser == null)
                throw new Exception("ConversationUser not found");

            // Delete
            _context.ConversationUsers.Remove(conversationUser);
            _context.SaveChanges();
        }
    }
}
