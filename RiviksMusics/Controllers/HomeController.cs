using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RiviksMusics.Data;
using RiviksMusics.Models;
using System.Diagnostics;


namespace RiviksMusics.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public HomeController(ILogger<HomeController> logger , ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _context = context;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            ViewBag.isdashbord = "active";
            return View();
        }

        [Authorize(Roles = "Admin")]
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

        public async Task<IActionResult> InsertRole(Roles roleName)
        {
           if(ModelState.IsValid)
            {
                if(!string.IsNullOrEmpty(roleName.Id))
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
