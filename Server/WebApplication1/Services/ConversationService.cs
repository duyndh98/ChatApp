using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.Entities;
using WebApplication1.Helpers;

namespace WebApplication1.Services
{
    public interface IConversationService
    {
        IEnumerable<Conversation> GetAll();
        Conversation GetById(int id);
        Conversation Create(Conversation conversation);
        void Update(Conversation conversation);
        void Delete(int id);

        IEnumerable<ConversationUser> GetConversationUsers(int id);

        IEnumerable<Message> GetMessages(int id);
        Tuple<long, IEnumerable<Message>> GetNewMessages(int id, long lastTimeSpan);
        Message GetLastMessage(int id);

        bool Check2MembersAlreadyInConversation(User user1, User user2);

        void UpdateHostMember(int id, int userId);
    }

    public class ConversationService : IConversationService
    {
        private WebApplication1Context _context;

        public ConversationService(WebApplication1Context context)
        {
            _context = context;
        }

        public IEnumerable<Conversation> GetAll()
        {
            return _context.Conversations;
        }

        public Conversation GetById(int id)
        {
            var conversation = _context.Conversations.Find(id);
            if (conversation == null)
                throw new Exception("Conversation not found");

            return conversation;
        }

        public Conversation Create(Conversation conversation)
        {
            // Check
            if (string.IsNullOrEmpty(conversation.Name))
                throw new Exception("Name is required");

            // Add
            _context.Conversations.Add(conversation);
            _context.SaveChanges();

            // Ok
            return conversation;
        }

        public void Update(Conversation conversation)
        {
            // Find
            var updatedConversation = _context.Conversations.Find(conversation.Id);
            if (updatedConversation == null)
                throw new Exception("Conversation not found");

            if (string.IsNullOrEmpty(conversation.Name))
                throw new Exception("Name is required");
            else
                updatedConversation.Name = conversation.Name;

            // Update
            _context.Conversations.Update(updatedConversation);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            // Find
            var conversation = _context.Conversations.Find(id);
            if (conversation == null)
                throw new Exception("Conversation not found");

            // Delete
            _context.Conversations.Remove(conversation);
            _context.SaveChanges();
        }

        public IEnumerable<ConversationUser> GetConversationUsers(int id)
        {
            // Find
            var conversation = _context.Conversations.Find(id);
            if (conversation == null)
                throw new Exception("Conversation not found");

            return conversation.ConversationUsers;
        }

        public IEnumerable<Message> GetMessages(int id)
        {
            // Find
            var conversation = _context.Conversations.Find(id);
            if (conversation == null)
                throw new Exception("Conversation not found");

            return conversation.Messages;
        }

        public Tuple<long, IEnumerable<Message>> GetNewMessages(int id, long lastTimeSpan)
        {
            var messages = GetMessages(id);
            var filtedMessages = new List<Message>();
            foreach (var message in messages)
            {
                var messageTimeSpan = Utils.DateTimeToInt64(message.ArrivalTime);
                if (messageTimeSpan > lastTimeSpan)
                    filtedMessages.Add(message);
            }

            var lastTime = filtedMessages.Aggregate((x, y) => x.ArrivalTime > y.ArrivalTime ? x : y).ArrivalTime;
            lastTimeSpan = Utils.DateTimeToInt64(lastTime);

            return new Tuple<long, IEnumerable<Message>>(lastTimeSpan, filtedMessages);
        }

        public Message GetLastMessage(int id)
        {
            var messages = GetMessages(id);
            if (messages == null || messages.Count() == 0)
                throw new Exception("Conversation has no message");

            Message lastMessage = null;
            long lastTimeSpan = 0;
            foreach (var message in messages)
            {
                var messageTimeSpan = Utils.DateTimeToInt64(message.ArrivalTime);
                if (messageTimeSpan > lastTimeSpan)
                {
                    lastMessage = message;
                    lastTimeSpan = messageTimeSpan;
                }
            }

            return lastMessage;
        }

        public bool Check2MembersAlreadyInConversation(User user1, User user2)
        {
            foreach (var conversation in _context.Conversations)
            {
                if (conversation.ConversationUsers.Count == 2)
                {
                    var memberIds = conversation.ConversationUsers.Select(x => x.UserId);
                    if (memberIds.Contains(user1.Id) && memberIds.Contains(user2.Id))
                        return true;
                }
            }

            return false;
        }

        public void UpdateHostMember(int id, int userId)
        {
            // Find
            var conversation = _context.Conversations.Find(id);
            if (conversation == null)
                throw new Exception("Conversation not found");

            if (!conversation.ConversationUsers.Select(x => x.UserId).Contains(userId))
                throw new Exception("Member not found");

            // Update hosted user
            conversation.HostUserId = userId;
            
            // Update
            _context.Conversations.Update(conversation);
            _context.SaveChanges();
        }
    }
}
