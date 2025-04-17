using System.ComponentModel.DataAnnotations;

namespace UserAuthService.Models
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
