using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace OnlineLearning.Services
{
    public class StringeeService
    {
        private readonly IConfiguration _configuration;

        public StringeeService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateAccessToken(string userId)
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var exp = now + 3600 * 24;
            var apiSid = _configuration["Stringee:ApiSid"];
            var apiSecret = _configuration["Stringee:ApiSecret"];
            var header = new JwtHeader(
                new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(apiSecret)),
                    SecurityAlgorithms.HmacSha256
                )
            );
          //  header["cty"] = _stringeeOption.Cty;

            var payload = new JwtPayload
        {
        { "jti", $"{apiSid}-{now}" },
        { "iss", apiSid },
        { "exp", exp },
        { "userId", userId }
        };

            var jwt = new JwtSecurityToken(header, payload);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(jwt);

            return tokenString;
        }
    }
}
