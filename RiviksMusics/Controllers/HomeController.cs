using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RiviksMusics.Data;
using RiviksMusics.Models;
using System.Diagnostics;


namespace RiviksMusics.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, RoleManager<IdentityRole> roleManager, IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _roleManager = roleManager;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var latestAlbums = _context.Album.OrderByDescending(m => m.UploadDate).Take(12).ToList();
            var latestSong = _context.Music.OrderByDescending(s => s.UploadDate).FirstOrDefault();

            var latestSongs = _context.Music.OrderByDescending(s => s.UploadDate).Take(6).ToList();

            var albumsong = _context.Album.Select(D => new IndexViewModel
            {
                Sku = D.Sku,
                AlbumName = D.AlbumName,
                AlbumImage = D.AlbumImage,
                Category = D.Category,
                UploadDate = D.UploadDate
            }).OrderByDescending(D => D.UploadDate).Take(6).ToList();

            var DisplayMusic = _context.Music
                .Where(M => M.SelectType == "Person").GroupBy(M => M.ArtistId)
               .Select(m => new IndexViewModel
               {
                   ArtistId = m.Key,
                   Sku = (m.FirstOrDefault().User.Sku),
                   artistName = (m.FirstOrDefault().User.FirstName + " " + m.FirstOrDefault().User.LastName),
                   SongName = (m.FirstOrDefault().SongName),
                   UploadImage = (m.FirstOrDefault().UploadImage),
                   AudioSize = (m.FirstOrDefault().AudioSize)
               }).Take(6).ToList();

            var viewModel = new IndexViewModel
            {
                LatestAlbums = latestAlbums,
                LatestSong = latestSong,
                LatestSongs = latestSongs,
                AlbumSong = albumsong,
                Displaymusic = DisplayMusic

            };

            return View(viewModel);
        }

        public async Task<IActionResult> Albums()
        {
            var DisplayAlbum = await _context.Album.Select(D => new Album
            {
                AlbumId = D.AlbumId,
                Sku = D.Sku,
                AlbumName = D.AlbumName,
                ArtistId = D.ArtistId,
                Category = D.Category,
                UploadDate = D.UploadDate,
                AlbumImage = D.AlbumImage,

            }).OrderBy(D => D.AlbumName).ToListAsync();
            return View(DisplayAlbum);

        }

        public async Task<IActionResult> Person()
        {

            var DisplayMusic = await _context.Music
                .Where(M => M.SelectType == "Person").GroupBy(M => M.ArtistId)
               .Select(m => new PersonGroup
               {
                   ArtistId = m.Key,
                   Sku = (m.FirstOrDefault().User.Sku),
                   SongCount = m.Count(),
                   artistName = (m.FirstOrDefault().User.FirstName + " " + m.FirstOrDefault().User.LastName),
                   SongName = (m.FirstOrDefault().SongName),
                   UploadImage = (m.FirstOrDefault().UploadImage)
               }).ToListAsync();

            return View(DisplayMusic);
        }
        public IActionResult Plans()
        {
            var plans = new List<PayNow>
            {
                new PayNow { PlanId = 1, PlanName = "Basic Plan", Rupees = 50, Days = 30, Duration = "1 Month", Style = "background-image:linear-gradient(45deg,#047df4,#01188b);"},
                new PayNow { PlanId = 2, PlanName = "Standard Plan", Rupees = 150 , Days = 60 , Duration = "3 Month" , Style = "background-image:linear-gradient(45deg,#00ff62,#233543);"},
                new PayNow { PlanId = 3, PlanName = "Premium Plan", Rupees = 250 ,Days = 90 , Duration = "6 Month" ,Style = "background-image:linear-gradient(45deg,#ff2ff7,#4700ff);"}
            };
            return View(plans);
        }
        public IActionResult About()
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



        public async Task<IActionResult> AlbumDetails(string Sku)
        {
            if (string.IsNullOrEmpty(Sku))
            {
                return NotFound();
            }

            var query = from album in _context.Album
                        where album.Sku == Sku
                        select new MusicAlbumViewModel
                        {
                            AlbumId = album.AlbumId,
                            AlbumName = album.AlbumName,
                            AlbumImage = album.AlbumImage,
                            Category = album.Category,
                            User = album.User,
                            Status = !string.IsNullOrEmpty(album.Status) && album.Status == "true" ? true : false,
                            Songs = _context.Music.Include(x => x.User).Where(x => x.AlbumId == album.AlbumId).ToList()
                        };

            var result = await query.FirstOrDefaultAsync();
            if (result == null)
            {
                return NotFound();
            }

            foreach (var item in result.Songs)
            {
                var Music = await _context.Music.FindAsync(item.MusicId);
                if (Music != null)
                {
                    Music.ViewSong = (Music.ViewSong ?? 0) + 1;
                    _context.Music.Update(Music);
                    await _context.SaveChangesAsync();
                }
            }


            ViewBag.HasSubscription = false;
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);

                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Admin"))
                {
                    ViewBag.HasSubscription = true;
                }
                else
                {

                    var payment = await _context.Payment
                            .Where(payment => payment.UserId == user.Id && payment.ExpiredPlanDate > DateTime.Now)
                            .FirstOrDefaultAsync();

                    if (payment != null)
                    {
                        ViewBag.HasSubscription = true;
                    }
                }
            }
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

                    song.DownloadSong = (song.DownloadSong ?? 0) + 1;
                    _context.SaveChanges();
                    return File(System.IO.File.OpenRead(filePath), "application/octet-stream", showMusicName);

                }
            }

            return NotFound();

        }
        public async Task<IActionResult> SingerDetails(string id)
        {
            var query = from music in _context.Music
                        where music.SelectType == "Person" && music.User.Sku == id
                        select new MusicAlbumViewModel
                        {
                            AlbumImage = music.UploadImage,
                            SongName = music.SongName,
                            Category = music.Category,
                            User = music.User,
                            MusicId = music.MusicId,
                            Songs = _context.Music.Include(x => x.User).Where(x => x.ArtistId == music.ArtistId).ToList(),
                            UploadSong = music.UploadSong,
                            ViewSong = music.ViewSong,
                            DownloadSong = music.DownloadSong,
                            AudioSize = music.AudioSize,
                            Status = !string.IsNullOrEmpty(music.Status) && music.Status == "true" ? true : false
                        };

            var result = await query.ToListAsync();
            foreach (var item in result)
            {
                var Music = await _context.Music.FindAsync(item.MusicId);
                if (Music != null)
                {
                    Music.ViewSong = ((Music.ViewSong ?? 0) + 1);
                    _context.Music.Update(Music);
                    await _context.SaveChangesAsync();
                }
            }
            ViewBag.HasSubscription = false;
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);

                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Admin"))
                {
                    ViewBag.HasSubscription = true;
                }
                else
                {
                    var payment = await _context.Payment
                        .Where(payment => payment.UserId == user.Id && payment.ExpiredPlanDate > DateTime.Now)
                        .FirstOrDefaultAsync();

                    if (payment != null)
                    {
                        ViewBag.HasSubscription = true;
                    }
                }

            }
            return View(result);
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

                }
                return RedirectToAction("Role");
            }
            return View(roleName);

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
