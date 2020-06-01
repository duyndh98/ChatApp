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
    public class ConversationUsersController : ControllerBase
    {
        private IConversationUserService _conversationUserService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public ConversationUsersController(
              IConversationUserService conversationUserService,
              IMapper mapper,
              IOptions<AppSettings> appSettings)
        {
            _conversationUserService = conversationUserService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        // GET: api/ConversationUsers
        [HttpGet]
        public IActionResult GetConversationUsers()
        {
            try
            {
                var conversations = _conversationUserService.GetAll();
                var model = _mapper.Map<IList<ConversationUserModel>>(conversations);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/ConversationUsers/5
        [HttpGet("{id}")]
        public IActionResult GetConversationUser(int id)
        {
            try
            {
                var conversation = _conversationUserService.GetById(id);
                var model = _mapper.Map<ConversationUserModel>(conversation);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/ConversationUsers/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public IActionResult PutConversationUser(int id, [FromBody]ConversationUserModel model)
        {
            try
            {
                var conversation = _mapper.Map<ConversationUser>(model);
                conversation.Id = id;

                // Update
                _conversationUserService.Update(conversation);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/ConversationUsers
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public IActionResult PostConversationUser([FromBody]ConversationUserModel model)
        {
            try
            {
                // Map model to entity
                var conversation = _mapper.Map<ConversationUser>(model);

                // Create
                _conversationUserService.Create(conversation);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/ConversationUsers/5
        [HttpDelete("{id}")]
        public IActionResult DeleteConversationUser(int id)
        {
            try
            {
                _conversationUserService.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
