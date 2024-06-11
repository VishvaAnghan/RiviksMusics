



namespace RiviksMusics.Models
{
    public class IndexViewModel
    {
        public List<Album>? LatestAlbums { get; internal set; }
        public Music? LatestSong { get; internal set; }
        public List<Music>? LatestSongs { get; internal set; }
        public List<Album>? PopulerArtist { get; internal set; }
       
        public List<IndexViewModel>? AlbumSong { get; internal set; }
        public string? artistName { get; internal set; }
        public string? SongName { get; internal set; }
        public string? UploadImage { get; internal set; }
        public List<IndexViewModel>? Displaymusic { get; internal set; }
        public string? Sku { get; internal set; }
        public string? AlbumName { get; internal set; }
        public string? AlbumImage { get; internal set; }
        public Category? Category { get; internal set; }
        public long? AudioSize { get;  set; }
        public string? USer { get; internal set; }
        public string? ArtistId { get; internal set; }
    }
}
