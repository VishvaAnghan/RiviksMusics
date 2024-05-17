using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RiviksMusics.Data;
using RiviksMusics.Models;

namespace RiviksMusics.Controllers
{
    public class MusicController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MusicController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index(List<Music> musics)
        {
            ViewBag.ismusic = "active";
            var model = new List<Music>();
            model = _context.Music.Select(m => new Music
            {
                MusicId = m.MusicId,
                SongName = m.SongName,
                SelectType = m.SelectType,
                Category = m.Category,
                Album = m.Album,
                User = m.User,
                Description = m.Description,
                UploadDate = m.UploadDate,
                UploadImage = m.UploadImage,
                UploadSong = m.UploadSong
            }).ToList();
            return View("Music", model);
        }
        public IActionResult Create(Music musics)
        {
            ViewBag.ismusic = "active";
            return View("AddMusic", musics);
        }

        [HttpPost]
        public IActionResult AddMusic(Music music, IFormFile? ImageFile,IFormFile? AudioFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //Create
                    var Music = new Music
                    {
                        SongName = music.SongName,
                        SelectType = music.SelectType,
                        CategoryId = music.CategoryId,
                        AlbumId = music.AlbumId,
                        ArtistId = music.ArtistId,
                        Description = music.Description,
                        UploadDate = music.UploadDate,
                        //UploadImage = music.UploadImage,
                        //UploadSong = music.UploadSong
                    };

                    var img = UploadImage(ImageFile);
                    if (!string.IsNullOrEmpty(img))
                    {
                        Music.UploadImage = img;
                    }

                    var audio = UploadSong(AudioFile);
                    if (!string.IsNullOrEmpty(audio))
                    {
                        Music.UploadSong = audio;
                    }

                    _context.Music.Add(Music);
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw;
                }
                return RedirectToAction("Index");
            }

            return View("AddMusic", music);
        }
        public IActionResult Edit(int Id)
        {
            ViewBag.ismusic = "active";
            var editMusic = _context.Music.Where(x => x.MusicId == Id)
                .Select(x => new Music
                {
                    MusicId = x.MusicId,
                    SongName = x.SongName,
                    SelectType = x.SelectType,
                    CategoryId = x.CategoryId,
                    AlbumId = x.AlbumId,
                    ArtistId = x.ArtistId,
                    Description = x.Description,
                    UploadDate = x.UploadDate,
                    UploadImage = x.UploadImage,
                    UploadSong = x.UploadSong,
                }).FirstOrDefault();
            return View("EditMusic", editMusic);
        }

        [HttpPost]
        public IActionResult EditMusic(Music music, IFormFile? ImageFile, IFormFile? AudioFile)
        {

            if (ModelState.IsValid)
            {
                var Music = _context.Music.Find(music.MusicId);
                if (Music != null)
                {
                    var img = UploadImage(ImageFile);
                    var audio = UploadSong(AudioFile);

                    Music.MusicId = music.MusicId;
                    Music.SongName = music.SongName;
                    Music.SelectType = music.SelectType;
                    Music.CategoryId = music.CategoryId;
                    Music.AlbumId = music.AlbumId;
                    Music.ArtistId = music.ArtistId;
                    Music.Description = music.Description;
                    Music.UploadDate = music.UploadDate;
                    if (!string.IsNullOrEmpty(img))
                    {
                        Music.UploadImage = img;
                    }

                    //Music.UploadSong = music.UploadSong;
                    if (!string.IsNullOrEmpty(audio))
                    {
                        Music.UploadSong = audio;
                    }
                    _context.Music.Update(Music);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {

                    return NotFound();
                }
            }
            return View("EditMusic", music);

        }
        public IActionResult Delete(Music music)
        {
            var deleteMusic = _context.Music.Where(x => x.MusicId == music.MusicId)
                .Select(x => new Music
                {
                    MusicId = x.MusicId,
                    SongName = x.SongName,
                    SelectType = x.SelectType,
                    CategoryId = x.CategoryId,
                    AlbumId = x.AlbumId,
                    ArtistId = x.ArtistId,
                    UploadDate = x.UploadDate,
                    UploadImage = x.UploadImage,
                    UploadSong = x.UploadSong,
                }).FirstOrDefault();
            return View("DeleteMusic", deleteMusic);
        }

        public IActionResult DeleteMusic(int MusicId)
        {
            var music = _context.Music.Find(MusicId);
            if (music != null)
            {
                _context.Music.Remove(music);
                _context.SaveChanges();

                return Json(new { Status = true });
            }
            return Json(new { Status = false });
        }

        public IActionResult CategoryRole()
        {
            var categories = _context.Category.Select(c => new
            {
                c.CategoryId,
                CategoryName = $"{c.CategoryName}"
            }).ToList();
            return Json(categories);

        }

        public IActionResult AlbumRole()
        {
            var albums = _context.Album.Select(c => new
            {
                c.AlbumId,
                AlbumName = $"{c.AlbumName}"
            }).ToList();
            return Json(albums);

        }
        public IActionResult ArtistRole()
        {
            var artists = _context.Users.Where(u => u.Role == "artist")
                 .Select(u => new
                 {
                     Id = u.Id,
                     FullName = $"{u.FirstName} {u.LastName}"
                 })
                 .ToList();

            return Json(artists);
        }

        public string UploadImage(IFormFile? ImageFile)
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {
                // Generate a unique file name to avoid conflicts
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImageFile.FileName);
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img");
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save the file to the specified path
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(fileStream);
                }

                // Update user's image path
                return uniqueFileName;
            }

            return "";
        }


        public string UploadSong(IFormFile? AudioFile)
        {
            if (AudioFile != null && AudioFile.Length > 0)
            {
                // Generate a unique file name to avoid conflicts
                FileInfo fi = new FileInfo(AudioFile.FileName);
                var uniqueFileName = Guid.NewGuid().ToString() + fi.Extension;
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "audio");
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save the file to the specified path
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    AudioFile.CopyTo(fileStream);
                }

                // Update user's audio path
                return uniqueFileName;
            }

            return "";
        }
    }
}

