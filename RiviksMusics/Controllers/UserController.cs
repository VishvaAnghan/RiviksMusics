using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RiviksMusics.Data;
using RiviksMusics.Models;

namespace RiviksMusics.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public UserController(ILogger<HomeController> logger, ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index(List<User> User)
        {
            ViewBag.isuser = "active";
            var model = new List<User>();
            model = _context.Users.Select(u => new User
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                PhoneNo = u.PhoneNo,
                Address = u.Address,
                BirthDate = u.BirthDate,
                Gender = u.Gender,
                Role = u.Role

            }).ToList();
            return View("User", model);
        }

        public IActionResult isusereditprofile()
        {
            ViewBag.isusereditprofile = "active";
            return View();
        }


        //public IActionResult Userdetails()
        //{
        //    ViewBag.isuser = "active";
        //    return View();
        //}

        public IActionResult Create(User user)
        {
            return View("AddUser", user);
        }
        public async Task<IActionResult> AddUser(User user)
        {
            if (ModelState.IsValid)
            {
                var applicationUser = new ApplicationUser
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNo = user.PhoneNo,
                    Address = user.Address,
                    BirthDate = user.BirthDate,
                    Gender = user.Gender,
                    Role = user.Role,
                    EmailConfirmed = true,
                    UserName = user.Email //Guid.NewGuid().ToString().Replace('-', 'a'),
                };

                if (!string.IsNullOrEmpty(user.Id))
                {
                    //Edit
                    var User = await _context.Users.FindAsync(user.Id);
                    if (User != null)
                    {
                        User.FirstName = user.FirstName;
                        User.LastName = user.LastName;
                        User.Email = user.Email;
                        User.PhoneNo = user.PhoneNo;
                        User.Address = user.Address;
                        User.BirthDate = user.BirthDate;
                        User.Gender = user.Gender;
                        User.Role = user.Role;
                        await _context.SaveChangesAsync();

                    }
                }
                else
                {
                    //Create
                    var isEmailExist = await _context.Users.Where(x => x.Id != user.Id && x.Email == user.Email).FirstOrDefaultAsync();
                    if (isEmailExist != null)
                    {
                        ModelState.AddModelError("Email", "Email already exist");
                    }
                    else
                    {
                        var userResult = await _userManager.CreateAsync(applicationUser, "Test@123");
                        if (userResult.Succeeded)
                        {
                            var roleResult = await _userManager.AddToRoleAsync(applicationUser, user.Role);
                            if (roleResult.Succeeded)
                            {
                                return RedirectToAction("Index");
                            }
                        }
                    }
                }
            }
 
            return View("AddUser",user);

        }
        public async Task<IActionResult> Edit(User user)
        {
            var getUser = await _context.Users.Where(x => x.Id == user.Id)
                .Select(x => new User
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    PhoneNo = x.PhoneNo,
                    Address = x.Address,
                    BirthDate = x.BirthDate,
                    Gender = x.Gender,
                }).FirstOrDefaultAsync();

            if (getUser != null)
            {
                var appplicationUser = await _userManager.FindByIdAsync(getUser.Id);
                var userRoles = await _userManager.GetRolesAsync(appplicationUser);
                if (userRoles.Count() > 0)
                {
                    var role = await _context.Roles.FirstOrDefaultAsync(x => x.Name == userRoles[0]);
                    if (role != null)
                    {
                        getUser.Role = role.Name;
                    }
                }
            }
            return View("EditUser", getUser);


        }
        public async Task<IActionResult> EditUser(User user)
        {
            var isEmailExist = await _context.Users.Where(x => x.Id != user.Id && x.Email == user.Email).FirstOrDefaultAsync();
            if (isEmailExist != null)
            {
                ModelState.AddModelError("Email", "Email already exist");
            }

            if (ModelState.IsValid)
            {
                var applicationUser = await _context.Users.FindAsync(user.Id);
                if (applicationUser != null)
                {
                    applicationUser.FirstName = user.FirstName;
                    applicationUser.LastName = user.LastName;
                    applicationUser.Email = user.Email;
                    applicationUser.PhoneNo = user.PhoneNo;
                    applicationUser.Address = user.Address;
                    applicationUser.BirthDate = user.BirthDate;
                    applicationUser.Gender = user.Gender;
                    applicationUser.Role = user.Role;

                    _context.Users.Update(applicationUser);
                    await _context.SaveChangesAsync();


                    var userRoles = await _userManager.GetRolesAsync(applicationUser);
                    var removeRoleResult = await _userManager.RemoveFromRolesAsync(applicationUser, userRoles);
                    if (removeRoleResult != null)
                    {
                        var roleResult = await _userManager.AddToRoleAsync(applicationUser, user.Role);
                        if (roleResult.Succeeded)
                        {
                            return RedirectToAction("Index");
                        }
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    return NotFound();
                }
            }
            return View("EditUser", user);
            //return RedirectToAction("User");
        }


        public IActionResult Delete(User user)
        {
            var deleteUser = _context.Users.Where(x => x.Id == user.Id)
                .Select(x => new User
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    PhoneNo = x.PhoneNo,
                    Address = x.Address,
                    BirthDate = x.BirthDate,
                    Gender = x.Gender
                }).FirstOrDefault();
            return View("DeleteUser", deleteUser);


        }
        public IActionResult DeleteUser(string id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();

                return Json(new { Status = true });
            }
            return Json(new { Status = false });
        }
        public async Task<IActionResult> EditProfile()
        {
            var currentUserId = (await _userManager.GetUserAsync(HttpContext.User)).Id;
            var applicationUser = _context.Users.Find(currentUserId);

            return View("EditProfile", applicationUser);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ApplicationUser user , IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                var updateUser = _context.Users.Where(x => x.Id == user.Id).FirstOrDefault();
                if (updateUser != null) {
                    updateUser.FirstName = user.FirstName;
                    updateUser.LastName = user.LastName;    
                    updateUser.Email = user.Email;
                    updateUser.PhoneNo = user.PhoneNo;
                    updateUser.Address = user.Address;
                    updateUser.BirthDate = user.BirthDate;
                    updateUser.Gender = user.Gender;
                    

                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        // Generate a unique file name to avoid conflicts
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImageFile.FileName);
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img");
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // Save the file to the specified path
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(fileStream);
                        }

                        // Update user's image path
                        updateUser.Image = uniqueFileName;
                    }
                    _context.SaveChanges();
                }
            }

            return View("EditProfile", user);
        }

        public IActionResult RoleAction()
        {
            var roles = _context.Roles
                        .Select(r => new SelectListItem { Value = r.Id, Text = r.Name })
                        .ToList();

            return Json(roles);
        }

     
    }
}
