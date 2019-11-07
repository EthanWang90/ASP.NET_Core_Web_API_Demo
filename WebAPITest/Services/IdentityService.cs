using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebAPITest.Data;
using WebAPITest.Domain;
using WebAPITest.Options;

namespace WebAPITest.Services
{
    public class IdentityService: IIdentityService
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtOptions _jwtOptions;
        private readonly TokenValidationParameters _tokenValidationParams;
        private readonly ApplicationDbContext _dbContext;

        public IdentityService(UserManager<IdentityUser> userManager, JwtOptions jwtOptions, TokenValidationParameters tokenValidationParams, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _jwtOptions = jwtOptions;
            _tokenValidationParams = tokenValidationParams;
            _dbContext = dbContext;
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

            AuthResult authResult = await generateTokenAsync(user);

            return authResult;
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

            AuthResult authResult = await generateTokenAsync(identityUser);
            return authResult;
        }

        public async Task<AuthResult> RefreshTokenAsync(string token, string refreshToken)
        {
            var validatedToken = GetPrincipalFromToken(token);

            if(validatedToken == null)
            {
                return new AuthResult
                {
                    Errors = new string[]
                    {
                        "Invalid Token"
                    }
                };
            }

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            var storeRefreshToken = await _dbContext.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken);
            if(storeRefreshToken == null)
            {
                return new AuthResult
                {
                    Errors = new string[]
                    {
                        "refresh token doesn't exist"
                    }
                };
            }

            if(DateTime.UtcNow > storeRefreshToken.ExpiryDate)
            {
                return new AuthResult
                {
                    Errors = new String[]
                    {
                        "This refresh token has expired"
                    }
                };
            }

            if(jti != storeRefreshToken.JwtId)
            {
                return new AuthResult
                {
                    Errors = new String[]
                    {
                        "Jwt Id is not same"
                    }
                };
            }

            IdentityUser user = await _userManager.Users.SingleOrDefaultAsync(x => x.Id == storeRefreshToken.UserId);
            AuthResult authResult = await generateTokenAsync(user);
            return authResult;
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParams, out var validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }
                return principal;
            }
            catch
            {
                return null;
            }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) && 
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase);
        }

        private async Task<AuthResult> generateTokenAsync(IdentityUser identityUser)
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
                Expires = DateTime.UtcNow.Add(_jwtOptions.TokenLifetime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateToken(tokenDescriber);

            RefreshToken refreshToken = new RefreshToken
            {
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                JwtId = token.Id,
                UserId = identityUser.Id
            };

            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();

            return new AuthResult
            {
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token,
                Success = true
            };
        }
    }
}
