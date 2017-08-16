using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthenticationServer.Adapters;
using AuthenticationServer.Models.ViewModels;
using AuthenticationServer.Resources;
using AuthenticationServer.ServicesInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AuthenticationServer.Providers
{
    public class TokenProviderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TokenProviderOptions _options;

        public TokenProviderMiddleware(
            RequestDelegate next,
            IOptions<TokenProviderOptions> options)
        {
            _next = next;
            _options = options.Value;
        }
        
        public Task Invoke(HttpContext context)
        {
            // If the request path doesn't match, skip
            if (!context.Request.Path.Equals(_options.Path, StringComparison.Ordinal))
            {
                return _next(context);
            }

            // Request must be POST with Content-Type: application/x-www-form-urlencoded
            if (!context.Request.Method.Equals("POST")
               || !context.Request.HasFormContentType)
            {
                context.Response.StatusCode = 400;
                return context.Response.WriteAsync("Bad request.");
            }

            return GenerateToken(context);
        }
        
        private async Task GenerateToken(HttpContext context)
        {
            var email = context.Request.Form["email"].ToString();
            var password = context.Request.Form["password"].ToString();

            var _accountService = (IAccountService)context.RequestServices.GetService(typeof(IAccountService));

            //var _verifyService = (IVerificationService)context.RequestServices.GetService(typeof(IVerificationService));
            //var _rawRabbitClient = (IBusClient)context.RequestServices.GetService(typeof(IBusClient));

            var identity = await _accountService.CheckAsync(email, password);

            //response if account null or inactive
            if (identity == null || identity.Status == false)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 400;
                var code = Errors.INCORRECT_LOGIN;
                var message = Errors.INCORRECT_LOGIN_MSG;

                if (identity != null && identity.Status == false)
                {
                    code = Errors.ACCOUNT_INACTIVE;
                    message = Errors.ACCOUNT_INACTIVE_MSG;
                }

                await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                {
                    Code = code,
                    Message = message
                }, Formatting.Indented));

                return;
            }

            var permissions = await _accountService.GetPermissionsOfAccountAsync(identity.Id);

            var now = DateTime.Now;

            var encodedJwt = TokenProviderMiddleware.GenerateAccessToken(_options, now, identity.Email, identity.Id.ToString(), permissions.ToArray());

            var response = new SignInResponseModel
            {
                AccessToken = encodedJwt,
                Expires = now.AddSeconds((int)_options.Expiration.TotalSeconds),
                Account = identity.ToAccountViewModel()
            };

            // Serialize and return the response
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            }));
        }

        public static string GenerateAccessToken(TokenProviderOptions options, DateTime issuedTime, string email, string accountId, params string[] permissions)
        {
            // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
            // You can add other claims here, if you want:
            var claims = new Claim[]
            {

                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, (issuedTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds.ToString(), ClaimValueTypes.Integer64),
                new Claim("Account:Id", accountId)
            }.Concat(permissions.Select(p => new Claim(ClaimTypes.Role, p)));

            // Create the JWT and write it to a string
            var jwt = new JwtSecurityToken(
                issuer: options.Issuer,
                audience: options.Audience,
                claims: claims,
                notBefore: issuedTime,
                expires: issuedTime.Add(options.Expiration),
                signingCredentials: options.SigningCredentials);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }
        
    }
}
