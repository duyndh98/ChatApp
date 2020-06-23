﻿using System;
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
    public class ContactsController : ControllerBase
    {
        private IContactService _contactService;
        private IUserService _userService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public ContactsController(
            IContactService contactService,
            IUserService userService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _contactService = contactService;
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        // GET: api/Contacts
        [Authorize(Roles = Role.Admin)]
        [HttpGet]
        public IActionResult GetContacts()
        {
            try
            {
                var contacts = _contactService.GetAll();
                var model = _mapper.Map<IList<Contact>>(contacts);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/Contacts/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public IActionResult PutContact(int id, [FromBody]ContactUpdateModel model)
        {
            try
            {
                var contactUser = _userService.GetById(model.ToUserId);

                var contact = new Contact()
                {
                    FromUserId = Auth.GetUserIdFromClaims(this),
                    ToUserId = contactUser.Id,
                    Status = model.Status
                };
                
                // Update
                _contactService.Update(contact);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/Contacts
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public IActionResult PostContact([FromBody]ContactCreationModel model)
        {
            try
            {
                var contactUser = _userService.GetById(model.ToUserId);

                // Map model to entity
                var contact = new Contact()
                {
                    FromUserId = Auth.GetUserIdFromClaims(this),
                    ToUserId = contactUser.Id
                };

                // Create
                _contactService.Create(contact);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/Contacts/5
        [HttpDelete("{id}")]
        public IActionResult DeleteContact([FromBody]ContactDeletionModel model)
        {
            try
            {
                var contactUser = _userService.GetById(model.ToUserId);

                var contact = new Contact()
                {
                    FromUserId = Auth.GetUserIdFromClaims(this),
                    ToUserId = contactUser.Id,
                };

                _contactService.Delete(contact);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
