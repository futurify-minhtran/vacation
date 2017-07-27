using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationServer.ServicesInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

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
        /*
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
            var _verifyService = (IVerificationService)context.RequestServices.GetService(typeof(IVerificationService));
            //var _rawRabbitClient = (IBusClient)context.RequestServices.GetService(typeof(IBusClient));

            //if username is not an email
            if (username != null && !username.Contains("@"))
            {
                //try to format username as internationlazition phone number
                try
                {
                    //try to format with expectation that user filled country code
                    username = PhoneNumberHelpers.GetFormatedPhoneNumber(username);
                }
                catch (NumberParseException)
                {
                    //the username user provide is not an email or valid phone number
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                    {
                        Code = Errors.INCORRECT_LOGIN,
                        Custom = Errors.INVALID_PHONE_NUMBER,
                        Message = Errors.INCORRECT_LOGIN_MSG
                    }, Formatting.Indented));
                    return;
                }
            }

            var identity = await _accountService.CheckAsync(username, password);

            //response if account null or inactive
            if (identity == null || identity.Status == UserStatus.InActive || (identity.AccountType == AccountType.Jobseeker && username.Contains("@")))
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 400;
                var code = Errors.INCORRECT_LOGIN;
                var message = Errors.INCORRECT_LOGIN_MSG;
                if (identity != null && identity.Status == UserStatus.InActive)
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

            if (identity.AccountType == AccountType.Jobseeker && !identity.PhoneNumberVerified)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 400;

                //1 account has only 1 verification => get first
                var verification = (await _verifyService.GetVerificationsOfAccount(identity.Id)).FirstOrDefault();

                //account is locked because exceeded limit of retried or resend times
                if (verification.Retry >= VerificationService.MAX_RETRY || verification.Resend > VerificationService.MAX_RESEND)
                {
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                    {
                        Code = Errors.VERIFICATION_LOCKED,
                        Message = Errors.VERIFICATION_LOCKED_MSG
                    }, Formatting.Indented));
                }
                else //wait for verification
                {
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                    {
                        Code = Errors.WAIT_FOR_VERIFICATION,
                        Message = Errors.WAIT_FOR_VERIFICATION_MSG
                    }, Formatting.Indented));
                }
                return;
            }

            //add banana reward for first login in day
            if (identity.AccountType == AccountType.Jobseeker)
            {
                var tracker = await _accountService.AddTracker(new LoginTracker { Account = identity, LoginAt = DateTime.Now });
                if (tracker != null)
                {
                    await _rawRabbitClient.PublishAsync(new AccountLoggedIn { AccountId = identity.Id, LoginAt = tracker.LoginAt });
                }
            }

            var permissions = await _accountService.GetPermissionsOfAccountAsync(identity.Id);

            var now = DateTime.Now;

            var encodedJwt = TokenProviderMiddleware.GenerateAccessToken(_options, now, identity.UserName, identity.Id.ToString(), permissions.ToArray());

            var response = new SignInResponseModel
            {
                AccessToken = encodedJwt,
                Expires = now.AddSeconds((int)_options.Expiration.TotalSeconds),
                Account = identity.ToViewModel()
            };

            // Serialize and return the response
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            }));
        }

        public static string GenerateAccessToken(TokenProviderOptions options, DateTime issuedTime, string username, string accountId, params string[] permissions)
        {
            // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
            // You can add other claims here, if you want:
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
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
        */
    }
}
