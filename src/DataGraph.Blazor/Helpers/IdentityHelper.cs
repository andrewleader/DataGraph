using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DataGraph.Helpers
{
    public static class IdentityHelper
    {
        public static string GetCustomerId(this ClaimsPrincipal user)
        {
            return user.Claims.First().Value;
        }
    }
}
