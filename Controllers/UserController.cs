using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace SurveyApplication.Controllers
{
    public class UserController : Controller
    {
        ProductDBEntities _context = new ProductDBEntities();
        public ActionResult Index()
        {
            var data = _context.TblUsers.ToList();
            return View(data);
        }
    }
}