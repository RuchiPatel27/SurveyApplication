using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SurveyApplication;
namespace SurveyApplication.Controllers
{
    public class SurveyController : Controller
    {
        ProductDBEntities _context = new ProductDBEntities();
        [Authorize(Roles = "Surveyor,User")]
        public ActionResult Index()
        {
            var data = _context.TblUsers.Where(x => x.Email == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault().UserId;
            var tdata = _context.TblUsers.Where(x => x.Email == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault().Role;
            if (tdata == "Surveyor")
            {
                var model =  _context.TblSurveys.Where(x => x.UserId == data).ToList();
                return View(model);
            }
            else
            {
                var model = _context.TblSurveys.ToList();
                return View(model);
            }
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [Authorize(Roles = "Surveyor")]
        [HttpPost]
        public ActionResult Create(TblSurvey model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.UserId = _context.TblUsers.Where(x => x.Email == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault().UserId;
                    model.CreatedBy = "admin@gmail.com";
                    model.CreatedON = DateTime.Now;
                    _context.TblSurveys.Add(model);
                    _context.SaveChanges();
                    return RedirectToAction("Index", model);
                }
                else
                {
                    ModelState.AddModelError("", "Data is not correct");
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }
            return View();
        }
        [Authorize(Roles = "Surveyor")]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var data = _context.TblSurveys.Where(x => x.SurveyId == id).FirstOrDefault();
            return View(data);
        }
        [Authorize(Roles = "Surveyor")]
        [HttpPost]
        public ActionResult Edit(TblSurvey model,int id)
        {
            var data = _context.TblSurveys.Where(x => x.SurveyId == model.SurveyId).FirstOrDefault();
            if(data != null)
            {
                data.Title = model.Title;
                data.FromDate = model.FromDate;
                data.ToDate = model.ToDate;
                data.CreatedBy = "admin@gmail.com";
                data.CreatedON = DateTime.Now;
                //data.UserId = _context.TblUsers.Where(x => x.Email == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault().UserId;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Surveyor")]
        public ActionResult Delete(int id)
        {
            var data = _context.TblSurveys.SingleOrDefault(x => x.SurveyId == id);
            _context.TblSurveys.Remove(data);
            _context.SaveChanges();
            return RedirectToAction("Index",data);
        }
    }
}