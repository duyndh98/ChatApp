using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebApplication1.Data;
using WebApplication1.Entities;
using WebApplication1.Helpers;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ConversationsController : ControllerBase
    {
        private IConversationService _conversationService;
        private IConversationUserService _conversationUserService;
        private IMessageService _messageService;
        private IUserService _userService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public ConversationsController(
              IConversationService conversationService,
              IConversationUserService conversationUserService,
              IMessageService messageService,
              IUserService userService,
              IMapper mapper,
              IOptions<AppSettings> appSettings)
        {
            _conversationService = conversationService;
            _conversationUserService = conversationUserService;
            _messageService = messageService;
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        // GET: api/Conversations
        [Authorize(Roles = Role.Admin)]
        [HttpGet]
        public IActionResult GetConversations()
        {
            try
            {
                var conversations = _conversationService.GetAll();
                var model = _mapper.Map<IList<ConversationViewModel>>(conversations);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/Conversations/5
        [HttpGet("{id}")]
        public IActionResult GetConversation(int id)
        {
            try
            {
                var conversation = _conversationService.GetById(id);
                var model = _mapper.Map<ConversationViewModel>(conversation);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/Conversations/5
        [HttpPut("{id}")]
        public IActionResult PutConversation(int id, [FromBody]ConversationUpdateModel model)
        {
            try
            {
                var conversation = _mapper.Map<Conversation>(model);
                conversation.Id = id;

                // Update
                _conversationService.Update(conversation);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/Conversations
        [HttpPost]
        public IActionResult PostConversation([FromBody]ConversationCreationModel model)
        {
            try
            {
                // Map model to entity
                var conversation = _mapper.Map<Conversation>(model);

                var currentUserId = Auth.GetUserIdFromClaims(this);
                conversation.HostUserId = currentUserId;

                // Create
                var createdConversation = _conversationService.Create(conversation);

                // Add current user as member
                _conversationUserService.Create(new ConversationUser()
                {
                    ConversationId = createdConversation.Id,
                    UserId = currentUserId
                });

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/Conversations/5
        [Authorize(Roles = Role.Admin)]
        [HttpDelete("{id}")]
        public IActionResult DeleteConversation(int id)
        {
            try
            {
                _conversationService.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("Members")]
        public IActionResult PostMember([FromBody]ConversationUserModel model)
        {
            try
            {
                // Map model to entity
                var member = _mapper.Map<ConversationUser>(model);

                // Create
                _conversationUserService.Create(member);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("Members")]
        public IActionResult DeleteMember([FromBody]ConversationUserModel model)
        {
            try
            {
                // Map model to entity
                var member = _mapper.Map<ConversationUser>(model);

                var conversation = _conversationService.GetById(model.ConversationId);
                var currentUserId = Auth.GetUserIdFromClaims(this);
                if (currentUserId != conversation.HostUserId)
                    throw new Exception("Have no right");
                    
                _conversationUserService.Delete(member);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("Members")]
        public IActionResult GetMembers(int id)
        {
            try
            {
                var conversationUsers = _conversationService.GetConversationUsers(id);
                var users = conversationUsers.Select(x => x.User);
                var model = _mapper.Map<IList<UserViewModel>>(users);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("Messages")]
        public IActionResult GetMessages(int id)
        {
            try
            {
                var messages = _conversationService.GetMessages(id);
                var model = _mapper.Map<IList<MessageViewModel>>(messages);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("Messages/New")]
        public IActionResult GetNewMessages(int id, long lastTimeSpan)
        {
            try
            {
                var timeMessages = _conversationService.GetNewMessages(id, lastTimeSpan);
                var time = timeMessages.Item1;
                var messages = timeMessages.Item2;
                var mappedMessages = _mapper.Map<IList<MessageViewModel>>(messages);
                return Ok(new Tuple<long, IList<MessageViewModel>>(time, mappedMessages));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("Messages/Last")]
        public IActionResult GetLastMessage(int id)
        {
            try
            {
                var lastMessage = _conversationService.GetLastMessage(id);
                var messageView = _mapper.Map<MessageViewModel>(lastMessage);
                return Ok(messageView);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("WithMembers")]
        public IActionResult PostConversationWithMembers([FromBody]ConversationCreationWithMemberModel model)
        {
            try
            {
                var members = new List<User>();

                // Add current member
                model.UserIds.Add(Auth.GetUserIdFromClaims(this));

                if (model.UserIds.Count <= 1)
                    throw new Exception("Too few members");

                foreach (var userId in model.UserIds)
                {
                    if (model.UserIds.Where(x => x == userId).Count() > 1)
                        throw new Exception("Duplicated members");

                    members.Add(_userService.GetById(userId));
                }

                // Check
                if (members.Count == 2)
                {
                    if (_conversationService.Check2MembersAlreadyInConversation(members[0], members[1]))
                        throw new Exception("Conversation already exist");
                }

                // Map model to entity
                var conversation = new Conversation()
                {
                    Name = model.Name,
                    HostUserId = model.UserIds.First()
                };

                // Create
                var createdConversation = _conversationService.Create(conversation);

                foreach (var user in members)
                {
                    _conversationUserService.Create(new ConversationUser()
                    {
                        ConversationId = createdConversation.Id,
                        UserId = user.Id
                    });
                }

                var conversationView = _mapper.Map<ConversationViewModel>(createdConversation);
                return Ok(conversationView);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/Conversations/5
        [HttpPut("HostMember/{id}")]
        public IActionResult PutConversationHostMember(int id, int userId)
        {
            try
            {
                // Get current host
                var hostUserId = _conversationService.GetById(id).HostUserId;
                
                // Check if current user is host
                var currentUserId = Auth.GetUserIdFromClaims(this);

                if (hostUserId != currentUserId)
                    throw new Exception("Have no right");                

                // Update
                _conversationService.UpdateHostMember(id, userId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
