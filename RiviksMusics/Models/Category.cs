using System.ComponentModel.DataAnnotations;

namespace RiviksMusics.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "catagory Name can only contain letters.")]
        [Display(Name = "Category Name")]
        public string? CategoryName { get; set; }
        public string? Description { get; set; }
        //public int Action { get; set; }
    }
}