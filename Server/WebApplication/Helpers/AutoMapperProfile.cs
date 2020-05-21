using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Entities;
using WebApplication.Models;

namespace WebApplication.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, ViewUserModel>();
            CreateMap<RegisterUserModel, User>();
            CreateMap<UpdateUserInfoModel, User>();
            CreateMap<UpdateUserRoleModel, User>();
        }
    }
}
