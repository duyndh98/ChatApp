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
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public ConversationsController(
              IConversationService conversationService,
              IConversationUserService conversationUserService,
              IMapper mapper,
              IOptions<AppSettings> appSettings)
        {
            _conversationService = conversationService;
            _conversationUserService = conversationUserService;
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
                _conversationService.Create(conversation);
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
        public IActionResult PostMember([FromBody]ConversationMemberModel model)
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
        public IActionResult DeleteMember([FromBody]ConversationMemberModel model)
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
                var members = _conversationUserService.GetMembers(id);
                var model = _mapper.Map<IList<ConversationMemberModel>>(members);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
