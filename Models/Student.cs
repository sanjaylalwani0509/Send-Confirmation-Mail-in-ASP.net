using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MSU_BARODA.Models
{
    public class Student : IdentityUser
    {
        public required string PRN { get; set; }  // Generated PRN

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public override string Email { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        public required string FullName { get; set; }

        [Required(ErrorMessage = "Contact number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Contact number must be exactly 10 digits.")]
        public required string ContactNo { get; set; }

        [Required(ErrorMessage = "Faculty is required.")]
        public required string Faculty { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        public required string Department { get; set; }

        [Required(ErrorMessage = "Course is required.")]
        public required string Course { get; set; }

        // Ensure PasswordHash is not null
        public override string PasswordHash { get; set; } = string.Empty;
    }
}
