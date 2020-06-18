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
    public class MessagesController : ControllerBase
    {
        private IMessageService _messageService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public MessagesController(
              IMessageService messageService,
              IMapper mapper,
              IOptions<AppSettings> appSettings)
        {
            _messageService = messageService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        // GET: api/Messages
        [Authorize(Roles = Role.Admin)]
        [HttpGet]
        public IActionResult GetMessages()
        {
            try
            {
                var messages = _messageService.GetAll();
                var model = _mapper.Map<IList<MessageViewModel>>(messages);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/Messages/5
        [HttpGet("{id}")]
        public IActionResult GetMessage(int id)
        {
            try
            {
                var message = _messageService.GetById(id);
                var model = _mapper.Map<MessageViewModel>(message);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/Messages/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public IActionResult PutMessage(int id, [FromBody]MessageUpdateModel model)
        {
            try
            {
                var message = _mapper.Map<Message>(model);
                message.Id = id;

                // Update
                _messageService.Update(message);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/Messages
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public IActionResult PostMessage([FromBody]MessageCreationModel model)
        {
            try
            {
                // Map model to entity
                var message = _mapper.Map<Message>(model);

                message.SenderId = Auth.GetUserIdFromClaims(this);

                // Create
                _messageService.Create(message);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/Messages/5
        [HttpDelete("{id}")]
        public IActionResult DeleteMessage(int id)
        {
            try
            {
                _messageService.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
