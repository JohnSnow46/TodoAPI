using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Application.DTOs.User
{
    namespace TodoAPI.Application.DTOs.User
    {
        public class RegisterDto
        {
            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email format")]
            [MaxLength(254, ErrorMessage = "Email cannot exceed 254 characters")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "First name is required")]
            [MaxLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
            public string FirstName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Last name is required")]
            [MaxLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
            public string LastName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password is required")]
            [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
            [MaxLength(100, ErrorMessage = "Password cannot exceed 100 characters")]
            public string Password { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password confirmation is required")]
            [Compare(nameof(Password), ErrorMessage = "Password and confirmation password do not match")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }
    }
}
