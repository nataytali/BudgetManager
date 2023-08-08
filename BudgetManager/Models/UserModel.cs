using Microsoft.AspNetCore.Identity;
using System;
using FluentValidation;

namespace BudgetManager.Models
{
    public class UserModel: IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

    }
    
}
