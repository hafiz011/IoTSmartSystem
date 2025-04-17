using System.ComponentModel.DataAnnotations;
using AspNetCore.Identity.MongoDbCore.Models;

namespace UserAuthService.Models
{
    public class ApplicationRole : MongoIdentityRole<Guid>
    {
        [Required]
        public string Description { get; set; } // Add custom properties like description if needed
    }
}
