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
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public ConversationsController(
              IConversationService conversationService,
              IMapper mapper,
              IOptions<AppSettings> appSettings)
        {
            _conversationService = conversationService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        // GET: api/Conversations
        [HttpGet]
        public IActionResult GetConversations()
        {
            try
            {
                var conversations = _conversationService.GetAll();
                var model = _mapper.Map<IList<ConversationModel>>(conversations);
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
                var model = _mapper.Map<ConversationModel>(conversation);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/Conversations/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public IActionResult PutConversation(int id, [FromBody]ConversationModel model)
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
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public IActionResult PostConversation([FromBody]ConversationModel model)
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
    }
}
