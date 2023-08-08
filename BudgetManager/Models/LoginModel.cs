using FluentValidation;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BudgetManager.Models
{
    public class LoginModel
    {
        [Required]
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [Required]
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
    public class LoginValidator : AbstractValidator<LoginModel>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email).EmailAddress();
            //RuleFor(x => x.Password).MinimumLength(6);
        }
    }
}
