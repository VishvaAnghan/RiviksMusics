﻿using System.ComponentModel.DataAnnotations;

namespace RiviksMusics.Models
{
    public class User
    {
        public string? Id { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [Display(Name = "Phone No")]
        [MaxLength(10)]
        public string? PhoneNo { get; set; }
        public string? Address { get; set; }
        [Required]
        [Display(Name = "Birth Date")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        [Required]
        public string? Gender { get; set; }
        public string? Image { get; set; }
        public string? Action { get; set; }
    }
}
