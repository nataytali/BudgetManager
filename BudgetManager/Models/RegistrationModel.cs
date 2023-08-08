using FluentValidation;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BudgetManager.Models
{
    public class RegistrationModel
    {
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("confirmPassword")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
    public class RegistrationValidator : AbstractValidator<RegistrationModel>
    {
        public RegistrationValidator()
        {
            RuleFor(x => x.FirstName).Length(0, 15);
            RuleFor(x => x.LastName).Length(0, 15);
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.Password).MinimumLength(6);
        }
    }
}
