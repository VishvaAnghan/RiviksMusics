using System.ComponentModel.DataAnnotations.Schema;

namespace RiviksMusics.Models
{
    public class MusicAlbumViewModel
    {
        public int AlbumId { get; set; }
       
        public string? ArtistId { get; set; }

        public virtual Music? Music { get; set; }
        public string? SongName { get; set; }
        public string? SelectType { get; set; }
        
        public virtual Category? Category { get; set; }
        public string? AlbumName { get; set; }

        public virtual ApplicationUser? User { get; set; }
        public string? Description { get; set; }
        //public DateTime UploadDate { get; set; }
        public string? AlbumImage { get; set; }

        public Music? Song { get; set; }
        public List<Music>? Songs { get; set; }
        public string? UploadSong { get; set; }
      
        public int? SongCount { get; set; }
        public int? MusicId { get; set; }
        public int? DownloadSong { get; set; }
        public int? ViewSong {  get; set; }

       
    }
}
