using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RiviksMusics.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Sku { get; set; }
        [Display(Name = "First Name")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "First Name can only contain letters.")]
        public string? FirstName { get; set; }
        [Display(Name = "Last Name")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Last Name can only contain letters.")]
        public string? LastName { get; set; }
        [MaxLength(10)]
        [Display(Name = "Phone No")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone No must be 10 digits")]
        public string? PhoneNo { get; set; }
        public string? Address { get; set; }
        [Display(Name = "Birth Date")]
        public DateTime BirthDate { get; set; }
        public string? Gender { get; set; }
        public string? Role { get; set; }
        public string? Image { get; set; }
       // public bool IsArtist { get;  set; }
      //  public bool IsAdmin { get;  set; }
    }
}
