using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.Entities;
using WebApplication1.Helpers;

namespace WebApplication1.Services
{
    public interface IMessageService
    {
        IEnumerable<Message> GetAll();
        Message GetById(int id);
        Message Create(Message message);
        void Update(Message message);
        void Delete(int id);

        IEnumerable<Message> GetMessagesOfConversation(int conversationId);
        Tuple<long, IEnumerable<Message>> GetNewMessagesOfConversation(int conversationId, long lastTimeSpan);
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
            // Check            
            if (!_context.Conversations.Any(x => x.Id == message.ConversationId))
                throw new Exception("Conversation not found");

            if (!_context.Users.Any(x => x.Id == message.SenderId))
                throw new Exception("User not found");

            if (!_context.ConversationUsers.Any(
                x => x.ConversationId == message.ConversationId && x.UserId == message.SenderId))
                throw new Exception("User is not member of the conversation");

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

        public IEnumerable<Message> GetMessagesOfConversation(int conversationId)
        {
            // Check
            if (!_context.Conversations.Any(x => x.Id == conversationId))
                throw new Exception("Conversation not found");

            return _context.Messages.Where(x => x.ConversationId == conversationId);
        }

        public Tuple<long, IEnumerable<Message>> GetNewMessagesOfConversation(int conversationId, long lastTimeSpan)
        {
            //var lastTime = Utils.Int64ToDateTime(lastTimeSpan);
            var messages = GetMessagesOfConversation(conversationId);
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
    }
}
