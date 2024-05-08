using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RiviksMusics.Models
{
    public class Album
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption. Identity)]
        public  int AlbumId { get; set; }

        public string? AlbumName { get; set; }

        public string? ArtistId { get; set; }
        [ForeignKey(nameof(ArtistId))]
        public virtual ApplicationUser? User { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; }
        [DataType(DataType.Date)]
        public DateTime UploadDate { get; set; }
        public string? AlbumImage { get; set; }
       
    }
}
