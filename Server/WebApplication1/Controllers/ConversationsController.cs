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
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public ConversationsController(
              IConversationService conversationService,
              IConversationUserService conversationUserService,
              IMessageService messageService,
              IMapper mapper,
              IOptions<AppSettings> appSettings)
        {
            _conversationService = conversationService;
            _conversationUserService = conversationUserService;
            _messageService = messageService;
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

                // Create
                var createdConversation = _conversationService.Create(conversation);

                // Add current user as member
                _conversationUserService.Create(new ConversationUser()
                {
                    ConversationId = createdConversation.Id,
                    UserId = Auth.GetUserIdFromClaims(this)
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
    }
}
