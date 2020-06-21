using Product_Tracking.Models;
using Product_Tracking.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Product_Tracking.Controllers
{
    public class AdminController : Controller
    {
        Product_TrackingEntities _db = new Product_TrackingEntities();
        // GET: Admin
        //[Route("Home")]
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.countuser = _db.tblUsers.SqlQuery(" SELECT * FROM tblUser ").Count();
            Session["countusers"] = ViewBag.countuser;
            return View();
        }
        public ActionResult Users()
        {
            List<UserViewModel> lst = new List<UserViewModel>();
            var products = _db.tblUsers.ToList();
            foreach (tblUser item in products)
            {
                var rolenameid = _db.UserRoles.Where(a => a.UserId==item.UserId).FirstOrDefault();
                string role=null;
                if (rolenameid != null)
                {
                    if (rolenameid.RoleId == 3)
                    {
                        role = "user";
                    }
                    else
                    {
                        role = "Admin";
                    }
                }
                lst.Add(new UserViewModel() { UserId = item.UserId,Username = item.Username, Password = item.Password, Email = item.Email, Address = item.Address, PhoneNumber = item.PhoneNumber,Rolename =role ,Photo = item.Photo});
            }
            return View(lst);
        }
        public ActionResult CreateUser()
        {
            ViewBag.UserRole = _db.tbl_Role.ToList();
            return View();
        }
        [HttpPost]
        public ActionResult CreateUser(UserViewModel uvm)

        {
            if (ModelState.IsValid)
            {
                try
                {
                    tblUser tbluser = new tblUser();
                    UserRole user = new UserRole();
                    tbluser.Username = uvm.Username;
                    tbluser.Password = uvm.Password;
                    tbluser.PhoneNumber = uvm.PhoneNumber;
                    tbluser.Address = uvm.Address;
                    user.RoleId = uvm.RoleId;
                    user.UserId = uvm.UserId;
                    tbluser.Email = uvm.Email;
                    HttpPostedFileBase fup = Request.Files["Photo"];
                    if (fup != null)
                    {
                        tbluser.Photo = fup.FileName;
                        if (tbluser.Photo == "")
                        {
                            fup.SaveAs(Server.MapPath(null));
                        }
                        else
                        {
                            fup.SaveAs(Server.MapPath("/Photo/" + fup.FileName));
                        }
                    }

                    _db.tblUsers.Add(tbluser);
                    _db.UserRoles.Add(user);
                    _db.SaveChanges();
                    ViewBag.Message = "User Created Successfully.";
                    return RedirectToAction("users");

                }
                catch (Exception e)
                {
                    return View("Error", new HandleErrorInfo(e, "Admin", "CreateUser"));
                }
            }
            return View();
        }
        public ActionResult deleteUser(int id)
        {
            tblUser tb = _db.tblUsers.Where(u => u.UserId == id).FirstOrDefault();
            UserViewModel uv = new UserViewModel();
            //uv.UserId = tb.UserId;
            uv.Username = tb.Username;
            //uv.Password = tb.Password;
            uv.Address = tb.Address;
            uv.PhoneNumber = tb.PhoneNumber;
            uv.Email = tb.Email;
            uv.Photo = tb.Photo;
            return View(uv);
        }
        [HttpPost, ActionName("deleteUser")]
        public ActionResult deleteUsers(int id)
        {
            tblUser tb = _db.tblUsers.Where(u => u.UserId == id).FirstOrDefault();
            UserRole user = _db.UserRoles.Where(u => u.UserId == id).FirstOrDefault();
            _db.tblUsers.Remove(tb);
            if (user != null)
            {
                _db.UserRoles.Remove(user);
            }
            _db.SaveChanges();
            return RedirectToAction("Users"); ;
        }
        public ActionResult Edit(int id)
        {
            ViewBag.tbl_Role = _db.tbl_Role.ToList();
            tblUser tb = _db.tblUsers.Where(p => p.UserId == id).FirstOrDefault();
            UserViewModel pvm = new UserViewModel();
            pvm.UserId = tb.UserId;
            pvm.Password = tb.Password;
            pvm.ConfirmPassword = tb.Password;
            pvm.Email = tb.Email;
            pvm.Username = tb.Username;
            pvm.Address = tb.Address;
            pvm.PhoneNumber = tb.PhoneNumber;
            pvm.Photo = tb.Photo;
            var rolenameid = _db.UserRoles.Where(a => a.UserId == tb.UserId).FirstOrDefault();
            //pvm.RoleId = rolenameid.RoleId;
            return View(pvm);
        }
    }
}