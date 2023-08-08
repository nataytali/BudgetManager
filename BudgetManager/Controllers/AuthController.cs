using BudgetManager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BudgetManager.Infrastructure;
using BudgetManager.Services;
using Microsoft.AspNetCore.Identity;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.AspNetCore;

namespace BudgetManager.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IUserService _userService;
        private readonly IJwtAuthManager _jwtAuthManager;
        private IValidator<LoginModel> _validator;
        public AuthController(ILogger<AuthController> logger, IUserService userService, IJwtAuthManager jwtAuthManager, UserManager<IdentityUser> userManager, IValidator<LoginModel> validator)
        {
            _logger = logger;
            _userService = userService;
            _jwtAuthManager = jwtAuthManager;
            _validator = validator;
        }

        [HttpPost, Route("login")]
        public IActionResult Login([FromBody] LoginModel request)
        {
            FluentValidation.Results.ValidationResult result = _validator.Validate(request);

            if (!result.IsValid)
            {
                result.AddToModelState(this.ModelState);
                return BadRequest(ModelState);
            }
            if (!_userService.IsValidUserCredentials(request.Email, request.Password))
            {
                return Unauthorized("Not valid credentials");
            }

            var role = _userService.GetUserRole(request.Email);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, request.Email),
                new Claim(ClaimTypes.Role, role)
            };

            var jwtResult = _jwtAuthManager.GenerateTokens(request.Email, claims, DateTime.Now);
            _logger.LogInformation($"User [{request.Email}] logged in the system.");
            return Ok(new LoginResult
            {
                UserName = request.Email,
                Role = role,
                AccessToken = jwtResult.AccessToken,
                RefreshToken = jwtResult.RefreshToken.TokenString
            });
        }
        [HttpGet("user")]
        [Authorize]
        public ActionResult GetCurrentUser()
        {
            return Ok(new LoginResult
            {
                UserName = User.Identity?.Name,
                Role = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty,
                OriginalUserName = User.FindFirst("OriginalUserName")?.Value
            });
        }

        [Authorize]
        [HttpPost, Route("logout")]
        public IActionResult Logout()
        {
            var userName = User.Identity?.Name;
            _jwtAuthManager.RemoveRefreshTokenByUserName(userName);
            _logger.LogInformation($"User [{userName}] logged out the system.");
            return Ok();

        }
        [HttpPost("refresh-token")]
        [Authorize]
        public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var userName = User.Identity?.Name;
                _logger.LogInformation($"User [{userName}] is trying to refresh JWT token.");

                if (string.IsNullOrWhiteSpace(request.RefreshToken))
                {
                    return Unauthorized();
                }

                var accessToken = await HttpContext.GetTokenAsync("Bearer", "access_token");
                var jwtResult = _jwtAuthManager.Refresh(request.RefreshToken, accessToken, DateTime.Now);
                _logger.LogInformation($"User [{userName}] has refreshed JWT token.");
                return Ok(new LoginResult
                {
                    UserName = userName,
                    Role = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty,
                    AccessToken = jwtResult.AccessToken,
                    RefreshToken = jwtResult.RefreshToken.TokenString
                });
            }
            catch (SecurityTokenException e)
            {
                return Unauthorized(e.Message); // return 401 so that the client side can redirect the user to login page
            }
        }

        public class LoginResult
        {
            [JsonPropertyName("username")]
            public string UserName { get; set; }

            [JsonPropertyName("role")]
            public string Role { get; set; }

            [JsonPropertyName("originalUserName")]
            public string OriginalUserName { get; set; }

            [JsonPropertyName("accessToken")]
            public string AccessToken { get; set; }

            [JsonPropertyName("refreshToken")]
            public string RefreshToken { get; set; }
        }

        public class RefreshTokenRequest
        {
            [JsonPropertyName("refreshToken")]
            public string RefreshToken { get; set; }
        }
    }
}
