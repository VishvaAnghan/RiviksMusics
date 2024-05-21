using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RiviksMusics.Data;
using RiviksMusics.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Cryptography.X509Certificates;

namespace RiviksMusics.Controllers
{
    public class AlbumController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AlbumController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index(List<Album> albums)
        {
            ViewBag.isalbum = "active";
            var model = new List<Album>();
            model = _context.Album.Select(m => new Album
            {
                AlbumId = m.AlbumId,
                AlbumName = m.AlbumName,
                User = m.User,
                Category = m.Category,
                UploadDate = m.UploadDate,
                AlbumImage = m.AlbumImage
            }).ToList();
            return View("Album", model);
        }

        public IActionResult Create(Album albums)
        {
            ViewBag.isalbum = "active";
            return View("AddAlbum", albums);
        }

        [HttpPost]
        public IActionResult AddAlbum(Album album, IFormFile? ImageFile)
        {

            if (ModelState.IsValid)
            {
                var Album = new Album
                {
                    AlbumName = album.AlbumName,
                    ArtistId = album.ArtistId,
                    CategoryId = album.CategoryId,
                    UploadDate = album.UploadDate,
                    AlbumImage = album.AlbumImage
                };

                if (album.AlbumId != 0)
                {
                    //Edit
                    var albums = _context.Album.Find(album.AlbumId);
                    if (Album != null)
                    {

                        album.AlbumName = album.AlbumName;
                        album.ArtistId = album.ArtistId;
                        album.CategoryId = album.CategoryId;
                        album.UploadDate = album.UploadDate;
                        album.AlbumImage = album.AlbumImage;
                        _context.SaveChanges();

                    }
                }
                else
                {
                    //Create

                    var img = UploadImage(ImageFile);
                    if (!string.IsNullOrEmpty(img))
                    {
                        Album.AlbumImage = img;
                    }

                    _context.Album.Add(Album);
                    _context.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            return View("AddAlbum", album);
        }

        public IActionResult Edit(int Id)
        {
            ViewBag.isalbum = "active";
            var editAlbum = _context.Album.Where(x => x.AlbumId == Id)
                .Select(x => new Album
                {
                    AlbumId = x.AlbumId,
                    AlbumName = x.AlbumName,
                    ArtistId = x.ArtistId,
                    CategoryId = x.CategoryId,
                    UploadDate = x.UploadDate,
                    AlbumImage = x.AlbumImage,
                }).FirstOrDefault();
            return View("EditAlbum", editAlbum);
        }

        [HttpPost]
        public IActionResult EditAlbum(Album album, IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                var Album = _context.Album.Find(album.AlbumId);
                if (Album != null)
                {
                    var img = UploadImage(ImageFile);

                    Album.AlbumId = album.AlbumId;
                    Album.AlbumName = album.AlbumName;
                    Album.ArtistId = album.ArtistId;
                    Album.CategoryId = album.CategoryId;
                    Album.UploadDate = album.UploadDate;
                    if (!string.IsNullOrEmpty(img))
                    {
                        Album.AlbumImage = img;
                    }

                    _context.Album.Update(Album);
                    _context.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    return NotFound();
                }
            }
            return View("EditAlbum", album);
        }
        public IActionResult Delete(Album album)
        {
            var deleteAlbum = _context.Album.Where(x => x.AlbumId == album.AlbumId)
                .Select(x => new Album
                {
                    AlbumId = x.AlbumId,
                    AlbumName = x.AlbumName,
                    ArtistId = x.ArtistId,
                    CategoryId = x.CategoryId,
                    UploadDate = x.UploadDate,
                    AlbumImage = x.AlbumImage
                }).FirstOrDefault();
            return View("DeleteAlbum", deleteAlbum);
        }

        public IActionResult DeleteAlbum(int AlbumId)
        {
            var album = _context.Album.Find(AlbumId);
            if (album != null)
            {
                _context.Album.Remove(album);
                _context.SaveChanges();

                return Json(new { Status = true });
            }
            return Json(new { Status = false });
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
        public IActionResult CategoryRole()
        {
            var categories = _context.Category.Select(c => new
            {
                c.CategoryId,
                CategoryName = $"{c.CategoryName}"
            }).ToList();
            return Json(categories);

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
    }
}



