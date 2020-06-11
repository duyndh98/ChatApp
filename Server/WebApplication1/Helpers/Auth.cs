using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApplication1.Helpers
{
    public static class Auth
    {
        public static int GetUserIdFromClaims(ControllerBase controller)
        {
            var claimsIdentity = controller.User.Identity as ClaimsIdentity;
            return int.Parse(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);
        }
    }
}
