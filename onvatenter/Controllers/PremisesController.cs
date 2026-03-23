using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using onvatenter.Data;

namespace onvatenter.Controllers
{
    public class PremisesController : Controller
    {
        private readonly AppDbContext _db;

        public PremisesController(AppDbContext db)
        {
            _db = db;
        }

        // GET: /Premises
        public IActionResult Index()
        {
            var premises = _db.Premises.OrderBy(p => p.Name).ToList();

            ViewBag.Breadcrumbs = new List<dynamic>
            {
                new { Name = "Home", Url = "/" },
                new { Name = "Premises", Url = (string)null }
            };

            return View(premises);
        }

        // GET: /Premises/{id}
        public IActionResult Details(int id)
        {
            var premises = _db.Premises
                .Include(p => p.Inspections)
                .FirstOrDefault(p => p.Id == id);
            if (premises == null) return NotFound();

            ViewBag.Breadcrumbs = new List<dynamic>
            {
                new { Name = "Home", Url = "/" },
                new { Name = "Premises", Url = "/Premises" },
                new { Name = premises.Name, Url = (string)null }
            };

            return View(premises);
        }

        // GET: /Premises/Create
        public IActionResult Create()
        {
            ViewBag.Breadcrumbs = new List<dynamic>
            {
                new { Name = "Home", Url = "/" },
                new { Name = "Premises", Url = "/Premises" },
                new { Name = "Create", Url = (string)null }
            };

            return View();
        }

        // POST: /Premises/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Premises premises)
        {
            if (ModelState.IsValid)
            {
                premises.CreatedAt = DateTime.Now;
                _db.Premises.Add(premises);
                _db.SaveChanges();
                return RedirectToAction("Details", new { id = premises.Id });
            }
            return View(premises);
        }

        // GET: /Premises/{id}/Edit
        public IActionResult Edit(int id)
        {
            var premises = _db.Premises.Find(id);
            if (premises == null) return NotFound();

            ViewBag.Breadcrumbs = new List<dynamic>
            {
                new { Name = "Home", Url = "/" },
                new { Name = "Premises", Url = "/Premises" },
                new { Name = premises.Name, Url = "/Premises/Details/" + id },
                new { Name = "Edit", Url = (string)null }
            };

            return View(premises);
        }

        // POST: /Premises/{id}/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Premises premises)
        {
            if (id != premises.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Premises.Update(premises);
                    _db.SaveChanges();
                    return RedirectToAction("Details", new { id = premises.Id });
                }
                catch (Exception)
                {
                    return View(premises);
                }
            }
            return View(premises);
        }

        // GET: /Premises/{id}/Delete
        public IActionResult Delete(int id)
        {
            var premises = _db.Premises.Find(id);
            if (premises == null) return NotFound();

            ViewBag.Breadcrumbs = new List<dynamic>
            {
                new { Name = "Home", Url = "/" },
                new { Name = "Premises", Url = "/Premises" },
                new { Name = premises.Name, Url = "/Premises/Details/" + id },
                new { Name = "Delete", Url = (string)null }
            };

            return View(premises);
        }

        // POST: /Premises/{id}/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var premises = _db.Premises.Find(id);
            if (premises != null)
            {
                _db.Premises.Remove(premises);
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
