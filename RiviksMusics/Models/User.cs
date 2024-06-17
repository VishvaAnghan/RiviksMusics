using System.ComponentModel.DataAnnotations;

namespace RiviksMusics.Models
{
    public class User
    {
        public string? Id { get; set; }
        public string? Sku { get; set; }
        [Required]
        [Display(Name = "First Name")]
        [RegularExpression(@"^[a-zA-Z]+$",ErrorMessage = "First Name can only contain letters.")]
        public string? FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Last Name can only contain letters.")]
        public string? LastName { get; set; }
        [Required]
        [EmailAddress]
        [RegularExpression(".+\\@.+\\..+",ErrorMessage ="Please Enter Valid Email Address")]
        public string? Email { get; set; }
       
        [Display(Name = "Phone No")]
        [MaxLength(10)]
        [RegularExpression(@"^[0-9]{10}$",ErrorMessage ="Phone No must be 10 digits")]
        public string? PhoneNo { get; set; }
        public string? Address { get; set; }
        [Required]
        [Display(Name = "Birth Date")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        [Required]
        public string? Gender { get; set; }
        public string? Image { get; set; }
        [Required]
        public string? Role { get; set; }
        public string? Action { get; set; }
    }
}
