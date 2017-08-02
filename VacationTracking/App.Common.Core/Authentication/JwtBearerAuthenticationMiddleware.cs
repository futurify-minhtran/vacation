using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;

namespace App.Common.Core.Authentication
{
    public static class JwtBearerAuthenticationMiddleware
    {
        public static void UseJwt(this IApplicationBuilder app, string secretKey, string audience, string issuer)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
            var tokenValidationParameters = new TokenValidationParameters
            {
                // The signing key must match!
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = issuer,

                // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudience = audience,

                // Validate the token expiry
                ValidateLifetime = true,

                // If you want to allow a certain amount of clock drift, set that here:
                ClockSkew = TimeSpan.Zero
            };

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters
            });
        }
    }
}
