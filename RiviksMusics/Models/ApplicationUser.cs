using Microsoft.AspNetCore.Identity;

namespace RiviksMusics.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNo { get; set; }
        public string? Address { get; set; }
        public DateTime BirthDate { get; set; }
        public string? Gender { get; set; }
        public string? Image { get; set; }

    }
}
