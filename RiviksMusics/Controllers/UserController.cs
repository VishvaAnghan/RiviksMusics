using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RiviksMusics.Data;
using RiviksMusics.Models;
using System.Net;

namespace RiviksMusics.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;

        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult User(List<User> User)
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
                Gender = u.Gender

            }).ToList();
            return View(model);
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
        public IActionResult AddUser(User user)
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
                    Gender = user.Gender
                };

                if (!string.IsNullOrEmpty(user.Id))
                {
                    //Edit
                    var User = _context.Users.Find(user.Id);
                    if (User != null)
                    {
                        User.FirstName = user.FirstName;
                        User.LastName = user.LastName;
                        User.Email = user.Email;
                        User.PhoneNo = user.PhoneNo;
                        User.Address = user.Address;
                        User.BirthDate = user.BirthDate;
                        User.Gender = user.Gender;
                        _context.SaveChanges();

                    }
                }
                else
                {
                    //Create
                    _context.Users.Add(applicationUser);
                    _context.SaveChanges();
                }
                return RedirectToAction("User");
            }
            return View("AddUser", user);

        }
        public IActionResult Edit(User user)
        {
            var getUser = _context.Users.Where(x => x.Id == user.Id)
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
            return View("EditUser", getUser);


        }
        public IActionResult EditUser(User user)
        {
            if (ModelState.IsValid)
            {
                var applicationUser = _context.Users.Find(user.Id);
                if (applicationUser != null)
                {
                    applicationUser.FirstName = user.FirstName;
                    applicationUser.LastName = user.LastName;
                    applicationUser.Email = user.Email;
                    applicationUser.PhoneNo = user.PhoneNo;
                    applicationUser.Address = user.Address;
                    applicationUser.BirthDate = user.BirthDate;
                    applicationUser.Gender = user.Gender;
                    _context.Users.Update(applicationUser);
                    _context.SaveChanges();
                    return RedirectToAction("User");
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


        
        public IActionResult EditProfile(User editProfile)
        {
            if (ModelState.IsValid)
            {
                var applicationUser = _context.Users.Find(editProfile.Id);
                if (applicationUser != null)
                {
                    applicationUser.FirstName = editProfile.FirstName;
                    applicationUser.LastName = editProfile.LastName;
                    applicationUser.Email = editProfile.Email;
                    applicationUser.PhoneNo = editProfile.PhoneNo;
                    applicationUser.Address = editProfile.Address;
                    applicationUser.BirthDate = editProfile.BirthDate;
                    applicationUser.Gender = editProfile.Gender;
                    applicationUser.Image = editProfile.Image;

                    _context.Users.Update(applicationUser);
                    _context.SaveChanges();
                    return RedirectToAction("User");
                }
                else
                {
                    return NotFound();
                }
            }
            return View("EditProfile", editProfile);
            //return RedirectToAction("User");

        }
    }
}
