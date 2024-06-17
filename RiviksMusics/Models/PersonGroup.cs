using System.ComponentModel.DataAnnotations;

namespace RiviksMusics.Models
{
    public class PersonGroup
    {
        public string? ArtistId { get; set; }
        public string? Sku { get; set; }
        public int SongCount { get; set; }
        public string? SongName { get; set; }
        public string? UploadImage { get; set; }
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Artist Name can only contain letters.")]
        public string? artistName { get; set; }
       
    }
}
