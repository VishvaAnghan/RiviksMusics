using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RiviksMusics.Data;
using RiviksMusics.Models;

namespace RiviksMusics.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CategoryController(ILogger<HomeController> logger, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()

        {
            return View();
        }

        public IActionResult Category(List<Category> categories)
        {
            ViewBag.iscategory = "active";
            var model = new List<Category>();
            model = _context.Category.Select(m => new Category
            {
                CategoryId = m.CategoryId,
                CategoryName = m.CategoryName,
                Description = m.Description
            }).ToList();
            return View(model);
        }



        public IActionResult Create(Category category)
        {
            return View("AddCategory", category);
        }

        public IActionResult AddCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                var Category = new Category
                {
                    //CategoryId = category.CategoryId,
                    CategoryName = category.CategoryName,
                    Description = category.Description
                };

                if (category.CategoryId != 0)
                {
                    //Edit
                    var categorys = _context.Category.Find(category.CategoryId);
                    if (Category != null)
                    {
                        category.CategoryId = category.CategoryId;
                        category.CategoryName = category.CategoryName;
                        category.Description = category.Description;
                        _context.SaveChanges();

                    }
                }
                else
                {
                    //Create
                    _context.Category.Add(Category);
                    _context.SaveChanges();
                }
                return RedirectToAction("Category");
            }

            return View("AddCategory", category);
        }

        public IActionResult Edit(int Id)
        {
            var getCategory = _context.Category.Where(x => x.CategoryId == Id)
                .Select(x => new Category
                {
                    CategoryId = x.CategoryId,
                    CategoryName = x.CategoryName,
                    Description = x.Description
                }).FirstOrDefault();
            return View("EditCategory", getCategory);
        }
        public IActionResult EditCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                var Category = _context.Category.Find(category.CategoryId);
                if (Category != null)
                {
                    Category.CategoryId = category.CategoryId;
                    Category.CategoryName = category.CategoryName;
                    Category.Description = category.Description;
                    _context.SaveChanges();
                    return RedirectToAction("Category");
                }
                else
                {

                    return NotFound();
                }
            }
            return View("EditCategory", Category);
            //return RedirectToAction("User");
        }
        public IActionResult Delete(Category category)
        {
            var deleteCategory = _context.Category.Where(x => x.CategoryId == category.CategoryId)
                .Select(x => new Category
                {
                    CategoryId = x.CategoryId,
                    CategoryName = x.CategoryName,
                    Description = x.Description
                }).FirstOrDefault();
            return View("DeleteCategory", deleteCategory);
        }

        public IActionResult DeleteCategory(int CategoryId)
        {
            var category = _context.Category.Find(CategoryId);
            if (category != null)
            {
                _context.Category.Remove(category);
                _context.SaveChanges();

                return Json(new { Status = true });
            }
            return Json(new { Status = false });
        }
    }
}