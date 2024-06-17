using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RiviksMusics.Models
{
    public class Album
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption. Identity)]
        public  int AlbumId { get; set; }
        public string? Sku { get; set; }
        [Required]
        [Display(Name = "Album Name")]
        public string? AlbumName { get; set; }
        [Required]
        [Display(Name = "Select Artist")]
        public string? ArtistId { get; set; }
        [ForeignKey(nameof(ArtistId))]
        public virtual ApplicationUser? User { get; set; }
        [Required]
        [Display(Name = "Select Category")]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Upload Date")]
        public DateTime UploadDate { get; set; }
        [Display(Name = "Album Image")]
        public string? AlbumImage { get; set; }
        public string? Status { get; set; }

    }
}
