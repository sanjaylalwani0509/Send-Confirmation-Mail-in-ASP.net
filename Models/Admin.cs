using System.ComponentModel.DataAnnotations;

namespace MSU_BARODA.Models
{
    public class Admin
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Admin ID is required.")]
        public string AdminID { get; set; } = string.Empty; // Auto-generated Admin ID

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name can't be longer than 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Contact number must be exactly 10 digits.")]
        public string ContactNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string MailId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Secret code is required.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Secret code must be between 6 and 20 characters.")]
        public string SecretCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
