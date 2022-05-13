using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace SurveyApplication.Controllers
{
    public class QuestionController : Controller
    {
        ProductDBEntities _context = new ProductDBEntities();
        [HttpGet]
        public ActionResult Index(int id)
        {
            TblQuestion model = _context.TblQuestions.Find(id);
            var data = _context.TblQuestions.Where(x => x.SurveyId == id)
                .Select(x => new QuestionModel { QuesId = x.QueId, Question = x.Question }).ToList();
            ViewBag.data = data;
            var datas = _context.TblQuestions.Where(x => x.SurveyId == id).Select(x => x.ControlType).ToList();
            ViewBag.datas = datas;
            var options = _context.TblQuestions.Where(x => x.SurveyId == id).Select(x => x.ControlOptions).ToList();
            ViewBag.options = options;
            ViewBag.Count = options.Count();
            return View();
        }
         [HttpGet]
        [Authorize(Roles = "Surveyor")]
        public ActionResult QuestionList()
        {
            var data = _context.TblQuestions.ToList();
            return View(data);
        }
        [Authorize(Roles = "Surveyor")]
        [HttpGet]
        public ActionResult Create(int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TblSurvey model = _context.TblSurveys.Find(id);
                    var data = _context.TblSurveys.Where(x => x.SurveyId == id).Select(x => x.Title).FirstOrDefault();
                    ViewBag.SurveyName = data;
                    ViewBag.SurveyId = id;
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
        [HttpPost]  
        public JsonResult InsertData(TblQuestion model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.Question = model.Question;
                    model.SurveyId = model.SurveyId;
                    model.ControlType = model.ControlType;
                    model.ControlOptions = model.ControlOptions;
                    model.CreatedBy = model.CreatedBy;
                    model.CreatedON = DateTime.Now;
                    _context.TblQuestions.Add(model);
                    _context.SaveChanges();
                    //return RedirectToAction("Index", "Survey");
                    return Json(new { Status = true, Message = "Inserted Survey Questions" });
                }
                else
                {
                    ModelState.AddModelError("", "Data is not correct");
                    return Json(new { Status = false, Message = "Validation error" });
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                return Json(new { Status = false, Message = "Something went wrong" });
            }
            //return View("~/Views/Survey/Index");
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            TblQuestion model = _context.TblQuestions.Find(id);
           var data =  _context.TblQuestions.Where(x => x.QueId == model.QueId).Select(x => x.SurveyId == model.SurveyId).FirstOrDefault();
            ViewBag.data = data;
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(TblQuestion model,int id)
        {
            var data = _context.TblQuestions.Where(x => x.QueId == model.QueId).FirstOrDefault();
            if (data != null)
            {
                data.Question = model.Question;
                data.ControlOptions = model.ControlOptions;
                data.ControlType = model.ControlType;
                data.CreatedBy = data.CreatedBy;
                data.CreatedON = DateTime.Now;
                _context.SaveChanges();
            }
            return RedirectToAction("Index",model);
        }
        public ActionResult Delete(int id)
        {
            var data = _context.TblQuestions.SingleOrDefault(x => x.QueId == id);
            _context.TblQuestions.Remove(data);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult AddUserAnswer()
        {
            return View();
        }
        [HttpPost]
        public JsonResult AddUserAnswer(string UserAnswer,int QuesId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TblUserAnswer model = new TblUserAnswer();
                    model.QuesId = QuesId;
                    model.UserId = _context.TblUsers.Where(x => x.Email == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault().UserId;
                    model.UserAnswer = UserAnswer;
                    _context.TblUserAnswers.Add(model);
                    _context.SaveChanges();
                    return Json(new { message = "Inserted" });
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
            return Json(new { message = "" });
        }
    }
}