using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Entities
{
    public static class Role
    {
        public const string Admin = "Admin";
        public const string User = "User";

        public static readonly string[] AvailableRoles = { Admin, User };
    }
}
