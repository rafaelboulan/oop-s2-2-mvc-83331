using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using onvatenter.Models.Data;

namespace onvatenter.Web.Controllers
{
    public class FollowUpsController : Controller
    {
        private readonly AppDbContext _db;

        public FollowUpsController(AppDbContext db)
        {
            _db = db;
        }

        // GET: /FollowUps
        public IActionResult Index()
        {
            var followUps = _db.FollowUps
                .Include(f => f.Inspection)
                .OrderBy(f => f.DueDate)
                .ToList();

            ViewBag.Breadcrumbs = new List<dynamic>
            {
                new { Name = "Home", Url = "/" },
                new { Name = "Follow-ups", Url = (string)null }
            };

            return View(followUps);
        }

        // GET: /FollowUps/{id}
        public IActionResult Details(int id)
        {
            var followUp = _db.FollowUps
                .Include(f => f.Inspection)
                .FirstOrDefault(f => f.Id == id);
            if (followUp == null) return NotFound();

            ViewBag.Breadcrumbs = new List<dynamic>
            {
                new { Name = "Home", Url = "/" },
                new { Name = "Follow-ups", Url = "/FollowUps" },
                new { Name = "Follow-up #" + followUp.Id, Url = (string)null }
            };

            return View(followUp);
        }

        // GET: /FollowUps/Create
        public IActionResult Create()
        {
            var inspections = _db.Inspections
                .Include(i => i.Premises)
                .OrderByDescending(i => i.InspectionDate)
                .ToList();
            ViewBag.Inspections = new SelectList(inspections, "Id", "Notes");

            ViewBag.Breadcrumbs = new List<dynamic>
            {
                new { Name = "Home", Url = "/" },
                new { Name = "Follow-ups", Url = "/FollowUps" },
                new { Name = "Create", Url = (string)null }
            };

            return View();
        }

        // POST: /FollowUps/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(FollowUp followUp)
        {
            if (ModelState.IsValid)
            {
                followUp.CreatedAt = DateTime.Now;
                _db.FollowUps.Add(followUp);
                _db.SaveChanges();
                return RedirectToAction("Details", new { id = followUp.Id });
            }

            var inspections = _db.Inspections
                .Include(i => i.Premises)
                .OrderByDescending(i => i.InspectionDate)
                .ToList();
            ViewBag.Inspections = new SelectList(inspections, "Id", "Notes", followUp.InspectionId);
            return View(followUp);
        }

        // GET: /FollowUps/{id}/Edit
        public IActionResult Edit(int id)
        {
            var followUp = _db.FollowUps.Find(id);
            if (followUp == null) return NotFound();

            var inspections = _db.Inspections
                .Include(i => i.Premises)
                .OrderByDescending(i => i.InspectionDate)
                .ToList();
            ViewBag.Inspections = new SelectList(inspections, "Id", "Notes", followUp.InspectionId);

            ViewBag.Breadcrumbs = new List<dynamic>
            {
                new { Name = "Home", Url = "/" },
                new { Name = "Follow-ups", Url = "/FollowUps" },
                new { Name = "Follow-up #" + followUp.Id, Url = "/FollowUps/Details/" + id },
                new { Name = "Edit", Url = (string)null }
            };

            return View(followUp);
        }

        // POST: /FollowUps/{id}/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, FollowUp followUp)
        {
            if (id != followUp.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    _db.FollowUps.Update(followUp);
                    _db.SaveChanges();
                    return RedirectToAction("Details", new { id = followUp.Id });
                }
                catch (Exception)
                {
                    var inspections = _db.Inspections
                        .Include(i => i.Premises)
                        .OrderByDescending(i => i.InspectionDate)
                        .ToList();
                    ViewBag.Inspections = new SelectList(inspections, "Id", "Notes", followUp.InspectionId);
                    return View(followUp);
                }
            }

            var inspectionsList = _db.Inspections
                .Include(i => i.Premises)
                .OrderByDescending(i => i.InspectionDate)
                .ToList();
            ViewBag.Inspections = new SelectList(inspectionsList, "Id", "Notes", followUp.InspectionId);
            return View(followUp);
        }

        // GET: /FollowUps/{id}/Delete
        public IActionResult Delete(int id)
        {
            var followUp = _db.FollowUps.Include(f => f.Inspection).FirstOrDefault(f => f.Id == id);
            if (followUp == null) return NotFound();

            ViewBag.Breadcrumbs = new List<dynamic>
            {
                new { Name = "Home", Url = "/" },
                new { Name = "Follow-ups", Url = "/FollowUps" },
                new { Name = "Follow-up #" + followUp.Id, Url = "/FollowUps/Details/" + id },
                new { Name = "Delete", Url = (string)null }
            };

            return View(followUp);
        }

        // POST: /FollowUps/{id}/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var followUp = _db.FollowUps.Find(id);
            if (followUp != null)
            {
                _db.FollowUps.Remove(followUp);
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
