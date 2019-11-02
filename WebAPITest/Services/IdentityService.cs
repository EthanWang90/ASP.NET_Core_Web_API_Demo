using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebAPITest.Domain;
using WebAPITest.Options;

namespace WebAPITest.Services
{
    public class IdentityService: IIdentityService
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtOptions _jwtOptions;

        public IdentityService(UserManager<IdentityUser> userManager, JwtOptions jwtOptions)
        {
            _userManager = userManager;
            _jwtOptions = jwtOptions;
        }

        public async Task<AuthResult> LoginAsync(string email, string password)
        {
            IdentityUser user = await _userManager.FindByEmailAsync(email);
            if(user == null)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new String[]
                    {
                        "Sorry, user doesn't exist"
                    }
                };
            }

            string token = generateTokenHandler(user);

            return new AuthResult
            {
                Success = true,
                Token = token
            };
        }

        public async Task<AuthResult> RegisterAsync(string email, string password)
        {
            var existUser = await _userManager.FindByEmailAsync(email);
            if (existUser != null)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = new string[] {"The email has already been used!"}
                };
            }

            IdentityUser identityUser = new IdentityUser
            {
                Email = email,
                UserName = email
            };

            IdentityResult identityResult = await _userManager.CreateAsync(identityUser, password);
            if (!identityResult.Succeeded)
            {
                return new AuthResult
                {
                    Success = false,
                    Errors = identityResult.Errors.Select(x => x.Description)
                };
            }

            var token = generateTokenHandler(identityUser);
            return new AuthResult
            {
                Success = true,
                Token = token
            };
        }

        private string generateTokenHandler(IdentityUser identityUser)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtOptions.Secret);
            var tokenDescriber = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Email, identityUser.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub, identityUser.Email),
                    new Claim("id", identityUser.Id)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateToken(tokenDescriber);

            return tokenHandler.WriteToken(token);
        }
    }
}
