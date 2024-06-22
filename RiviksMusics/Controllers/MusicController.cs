using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RiviksMusics.Data;
using RiviksMusics.Models;
using System.Linq;
using System;

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


        public async Task<IActionResult> Index(List<Music> musics)
        {
            ViewBag.ismusic = "active";
            DateTime today = DateTime.Today;
            int viewsong = _context.Music.Count(i => i.UploadDate.Date == today);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Artist"))
            {

                musics = _context.Music.Where(m => m.ArtistId == user.Id)
                .Select(m => new Music
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
                    UploadSong = m.UploadSong,
                    ViewSong = m.ViewSong ?? 0,
                    DownloadSong = m.DownloadSong ?? 0,
                    AudioSize = m.AudioSize,
                    Status = m.Status
                }).ToList();
            }
            else if (roles.Contains("Admin"))
            {

                musics = _context.Music
                 .Select(m => new Music
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
                     UploadSong = m.UploadSong,
                     ViewSong = m.ViewSong,
                     DownloadSong = m.DownloadSong ?? 0,
                     AudioSize = m.AudioSize,
                     Status = m.Status
                 }).ToList();

            }
            else
            {
                musics = new List<Music>();
            }

          
            return View("Music", musics);
        }

        public IActionResult Create(Music musics)
        {
            ViewBag.ismusic = "active";
            return View("AddMusic", musics);
        }

        [HttpPost]
        public IActionResult AddMusic(Music music, IFormFile? ImageFile, IFormFile? AudioFile)
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
                        Status = music.Status

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
                        Music.AudioSize = AudioFile.Length;
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
                    ViewSong = x.ViewSong,
                    DownloadSong = x.DownloadSong,
                    Status = x.Status
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

                    if (music.SelectType == "Person")
                    {
                        Music.CategoryId = music.CategoryId;
                        Music.AlbumId = null;
                    }
                    else
                    {
                        Music.AlbumId = music.AlbumId;
                        Music.CategoryId = null;
                    }


                    Music.ArtistId = music.ArtistId;
                    Music.Description = music.Description;
                    Music.UploadDate = music.UploadDate;
                    if (!string.IsNullOrEmpty(img))
                    {
                        Music.UploadImage = img;
                    }

                    if (!string.IsNullOrEmpty(audio))
                    {
                        Music.UploadSong = audio;
                        Music.AudioSize = AudioFile.Length;
                    }
                    Music.ViewSong = music.ViewSong;
                    Music.DownloadSong = music.DownloadSong;
                    Music.Status = music.Status;
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


        public async Task<IActionResult> ArtistRole()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;
            var userRole = await _userManager.GetRolesAsync(user);
            List<object> artists;
            if (userRole.Contains("Artist"))
            {
                artists = _context.Users.Where(u => u.Id == userId)
               .Select(u => new
               {
                   Id = u.Id,
                   FullName = $"{u.FirstName} {u.LastName}"
               }).ToList<object>();
            }

            else if (userRole.Contains("Admin"))
            {
                artists = _context.Users
               .Select(u => new
               {
                   Id = u.Id,
                   FullName = $"{u.FirstName} {u.LastName}"
               }).ToList<object>();
            }
            else
            {
                artists = new List<object>();
            }
            return Json(artists);
        }



        public string UploadImage(IFormFile? ImageFile)
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImageFile.FileName);
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img");
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);


                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(fileStream);
                }

                return uniqueFileName;
            }

            return "";
        }

        public string UploadSong(IFormFile? AudioFile)
        {
            if (AudioFile != null && AudioFile.Length > 0)
            {
                FileInfo fi = new FileInfo(AudioFile.FileName);
                var uniqueFileName = Guid.NewGuid().ToString() + fi.Extension;
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "audio");
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    AudioFile.CopyTo(fileStream);
                }
                return uniqueFileName;
            }
            return "";

        }


        public ActionResult ViewSong(int id)
        {
            var song = _context.Music.Find(id);
            if (song != null)
            {
                song.ViewSong++;
                _context.SaveChanges();
            }

            return View("Index");
        }
        public ActionResult DownloadSong(int id)
        {
            var song = _context.Music.Find(id);
            if (song != null)
            {
                song.DownloadSong++;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        
    }

}


