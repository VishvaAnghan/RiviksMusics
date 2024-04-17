using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RiviksMusics.Data;
using RiviksMusics.Models;
using System.Diagnostics;


namespace RiviksMusics.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        
        public HomeController(ILogger<HomeController> logger , ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.isdashbord = "active";
            return View();
        }
        public IActionResult Role(List<Roles>roles)
        {
            ViewBag.isrole = "active";
            var model = new List<Roles>();
           model = _context.Roles.Select(x => new Roles
           {
               Id = x.Id,
               Name = x.Name,
           })
                .ToList();
             return View (model);
        }

        public IActionResult InsertRole(Roles roleName)
        {
           if(ModelState.IsValid)
            {
                var identityRole = new IdentityRole
                {
                    Name = roleName.Name
                };

                if(!string.IsNullOrEmpty(roleName.Id))
                {
                    //Edit
                    var role = _context.Roles.Find(roleName.Id);
                    role.Name = roleName.Name;
                    _context.SaveChanges();
                }
                else
                {
                    //Create
                    _context.Roles.Add(identityRole);
                    _context.SaveChanges();
                }
                return RedirectToAction("Role");
            }
            return View(roleName);
            //return RedirectToAction("Role");
        }


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
