using Microsoft.AspNetCore.Authentication.Cookies;
using SurveyApplication.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace SurveyApplication.Controllers
{
    public class AccountController : Controller
    {
        SqlConnection con = new SqlConnection("data source=132.148.1.136;initial catalog=ProductDB;user id=sa;password=vision");
        [Authorize]
        public ActionResult Dashboard()
        {
            return View();
        }
       
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel LoginViewModel,string returnUrl)
        {
            if (IsValidUser(LoginViewModel.Email, LoginViewModel.Password))
            {
                FormsAuthentication.SetAuthCookie(LoginViewModel.Email, false);
                return RedirectToAction("Dashboard", "Account");
            }
            else
            {
                ModelState.AddModelError("", "Your Email and password is incorrect");
            }
            return View(LoginViewModel);
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegistrationViewModel RegistrationViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!IsUserExist(RegistrationViewModel.Email))
                    {
                        string query = "insert into TblUser(FirstName,MiddleName,LastName,Address,City,DateofBirth,Email,Password,IsSurveyor,Role) values (@FirstName,@MiddleName,@LastName,@Address,@City,@DateofBirth,@Email,@Password,@IsSurveyor,@Role)";
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Connection = con;
                            cmd.Parameters.AddWithValue("@FirstName", RegistrationViewModel.FirstName);
                            cmd.Parameters.AddWithValue("@MiddleName", RegistrationViewModel.MiddleName);
                            cmd.Parameters.AddWithValue("@LastName", RegistrationViewModel.LastName);
                            cmd.Parameters.AddWithValue("@Address", RegistrationViewModel.Address);
                            cmd.Parameters.AddWithValue("@City", RegistrationViewModel.City);
                            cmd.Parameters.AddWithValue("@DateofBirth", RegistrationViewModel.DateofBirth);
                            cmd.Parameters.AddWithValue("@Email", RegistrationViewModel.Email);
                            
                            
                            cmd.Parameters.AddWithValue("@Password", Base64Encode(RegistrationViewModel.Password));
                            if (RegistrationViewModel.IsSurveyor == "Surveyor")
                            {
                                cmd.Parameters.AddWithValue("@IsSurveyor", true);
                                cmd.Parameters.AddWithValue("@Role", "Surveyor");
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@IsSurveyor", false);
                                cmd.Parameters.AddWithValue("@Role", "User");
                            }

                            con.Open();
                            int i = cmd.ExecuteNonQuery();
                            con.Close();
                            if (i > 0)
                            {
                                FormsAuthentication.SetAuthCookie(RegistrationViewModel.Email, false);
                                return RedirectToAction("Login", "Account");
                            }
                            else
                            {
                                ModelState.AddModelError("", "something went wrong try later!");
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "User with same email already exist!");
                        return Json(new { Status = false, Message = "Validation error" });
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Data is not correct");
                    return View(RegistrationViewModel);
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }
            return RedirectToAction("Login", "Account");
        }
        private bool IsUserExist(string email)
        {
            bool IsUserExist = false;
            string query = "select * from TblUser where Email=@email";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@email", email);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                if (dt.Rows.Count > 0)
                {
                    IsUserExist = true;
                }
            }
            return IsUserExist;
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        private bool IsValidUser(string email, string password)
        {
            var encryptpassowrd = Base64Encode(password);
            bool IsValid = false;
            string query = "select * from TblUser where Email=@email and Password=@Password";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", encryptpassowrd);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                con.Open();
                int i = cmd.ExecuteNonQuery();
                con.Close();
                if (dt.Rows.Count > 0)
                {
                    IsValid = true;
                }
            }
            return IsValid;
        }
        public ActionResult Index()
        {
            ProductDBEntities _context = new ProductDBEntities();
            var data = _context.TblUsers.ToList();
            return View(data);
        }
        public ActionResult Delete(int id)
        {
            ProductDBEntities _context = new ProductDBEntities();
            var data = _context.TblUsers.SingleOrDefault(x => x.UserId == id);
            _context.TblUsers.Remove(data);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }
    }
}