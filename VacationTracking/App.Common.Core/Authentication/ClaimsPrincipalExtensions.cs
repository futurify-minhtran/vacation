using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace App.Common.Core.Authentication
{
    public static class ClaimsPrincipalExtensions
    {
        public static int? GetAccountId(this ClaimsPrincipal user)
        {
            if (user == null)
            {
                return null;
            }

            if (user.Claims == null)
            {
                return null;
            }

            int id = 0;

            string accountId = user.FindFirstValue("Account:Id");

            if (int.TryParse(accountId, out id))
            {
                return id;
            }
            else
            {
                return null;
            }
        }
    }
}
