using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RiviksMusics.Data;
using RiviksMusics.Models;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks.Dataflow;


namespace RiviksMusics.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, RoleManager<IdentityRole> roleManager, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _roleManager = roleManager;
            _webHostEnvironment = webHostEnvironment;

        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Albums()
        {
            var DisplayAlbum = await _context.Album.Select(D => new Album
            {
                AlbumId = D.AlbumId,
                AlbumName = D.AlbumName,
                ArtistId = D.ArtistId,
                Category = D.Category,
                UploadDate = D.UploadDate,
                AlbumImage = D.AlbumImage,
            }).OrderBy(D => D.AlbumName).ToListAsync();
            return View(DisplayAlbum);

        }

        public async Task<IActionResult> Person(string id)
        {
            /*var DisplayMusic = await _context.Music.Where(M => M.SelectType == "Person").Select(M => new Music
            {
                MusicId = M.MusicId,
                SongName = M.SongName,
                SelectType = M.SelectType,
                Category = M.Category,
                User = M.User,
                UploadDate= M.UploadDate,
                UploadImage= M.UploadImage,
                UploadSong  = M.UploadSong,
                ArtistId = M.ArtistId
            }).OrderBy(M => M.SelectType).ToListAsync();*/

            var DisplayMusic = await _context.Music
                .Where(M => M.SelectType == "Person").GroupBy(M => M.ArtistId)
               .Select(m => new PersonGroup
               {
                   ArtistId = m.Key,
                   SongCount = m.Count(),
                   artistName = (m.FirstOrDefault().User.FirstName + " "+ m.FirstOrDefault().User.LastName),
                   SongName = (m.FirstOrDefault().SongName),
                   UploadImage = (m.FirstOrDefault().UploadImage)
               }).ToListAsync();

            return View(DisplayMusic);
        }
        public IActionResult Events()
        {
            return View();
        }
        public IActionResult News()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
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

        public async Task<IActionResult> AlbumDetails(int id)
        {
            var query = from album in _context.Album
                            //join music in _context.Music on album.AlbumId equals music.AlbumId into musicAlbumGroup
                            //from music in musicAlbumGroup.DefaultIfEmpty()
                        where album.AlbumId == id
                        select new MusicAlbumViewModel
                        {
                            AlbumId = album.AlbumId,
                            AlbumName = album.AlbumName,
                            //Description = music.Description,
                            AlbumImage = album.AlbumImage,
                            // SongName = music.SongName,
                            // SelectType = music.SelectType,
                            //Category = (music.Category != null) ? music.Category.CategoryName : "",
                            Category = album.Category,
                            User = album.User,
                            // UploadDate = music.UploadDate,
                            //Songs = new List<Music> {music}
                            Songs = _context.Music.Include(x => x.User).Where(x => x.AlbumId == album.AlbumId).ToList()
                            //   UploadSong = music.UploadSong
                        };


            var result = await query.FirstOrDefaultAsync();
            return View(result);
        }

        public async Task<IActionResult> DownloadFile(int id)
        {
            var song = await _context.Music.FirstOrDefaultAsync(m => m.MusicId == id);
            if (song != null)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "audio");
                var filePath = Path.Combine(uploadsFolder, song.UploadSong);

                FileInfo fi = new FileInfo(song.UploadSong);
                var showMusicName = song.SongName.Replace(' ', '-').Trim() + fi.Extension;
                if (System.IO.File.Exists(filePath))
                {
                    return File(System.IO.File.OpenRead(filePath), "application/octet-stream", showMusicName);
                }
            }

            return NotFound();
        }

        
        public async Task<IActionResult> SingerDetails(string id)
        {
            var query = from music in _context.Music
                        where music.SelectType == "Person" && music.ArtistId == id

                        //join music in _context.Music on album.AlbumId equals music.AlbumId into musicAlbumGroup
                        //from music in musicAlbumGroup.DefaultIfEmpty()
                        select new MusicAlbumViewModel
                        {
                            // AlbumId = album.AlbumId,
                            //AlbumName = album.AlbumName,
                            //Description = music.Description,
                            AlbumImage = music.UploadImage,
                            SongName = music.SongName,
                            // SelectType = music.SelectType,
                            //Category = (music.Category != null) ? music.Category.CategoryName : "",
                            Category = music.Category,
                            User = music.User,
                            // UploadDate = music.UploadDate,
                            //Songs = new List<Music> {music}
                            MusicId =music.MusicId,
                            Songs = _context.Music.Include(x => x.User).Where(x => x.ArtistId == music.ArtistId).ToList(),
                            UploadSong = music.UploadSong
                        };

            var result = await query.ToListAsync();
            return View(result);
        }
        /* public IActionResult Elements()
         {

             return View();
         }*/



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

        #region Role

        [Authorize(Roles = "Admin")]
        public IActionResult Role(List<Roles> roles)
        {
            ViewBag.isrole = "active";
            var model = new List<Roles>();
            model = _context.Roles.Select(x => new Roles
            {
                Id = x.Id,
                Name = x.Name,
            })
                 .ToList();
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> InsertRole(Roles roleName)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(roleName.Id))
                {
                    //Edit
                    var role = _context.Roles.Find(roleName.Id);
                    role.Name = roleName.Name;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    //Create

                    var identityRole = new IdentityRole
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = roleName.Name
                    };

                    var result = await _roleManager.CreateAsync(identityRole);
                    //_context.Roles.Add(identityRole);
                    //_context.SaveChanges();
                }
                return RedirectToAction("Role");
            }
            return View(roleName);
            //return RedirectToAction("Role");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult EditRole(Roles roleName)
        {
            if (ModelState.IsValid)
            {
                var identityRole = _context.Roles.Find(roleName.Id);
                if (identityRole != null)
                {
                    identityRole.Name = roleName.Name;
                    _context.Roles.Update(identityRole);
                    _context.SaveChanges();
                    return RedirectToAction("Role");
                }
                else
                {
                    return NotFound();
                }
            }
            return View(roleName);
            //return RedirectToAction("Role");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeleteRole(string id)
        {


            var role = _context.Roles.Find(id);

            if (role != null)
            {
                _context.Roles.Remove(role);
                _context.SaveChanges();
                return Json(new { status = true });
            }

            return Json(new { status = false });

        }

        public IActionResult GetRoleById(string id)
        {
            var role = _context.Roles.Find(id);
            return Json(role);
        }
        #endregion

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
