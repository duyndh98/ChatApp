using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.Data;
using WebApplication1.Entities;
using WebApplication1.Helpers;
using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.Helpers;

namespace WebApplication1.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        private IConversationUserService _conversationUserService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public UsersController(
              IUserService userService,
              IConversationUserService conversationUserService,
              IMapper mapper,
              IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _conversationUserService = conversationUserService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("Authenticate")]
        public IActionResult Authenticate([FromBody]UserAuthenticationModel model)
        {
            try
            {
                var user = _userService.Authenticate(model.Username, model.Password);

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                // Return basic user info and authentication token
                return Ok(new
                {
                    Id = user.Id,
                    Username = user.UserName,
                    FullName = user.FullName,
                    Role = user.Role,
                    Token = tokenString
                });
            }
            catch (Exception ex)
            {
                // Return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register([FromBody]UserRegistrationModel model)
        {
            try
            {
                // Map model to entity
                var user = _mapper.Map<User>(model);
                
                // Default role is user
                user.Role = Role.User;

                // Create
                _userService.Create(user, model.Password);
                return Ok();
            }
            catch (Exception ex)
            {
                // Return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.Admin)]
        [HttpGet]
        public IActionResult GetUsers()
        {
            try
            {
                var users = _userService.GetAll();
                var model = _mapper.Map<IList<UserViewModel>>(users);
                return Ok(model);
            }
            catch (Exception ex)
            {
                // Return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            try
            {
                var user = _userService.GetById(id);
                var model = _mapper.Map<UserViewModel>(user);
                return Ok(model);
            }
            catch (Exception ex)
            {
                // Return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("Owner")]
        public IActionResult GetUserOwner()
        {
            try
            {
                var userId = Auth.GetUserIdFromClaims(this);
                var user = _userService.GetById(userId);
                var model = _mapper.Map<UserViewModel>(user);
                return Ok(model);
            }
            catch (Exception ex)
            {
                // Return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("Owner")]
        public IActionResult PutUserOwnerInfo([FromBody]UserInfoUpdateModel model)
        {
            try
            {
                // Map model to entity and set id
                var user = _mapper.Map<User>(model);
                user.Id = Auth.GetUserIdFromClaims(this);

                // Update 
                _userService.Update(user, model.Password);
                return Ok();
            }
            catch (Exception ex)
            {
                // Return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPut("Grant/{id}")]
        public IActionResult PutUserRole(int id, [FromBody]UserRoleUpdateModel model)
        {
            try
            {
                // Map model to entity and set id
                var user = _mapper.Map<User>(model);
                user.Id = id;

                // Update 
                _userService.Update(user, null);
                return Ok();
            }
            catch (Exception ex)
            {
                // Return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.Admin)]
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                _userService.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                // Return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("Owner/Conversations")]
        public IActionResult GetConversations()
        {
            try
            {
                var conversationUsers = _userService.GetConversationUsers(Auth.GetUserIdFromClaims(this));
                var conversations = conversationUsers.Select(x => x.Conversation);
                var model = _mapper.Map<IList<ConversationViewModel>>(conversations);
                return Ok(model);
            }
            catch (Exception ex)
            {
                // Return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}