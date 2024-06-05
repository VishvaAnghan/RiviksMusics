using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RiviksMusics.Models
{
    public class Music
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MusicId { get; set; }
        [MaxLength(150)]
        [Required]
        [Display(Name = "Song Name")]
        public string? SongName { get; set; }
        
        public string? SelectType { get; set; }
        
        [Display(Name = "Select Category")]
        [ForeignKey("Category")]
        public int? CategoryId { get; set; }
        public virtual Category? Category { get; set; }
        
        [ForeignKey("Album")]
        [Display(Name = "Select Album")]
        public int? AlbumId { get; set; }
        public virtual Album? Album { get; set; }
        
        [Display(Name = "Select Artist")]
        public string? ArtistId { get; set; }
        [ForeignKey(nameof(ArtistId))]
        public virtual ApplicationUser? User { get; set; }
        public string? Description { get; set; }
        [DataType(DataType.Date)]
        [Display(Name ="Upload Date")]
        public DateTime UploadDate { get; set; }
        [Display(Name = "Upload Image")]
        public string? UploadImage { get; set; }
        [Display(Name = "Upload Song")]
        public string? UploadSong { get; set; }
        public int? ViewSong { get; set; }
        public int? DownloadSong { get; set; }
        //public int? ViewCount { get;  set; }

        //public List<Music> Musics { get; set; }
       // public int ViewSongCount { get; set; }
    }
}
