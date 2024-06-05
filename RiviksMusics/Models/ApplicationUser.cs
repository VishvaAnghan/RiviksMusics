using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RiviksMusics.Models
{
    public class ApplicationUser : IdentityUser
    { 
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }
        [MaxLength(10)]
        [Display(Name = "Phone No")]
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
