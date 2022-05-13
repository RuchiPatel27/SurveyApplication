using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace SurveyApplication.Controllers
{
    public class HomeController : Controller
    {
        ProductDBEntities db = new ProductDBEntities();
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult OnlyAdminView()
        {
            var data = db.TblUsers.Where(x => x.Role == "Surveyor").ToList();
            ViewBag.data = data;
            return View(ViewBag.data);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult UserList()
        {
            var data = db.TblUsers.Where(x => x.Role == "User").ToList();
            ViewBag.data = data;
            return View(ViewBag.data);
        }
    }
}