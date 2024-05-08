using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RiviksMusics.Data;
using RiviksMusics.Models;

namespace RiviksMusics.Controllers
{
    public class AlbumController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AlbumController(ILogger<HomeController> logger, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        //public IActionResult Index()
        //{
        //    return View();
        //}
        public IActionResult Index(List<Album> albums)
        {
            ViewBag.isalbum = "active";
            var model = new List<Album>();
            model = _context.Album.Select(m => new Album
            {
                AlbumId = m.AlbumId,
                AlbumName = m.AlbumName,
                User = m.User,
                //CategoryId = m.CategoryId,
                Category = m.Category,
                UploadDate = m.UploadDate,
                AlbumImage = m.AlbumImage
                //AlbumUpload = m.AlbumUpload
            }).ToList();
            return View("Album",model);
        }

        public IActionResult Create(Album albums)
        {
            ViewBag.isalbum = "active";
            var artists = _context.Users.Where(u => u.Role == "artist")
                   .Select(u => new
                   {
                       Id= u.Id,
                       FullName = $"{u.FirstName} {u.LastName}"
                   })
                   .ToList();
            ViewBag.Artists = new SelectList(artists, "Id", "FullName");
            var categories = _context.Category.Select(c => new
            {
                c.CategoryId,
                CategoryName = $"{c.CategoryName}"
            }).ToList();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");
           // ViewBag.Id = new SelectList(_context.User, "Id", "UserName");
           // ViewBag.Category = new SelectList(_context.Category, "CategoryId", "CategoryName");
            return View("AddAlbum", albums);
        }
       
        public IActionResult AddAlbum(Album album)
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
                    //AlbumUpload = album.AlbumUpload
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
                        //album.AlbumUpload = album.AlbumUpload;
                        _context.SaveChanges();

                    }
                }
                else
                {
                    //Create
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
            var artists = _context.Users.Where(u => u.Role == "artist")
                   .Select(u => new
                   {
                       Id = u.Id,
                       FullName = $"{u.FirstName} {u.LastName}"
                   })
                   .ToList();
            ViewBag.Artists = new SelectList(artists, "Id", "FullName");
            //var categories = _context.Category.Select(c => new
            //{
            //    c.CategoryId,
            //    CategoryName = $"{c.CategoryName}"
            //}).ToList();
            //ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");
            ViewBag.Categories = new SelectList(_context.Category, "CategoryId", "CategoryName");
            var editAlbum = _context.Album.Where(x => x.AlbumId == Id)
                .Select(x => new Album
                {
                    AlbumId = x.AlbumId,
                    AlbumName = x.AlbumName,
                    ArtistId = x.ArtistId,
                    CategoryId = x.CategoryId,
                    UploadDate = x.UploadDate,
                    AlbumImage = x.AlbumImage,
                    //AlbumUpload = x.AlbumUpload

                }).FirstOrDefault();
            return View("EditAlbum", editAlbum);
        }
        public IActionResult EditAlbum(Album album)
        {

            if (ModelState.IsValid)
            {
                var Album = _context.Album.Find(album.AlbumId);
                if (Album != null)
                {
                    Album.AlbumId = album.AlbumId;
                    Album.AlbumName = album.AlbumName;
                    Album.ArtistId = album.ArtistId;
                    Album.CategoryId = album.CategoryId;
                    Album.UploadDate = album.UploadDate;
                    Album.AlbumImage = album.AlbumImage;
                    //Album.AlbumUpload = album.AlbumUpload;
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
                    //AlbumUpload = x.AlbumUpload
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
    }
}



