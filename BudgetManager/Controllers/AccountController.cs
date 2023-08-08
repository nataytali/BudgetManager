using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using BudgetManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.AspNetCore;
using System.Net.Http;
using System.Text;
using System.Net;

namespace BudgetManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;
        private IValidator<RegistrationModel> _validator;
        public AccountController(ApplicationContext context, IMapper mapper, UserManager<IdentityUser> userManager, IValidator<RegistrationModel> validator)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _validator = validator;
        }
        [HttpGet, Authorize]
        public async Task<List<UserModel>> Get()
        {
            var users = await _context.Users.ToListAsync();
            return users;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegistrationModel model)
        {
            var sb = new StringBuilder();
            // Perform validation on the model
            ValidationResult result = await _validator.ValidateAsync(model);

            if (!result.IsValid)
            {
                result.AddToModelState(this.ModelState);
                return BadRequest(new
                {
                    message = "Invalid model state",
                    status = (int)HttpStatusCode.BadRequest,
                    modelState = ModelState
                });
            }
            //check if user exists
            bool userExists = await _context.Users.AnyAsync(u => u.Email == model.Email);

            if (userExists)
            {
                result.AddToModelState(this.ModelState);
                return BadRequest(new 
                { 
                    message = "User with such email already exists",
                    status = (int)HttpStatusCode.BadRequest,
                    modelState = ModelState
                });
            }
            else
            {
                // Create new User
                var user = _mapper.Map<UserModel>(model);
                var hasher = new PasswordHasher<IdentityUser>();
                var hash = hasher.HashPassword(user, model.Password);
                user.PasswordHash = hash;
                
                await _context.Users.AddAsync(user);
                _context.SaveChanges();

                return Ok(model);
            }
            
        }
    }
}
