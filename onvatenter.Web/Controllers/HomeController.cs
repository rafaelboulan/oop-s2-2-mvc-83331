using Microsoft.AspNetCore.Mvc;
using onvatenter.Models.Data;

namespace onvatenter.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;

        public HomeController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            ViewBag.PremisesCount = _db.Premises.Count();
            ViewBag.InspectionsCount = _db.Inspections.Count();
            ViewBag.FollowUpsCount = _db.FollowUps.Count();

            ViewBag.Breadcrumbs = new List<dynamic>
            {
                new { Name = "Home", Url = (string)null }
            };

            return View();
        }
    }
}
