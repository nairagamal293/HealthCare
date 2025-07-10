using Microsoft.AspNetCore.Identity;

namespace HeathCare.Models
{
    using Microsoft.AspNetCore.Identity;
    using System.ComponentModel.DataAnnotations;

    namespace HeathCare.Models
    {
        public class ApplicationUser : IdentityUser
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            // Add other properties with default values if needed
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        }
    }
}
