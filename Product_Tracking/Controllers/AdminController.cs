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
            return RedirectToAction("Users");
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
            pvm.RoleId = rolenameid.RoleId != null ? rolenameid.RoleId.Value : 0;
            return View(pvm);
        }
        [HttpPost, ActionName("Edit")]
        public ActionResult Edit_user(UserViewModel uvm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    tblUser tbluser = _db.tblUsers.Where(p => p.UserId == uvm.UserId).FirstOrDefault();
                    UserRole user = _db.UserRoles.Where(p => p.UserId == uvm.UserId).FirstOrDefault();
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
                        //tbluser.Photo = fup.FileName;
                        //if (tbluser.Photo == "")
                        //{
                        //    fup.SaveAs(Server.MapPath(null));
                        //}
                        //else
                        //{
                        //    fup.SaveAs(Server.MapPath("/Photo/" + fup.FileName));
                        //}
                        if (fup.FileName != "")
                        {
                            System.IO.File.Delete(Server.MapPath("~/Photo/" + uvm.Photo));
                            tbluser.Photo = fup.FileName;
                            fup.SaveAs(Server.MapPath("~/Photo/" + fup.FileName));
                        }
                        else
                        {
                            tbluser.Photo = uvm.Photo;
                        }
                    }
                    _db.SaveChanges();
                    ViewBag.Message = "User Created Successfully.";
                    return RedirectToAction("users");

                }
                catch (Exception e)
                {
                    return View("Error", new HandleErrorInfo(e, "Admin", "Edit"));
                }
            }
            return RedirectToAction("Edit");
        }

        public ActionResult Deals()
        {
            List<Deals> d = new List<Deals>();
            var deals = _db.tbl_Deals.ToList();
            foreach (tbl_Deals item in deals)
            {
                d.Add(new Deals() { DealsId = item.DealsId, DealsName = item.DealsName, DealsDiscription = item.DealsDiscription, DiscountPercent = item.DiscountPercent, StartDate = item.StartDate, EndDate = item.EndDate});
            }
                return View(d);
        }
        public ActionResult Deals_data()
        {
            return View();
        }
        [HttpPost/*, ActionName("InsertDeals")*/]
        public ActionResult Deals_data(Deals d)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    tbl_Deals deals = new tbl_Deals();
                    deals.DealsId = d.DealsId;
                    deals.DealsName = d.DealsName;
                    deals.DealsDiscription = d.DealsDiscription;
                    deals.DiscountPercent = d.DiscountPercent;
                    deals.StartDate = d.StartDate;
                    //if (d.StartDate < d.EndDate)
                    //{
                    //    deals.StartDate = d.StartDate;
                    //}
                    //else
                    //{
                    //    //swal("Good job!", "You clicked the button!", "success");
                    //    ViewBag.Message = "Success. You Clicked Button";
                    //    return View();
                    //}
                    deals.EndDate = d.EndDate;
                    //if (d.StartDate > d.EndDate)
                    //{
                    //    deals.EndDate = d.EndDate;
                    //}
                    //else
                    //{

                    //}                                       
                    _db.tbl_Deals.Add(deals);
                    _db.SaveChanges();
                    ViewBag.Message = "Deals Created Successfully.";
                    return RedirectToAction("Deals_data");

                }
                catch (Exception e)
                {
                    return View("Error", new HandleErrorInfo(e, "Admin", "CreateUser"));
                }
            }
            return View();
        }
        public ActionResult deletedeals(int id)
        {
            tbl_Deals tb = _db.tbl_Deals.Where(u => u.DealsId == id).FirstOrDefault();
            Deals uv = new Deals();
            uv.DealsName = tb.DealsName;
            uv.DealsDiscription = tb.DealsDiscription;
            uv.DiscountPercent = tb.DiscountPercent;
            uv.StartDate = tb.StartDate;
            uv.EndDate = tb.EndDate;
            return View(uv);
        }
        [HttpPost, ActionName("deletedeals")]
        public ActionResult deletedeal(int id)
        {
            tbl_Deals tb = _db.tbl_Deals.Where(u => u.DealsId == id).FirstOrDefault();
            _db.tbl_Deals.Remove(tb);
            _db.SaveChanges();
            return RedirectToAction("Deals");
        }
        public ActionResult EditDeals(int id)
        {
            tbl_Deals tb = _db.tbl_Deals.Where(p => p.DealsId == id).FirstOrDefault();
            Deals pvm = new Deals();
            pvm.DealsId = tb.DealsId;
            pvm.DealsName = tb.DealsName;
            pvm.DealsDiscription = tb.DealsDiscription;
            pvm.DiscountPercent = tb.DiscountPercent;
            pvm.StartDate = tb.StartDate;
            pvm.EndDate = tb.EndDate;            
            return View(pvm);
        }
        [HttpPost, ActionName("EditDeals")]
        public ActionResult EditDeal(Deals d)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    tbl_Deals deal = _db.tbl_Deals.Where(p => p.DealsId == d.DealsId).FirstOrDefault();
                    deal.DealsName = d.DealsName;
                    deal.DealsDiscription = d.DealsDiscription;
                    deal.DiscountPercent = d.DiscountPercent;
                    deal.StartDate = d.StartDate;
                    deal.EndDate = d.EndDate;
                    _db.SaveChanges();
                    ViewBag.Message = "Deal Updated Successfully.";
                    return RedirectToAction("Deals");
                }
                catch (Exception e)
                {
                    return View("Error", new HandleErrorInfo(e, "Admin", "EditDeals"));
                }
            }
            return RedirectToAction("Deals");
        }
    }
}