using Product_Tracking.Models;
using Product_Tracking.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Product_Tracking.Controllers
{
    public class AccountController : Controller
    {
        Product_TrackingEntities _db = new Product_TrackingEntities();
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }
        //login
        [HttpPost]
        public ActionResult Login(LoginViewModel l, string ReturnUrl = "")
        {
            var users = _db.tblUsers.Where(a => a.Username == l.Username && a.Password == l.Password).FirstOrDefault();
            if (users != null)
            {
                if (users.Username == l.Username && users.Password == l.Password)
                {
                    var userrole = _db.UserRoles.Where(a => a.UserId == users.UserId).FirstOrDefault();
                    if (userrole.RoleId == 4)
                    {
                        Session.Add("fullname", users.Username);
                        Session["Photo"] = users.Photo;
                        Session["email"] = users.Email;
                        FormsAuthentication.SetAuthCookie(l.Username, true);
                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {                            
                            return RedirectToAction("Index", "Admin",new { loginStatus=true});
                        }
                    }
                    else if (userrole.RoleId == 3)
                    {
                        Session.Add("fullname", users.Username);
                        FormsAuthentication.SetAuthCookie(l.Username, true);
                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "User",new { loginStatus = true });
                        }
                    }
                }
            }
            else
            {
                if (Url.IsLocalUrl(ReturnUrl))
                {
                    return Redirect(ReturnUrl);
                }
                ModelState.AddModelError("", "Invalid User");
            }
            return View();
        }
        public ActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Signup(UserViewModel uvm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    tblUser tbluser = new tblUser();
                    tbluser.Username = uvm.Username;
                    tbluser.Password = uvm.Password;
                    tbluser.PhoneNumber = uvm.PhoneNumber;
                    tbluser.Address = uvm.Address;
                    tbluser.Email = uvm.Email;
                    HttpPostedFileBase fup = Request.Files["Photo"];
                    if (fup != null)
                    {
                        tbluser.Photo = fup.FileName;
                        if (tbluser.Photo == "")
                        {
                            fup.SaveAs(Server.MapPath( null));
                        }
                        else
                        {
                            fup.SaveAs(Server.MapPath("/Photo/" + fup.FileName));
                        }
                    }
                    
                        _db.tblUsers.Add(tbluser);
                        _db.SaveChanges();
                        ViewBag.Message = "User Created Successfully.";
                        return RedirectToAction("Signup");
                    
                }
                catch(Exception e)
                {
                    return View("Error", new HandleErrorInfo(e,"Account","Signup"));
                }                
            }
            return View();
        }
        [Authorize]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }

    }
}