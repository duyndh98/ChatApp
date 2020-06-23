using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Entities;
using WebApplication1.Models;

namespace WebApplication1.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserViewModel>();
            CreateMap<UserAuthenticationModel, User>();
            CreateMap<UserRegistrationModel, User>();
            CreateMap<UserInfoUpdateModel, User>();
            CreateMap<UserRoleUpdateModel, User>();
            CreateMap<Conversation, ConversationViewModel>();
            CreateMap<ConversationCreationModel, Conversation>();
            CreateMap<ConversationUpdateModel, Conversation>();
            CreateMap<ConversationUserModel, ConversationUser>();
            CreateMap<ConversationUser, ConversationUserModel>();

            CreateMap<MessageCreationModel, Message>();
            CreateMap<MessageUpdateModel, Message>();
            CreateMap<Message, MessageViewModel> ();

            CreateMap<Contact, ContactViewModel>();            
        }
    }
}
