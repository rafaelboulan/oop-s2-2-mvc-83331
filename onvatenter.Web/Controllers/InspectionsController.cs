using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using onvatenter.Models.Data;
using Microsoft.AspNetCore.Authorization;

namespace onvatenter.Web.Controllers
{
    [Authorize(Roles = "Admin,Inspector")]
    public class InspectionsController : Controller
    {
        private readonly AppDbContext _db;

        public InspectionsController(AppDbContext db)
        {
            _db = db;
        }

        // GET: /Inspections
        public IActionResult Index()
        {
            var inspections = _db.Inspections
                .Include(i => i.Premises)
                .OrderByDescending(i => i.InspectionDate)
                .ToList();

            ViewBag.Breadcrumbs = new List<dynamic>
            {
                new { Name = "Home", Url = "/" },
                new { Name = "Inspections", Url = (string)null }
            };

            return View(inspections);
        }

        // GET: /Inspections/{id}
        public IActionResult Details(int id)
        {
            var inspection = _db.Inspections
                .Include(i => i.Premises)
                .Include(i => i.FollowUps)
                .FirstOrDefault(i => i.Id == id);
            if (inspection == null) return NotFound();

            ViewBag.Breadcrumbs = new List<dynamic>
            {
                new { Name = "Home", Url = "/" },
                new { Name = "Inspections", Url = "/Inspections" },
                new { Name = "Inspection #" + inspection.Id, Url = (string)null }
            };

            return View(inspection);
        }

        // GET: /Inspections/Create
        public IActionResult Create()
        {
            var premises = _db.Premises.OrderBy(p => p.Name).ToList();
            ViewBag.Premises = new SelectList(premises, "Id", "Name");

            ViewBag.Breadcrumbs = new List<dynamic>
            {
                new { Name = "Home", Url = "/" },
                new { Name = "Inspections", Url = "/Inspections" },
                new { Name = "Create", Url = (string)null }
            };

            return View();
        }

        // POST: /Inspections/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inspection inspection)
        {
            if (ModelState.IsValid)
            {
                inspection.CreatedAt = DateTime.Now;
                _db.Inspections.Add(inspection);
                _db.SaveChanges();
                return RedirectToAction("Details", new { id = inspection.Id });
            }
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                Console.WriteLine("MODELSTATE ERRORS:");
                Console.WriteLine(string.Join("\n", errors));
            }

            var premises = _db.Premises.OrderBy(p => p.Name).ToList();
            ViewBag.Premises = new SelectList(premises, "Id", "Name", inspection.PremisesId);
            return View(inspection);
        }

        // GET: /Inspections/{id}/Edit
        public IActionResult Edit(int id)
        {
            var inspection = _db.Inspections.Find(id);
            if (inspection == null) return NotFound();

            var premises = _db.Premises.OrderBy(p => p.Name).ToList();
            ViewBag.Premises = new SelectList(premises, "Id", "Name", inspection.PremisesId);

            ViewBag.Breadcrumbs = new List<dynamic>
            {
                new { Name = "Home", Url = "/" },
                new { Name = "Inspections", Url = "/Inspections" },
                new { Name = "Inspection #" + inspection.Id, Url = "/Inspections/Details/" + id },
                new { Name = "Edit", Url = (string)null }
            };

            return View(inspection);
        }

        // POST: /Inspections/{id}/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Inspection inspection)
        {
            if (id != inspection.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Inspections.Update(inspection);
                    _db.SaveChanges();
                    return RedirectToAction("Details", new { id = inspection.Id });
                }
                catch (Exception)
                {
                    var premises = _db.Premises.OrderBy(p => p.Name).ToList();
                    ViewBag.Premises = new SelectList(premises, "Id", "Name", inspection.PremisesId);
                    return View(inspection);
                }
            }

            var premisesList = _db.Premises.OrderBy(p => p.Name).ToList();
            ViewBag.Premises = new SelectList(premisesList, "Id", "Name", inspection.PremisesId);
            return View(inspection);
        }

        // GET: /Inspections/{id}/Delete
        public IActionResult Delete(int id)
        {
            var inspection = _db.Inspections.Include(i => i.Premises).FirstOrDefault(i => i.Id == id);
            if (inspection == null) return NotFound();

            ViewBag.Breadcrumbs = new List<dynamic>
            {
                new { Name = "Home", Url = "/" },
                new { Name = "Inspections", Url = "/Inspections" },
                new { Name = "Inspection #" + inspection.Id, Url = "/Inspections/Details/" + id },
                new { Name = "Delete", Url = (string)null }
            };

            return View(inspection);
        }

        // POST: /Inspections/{id}/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var inspection = _db.Inspections.Find(id);
            if (inspection != null)
            {
                _db.Inspections.Remove(inspection);
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
