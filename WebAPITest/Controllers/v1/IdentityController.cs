using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAPITest.Contracts.Request;
using WebAPITest.Contracts.Response;
using WebAPITest.Contracts.v1;
using WebAPITest.Domain;
using WebAPITest.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPITest.Controllers.v1
{
    public class IdentityController : Controller
    {

        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        // GET: /<controller>/
        [HttpPost(ApiRoutes.Identity.Register)]
        public async Task<IActionResult> Register([FromBody]RegisterRequest registerRequest)
        {

            AuthResult authResult = await _identityService.RegisterAsync(registerRequest.Email, registerRequest.Password);

            if (!authResult.Success)
            {
                return BadRequest(new AuthFailResponse
                {
                    Errors = authResult.Errors
                });
            }

            return Ok(new AuthSuccessResponse {
                Token = authResult.Token
            });
        }

        [HttpPost(ApiRoutes.Identity.Login)]
        public async Task<IActionResult> Login([FromBody]LoginRequest loginRequest)
        {
            AuthResult authResult = await _identityService.LoginAsync(loginRequest.Email, loginRequest.Password);

            if (!authResult.Success)
            {
                return BadRequest(new AuthFailResponse {
                    Errors = authResult.Errors
                });
            }

            return Ok(new AuthSuccessResponse {
                Token = authResult.Token
            });
        }
    }
}
