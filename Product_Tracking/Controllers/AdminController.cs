using Product_Tracking.Models;
using Product_Tracking.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Product_Tracking.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        Product_TrackingEntities _db = new Product_TrackingEntities();
        // GET: Admin
        //[Route("Home")]
        [Authorize]
        public ActionResult Index(bool? loginStatus)
        {
            if (Session["Photo"] != null)
            {
                ViewBag.countuser = _db.tblUsers.SqlQuery(" SELECT * FROM tblUser ").Count();
                ViewBag.countdeals = _db.tbl_Deals.SqlQuery(" SELECT * FROM tbl_Deals ").Count();
                ViewBag.category = _db.tbl_ProductCategory.SqlQuery(" SELECT * FROM tbl_ProductCategory").Count();
                ViewBag.store = _db.tbl_Store.SqlQuery(" SELECT * FROM tbl_Store").Count();
                var productexpcheck = _db.tbl_Product.Where(u => u.ProductExpireDate == DateTime.Today).ToList();
                List<string> productexplst = new List<string>();
                foreach (var item in productexpcheck)
                {
                    productexplst.Add(item.ProductName);
                }
                if (productexplst.Count > 0)
                {
                    TempData["productexplst"] = productexplst;
                }
                //TempData["AlertMessage"] = "hello" + " " + "is expiring today";
                Session["countusers"] = ViewBag.countuser;
                Session["countdeals"] = ViewBag.countuser;
                Session["category"] = ViewBag.category;
                Session["store"] = ViewBag.store;
                TempData["loginStatus"] = loginStatus;

                return View();
            }
            return RedirectToAction("Login", "Account");
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
                        if (fup.FileName != null)
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
                    deals.EndDate = d.EndDate;
                    _db.tbl_Deals.Add(deals);
                    _db.SaveChanges();
                    //----
                    List<tblUser> tbluser = new List<tblUser>();
                    var user = _db.tblUsers.ToList();
                    foreach (tblUser item in user)
                    {
                        string from = "trustboy100@gmail.com";
                        using (MailMessage mm = new MailMessage(from,item.Email, deals.DealsName, "Discount Percent: "+deals.DiscountPercent+"%"+"\n"+"StartDate: "+deals.StartDate+"\n"+"EndDate: "+deals.EndDate))
                        {
                            mm.IsBodyHtml = false;
                            SmtpClient smtp = new SmtpClient();
                            smtp.Host = "smtp.gmail.com";
                            smtp.EnableSsl = true;
                            smtp.UseDefaultCredentials = false;
                            NetworkCredential NetworkCred = new NetworkCredential(from, "YogeshPassword");
                            smtp.Credentials = NetworkCred;
                            smtp.Port = 587;
                            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                            smtp.Send(mm);
                            ViewBag.Message = "Mail Sent";
                        }
                    }
                        //--
                        ViewBag.Message = "Deals Created Successfully.";
                    return RedirectToAction("Deals");

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
            if (tb.DealsId != null)
            {
                pvm.DealsId = tb.DealsId;
            }
            else
            {
                pvm.DealsId = 0;
            }
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
        public ActionResult Category()
        {
            List<CategoryViewModel> category = new List<CategoryViewModel>();
            var cat = _db.tbl_ProductCategory.ToList();
            foreach (tbl_ProductCategory item in cat)
            {
                category.Add(new CategoryViewModel() { CategoryId = item.CategoryId, CategoryName = item.CategoryName});
            }
            return View(category);
        }
        public ActionResult InsertCategory()
        {
            return View();
        }
        [HttpPost]
        public ActionResult InsertCategory(CategoryViewModel cvm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    tbl_ProductCategory category = new tbl_ProductCategory();
                    category.CategoryId = cvm.CategoryId;
                    category.CategoryName = cvm.CategoryName;
                    _db.tbl_ProductCategory.Add(category);
                    _db.SaveChanges();
                    ViewBag.Message = "Category Created Successfully.";
                    return RedirectToAction("Category");

                }
                catch (Exception e)
                {
                    return View("Error", new HandleErrorInfo(e, "Admin", "InsertCategory"));
                }
            }
            return View();
        }
        public ActionResult DeleteCategory(int id)
        {
            tbl_ProductCategory tb = _db.tbl_ProductCategory.Where(u => u.CategoryId == id).FirstOrDefault();
            CategoryViewModel cvm = new CategoryViewModel();
            cvm.CategoryId = tb.CategoryId;
            cvm.CategoryName = tb.CategoryName;
            return View(cvm);
        }
        [HttpPost, ActionName("DeleteCategory")]
        public ActionResult DeleteCategories(int id)
        {
            tbl_ProductCategory tb = _db.tbl_ProductCategory.Where(u => u.CategoryId == id).FirstOrDefault();
            _db.tbl_ProductCategory.Remove(tb);
            _db.SaveChanges();
            return RedirectToAction("Category");
        }
        public ActionResult EditCategory(int id)
        {
            tbl_ProductCategory tb = _db.tbl_ProductCategory.Where(p => p.CategoryId == id).FirstOrDefault();
            CategoryViewModel cvm = new CategoryViewModel();
            cvm.CategoryId = tb.CategoryId;
            cvm.CategoryName = tb.CategoryName;
            return View(cvm);
        }
        [HttpPost, ActionName("EditCategory")]
        public ActionResult EditCategories(CategoryViewModel cvm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    tbl_ProductCategory category = _db.tbl_ProductCategory.Where(p => p.CategoryId == cvm.CategoryId).FirstOrDefault();
                    category.CategoryId = cvm.CategoryId;
                    category.CategoryName = cvm.CategoryName;
                    _db.SaveChanges();
                    ViewBag.Message = "Category Updated Successfully.";
                    return RedirectToAction("category");
                }
                catch (Exception e)
                {
                    return View("Error", new HandleErrorInfo(e, "Admin", "EditCategory"));
                }
            }
            return RedirectToAction("Category");
        }
        public ActionResult Store()
        {
            List<StoreViewModel> store = new List<StoreViewModel>();
            var stores = _db.tbl_Store.ToList();
            foreach (tbl_Store item in stores)
            {
                store.Add(new StoreViewModel() { StoreId = item.StoreId, StoreName = item.StoreName,StoreLocation= item.StoreLocation,StoreCapacity = item.StoreCapacity });
            }
            return View(store);
        }
        public ActionResult InsertStore()
        {
            return View();
        }
        [HttpPost]
        public ActionResult InsertStore(StoreViewModel svm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    tbl_Store store = new tbl_Store();
                    store.StoreId = svm.StoreId;
                    store.StoreName = svm.StoreName;
                    store.StoreLocation = svm.StoreLocation;
                    store.StoreCapacity = svm.StoreCapacity;
                    _db.tbl_Store.Add(store);
                    _db.SaveChanges();
                    ViewBag.Message = "Store Created Successfully.";
                    return RedirectToAction("Store");

                }
                catch (Exception e)
                {
                    return View("Error", new HandleErrorInfo(e, "Admin", "InsertStore"));
                }
            }
            return View();
        }
        public ActionResult DeleteStore(int id)
        {
            tbl_Store tb = _db.tbl_Store.Where(u => u.StoreId == id).FirstOrDefault();
            StoreViewModel svm = new StoreViewModel();
            svm.StoreId = tb.StoreId;
            svm.StoreName = tb.StoreName;
            svm.StoreLocation = tb.StoreLocation;
            svm.StoreCapacity = tb.StoreCapacity;
            return View(svm);
        }
        [HttpPost, ActionName("DeleteStore")]
        public ActionResult DeleteStores(int id)
        {
            tbl_Store tb = _db.tbl_Store.Where(u => u.StoreId == id).FirstOrDefault();
            _db.tbl_Store.Remove(tb);
            _db.SaveChanges();
            return RedirectToAction("Store");
        }
        public ActionResult EditStore(int id)
        {
            tbl_Store tb = _db.tbl_Store.Where(p => p.StoreId == id).FirstOrDefault();
            StoreViewModel svm = new StoreViewModel();
            svm.StoreId = tb.StoreId;
            svm.StoreName = tb.StoreName;
            svm.StoreLocation = tb.StoreLocation;
            svm.StoreCapacity = tb.StoreCapacity;
            return View(svm);
        }
        [HttpPost, ActionName("EditStore")]
        public ActionResult EditStores(StoreViewModel svm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    tbl_Store store = _db.tbl_Store.Where(p => p.StoreId == svm.StoreId).FirstOrDefault();
                    store.StoreId = svm.StoreId;
                    store.StoreName = svm.StoreName;
                    store.StoreLocation = svm.StoreName;
                    store.StoreCapacity = svm.StoreCapacity;
                    _db.SaveChanges();
                    ViewBag.Message = "Store Updated Successfully.";
                    return RedirectToAction("Store");
                }
                catch (Exception e)
                {
                    return View("Error", new HandleErrorInfo(e, "Admin", "EditStore"));
                }
            }
            return RedirectToAction("Store");
        }
        public ActionResult Product()
        {
            List<ProductViewModel> lst = new List<ProductViewModel>();
            var products = _db.tbl_Product.ToList();
            foreach (tbl_Product item in products)
            {
                var storeid = _db.tbl_ProductStore.Where(a => a.ProductId == item.ProductId).FirstOrDefault();
                var storename = _db.tbl_Store.Where(a => a.StoreId == storeid.StoreId).FirstOrDefault();
                var dealsname = _db.tbl_Deals.Where(a => a.DealsId == item.DealsId).FirstOrDefault();
                var categoryname = _db.tbl_ProductCategory.Where(a => a.CategoryId == item.CategoryId).FirstOrDefault();
                var Statusname = _db.tbl_ProductStatus.Where(a => a.StatusId == item.StatusId).FirstOrDefault();
                lst.Add(new ProductViewModel() { ProductId = item.ProductId, ProductName = item.ProductName, ProductDescription = item.ProductDescription, StoreName = storename.StoreName,DealsName = dealsname.DealsName,CategoryName = categoryname.CategoryName, StatusName = Statusname.StatusName, ProductPackingDate = item.ProductPackingDate, ProductExpireDate = item.ProductExpireDate});
            }
            return View(lst);
        }
        public ActionResult InsertProduct()
        {
            InitCommon();
            return View();
        }

        private void InitCommon()
        {
            ViewBag.CategoryName = _db.tbl_ProductCategory.ToList();
            ViewBag.DealsName = _db.tbl_Deals.ToList();
            ViewBag.storeName = _db.tbl_Store.ToList();
            ViewBag.StatusName = _db.tbl_ProductStatus.ToList();
        }

        [HttpPost]
        public ActionResult InsertProduct(ProductViewModel pvm)
        {
            InitCommon();
            if (ModelState.IsValid)
                {
                    if (pvm.StoreId == 0 || pvm.CategoryId == 0 || pvm.DealsId == 0)
                    {
                        ModelState.AddModelError(string.Empty, "you can not leave the empty dropdown please select any of these");
                    }
                    try
                    {
                        tbl_Product product = new tbl_Product();
                        tbl_ProductStore store = new tbl_ProductStore();
                        product.ProductName = pvm.ProductName;
                        product.ProductDescription = pvm.ProductDescription;
                        product.ProductPackingDate = pvm.ProductPackingDate;
                        product.ProductExpireDate = pvm.ProductExpireDate;
                        if (pvm.DealsId != null)
                        {
                            product.DealsId = pvm.DealsId;
                        }
                        else
                        {
                            product.DealsId = null;
                        }
                        if (pvm.CategoryId != null)
                        {
                            product.CategoryId = pvm.CategoryId;
                        }
                        else
                        {
                            product.CategoryId = null;
                        }
                        if (pvm.StatusId != null)
                        {
                            product.StatusId = pvm.StatusId;
                        }
                        else
                        {
                            product.StatusId = null;
                        }
                        store.StoreId = pvm.StoreId;
                        store.ProductId = pvm.ProductId;
                        _db.tbl_Product.Add(product);
                        _db.tbl_ProductStore.Add(store);
                        _db.SaveChanges();
                        ViewBag.Message = "Product Created Successfully.";
                        return RedirectToAction("Product");

                    }
                    catch (Exception e)
                    {
                        return View("Error", new HandleErrorInfo(e, "Admin", "CreateProduct"));
                    }
            }            
            return View();            
        }
        public ActionResult DeleteProduct(int id)
        {
            tbl_Product tb = _db.tbl_Product.Where(u => u.ProductId == id).FirstOrDefault();
            ProductViewModel pvm = new ProductViewModel();
            pvm.ProductId = tb.ProductId;
            pvm.ProductName = tb.ProductName;
            pvm.ProductDescription = tb.ProductDescription;
            pvm.ProductPackingDate = tb.ProductPackingDate;
            pvm.ProductExpireDate = tb.ProductExpireDate;
            tbl_Deals td = _db.tbl_Deals.Where(u => u.DealsId == id).FirstOrDefault();
            pvm.DealsName = td.DealsName;
            tbl_ProductCategory tpc = _db.tbl_ProductCategory.Where(u => u.CategoryId == tb.CategoryId).FirstOrDefault();
            pvm.CategoryName = tpc.CategoryName;
            tbl_ProductStore tps = _db.tbl_ProductStore.Where(u => u.ProductId == tb.ProductId).FirstOrDefault();
            tbl_Store ts = _db.tbl_Store.Where(u => u.StoreId == tps.StoreId).FirstOrDefault();
            pvm.StoreName = ts.StoreName;
            return View(pvm);
        }
        [HttpPost, ActionName("DeleteProduct")]
        public ActionResult DeleteProducts(int id)
        {
            tbl_Product tb = _db.tbl_Product.Where(u => u.ProductId == id).FirstOrDefault();
            tbl_ProductStore productstore = _db.tbl_ProductStore.Where(u => u.ProductId == tb.ProductId).FirstOrDefault();
            _db.tbl_Product.Remove(tb);
            if (productstore != null)
            {
                _db.tbl_ProductStore.Remove(productstore);

            }
            _db.SaveChanges();
            return RedirectToAction("Product");
        }
        public ActionResult EditProducts(int id)
        {
            InitCommon();
            //ViewBag.tbl_Role = _db.tbl_Role.ToList();
            tbl_Product tb = _db.tbl_Product.Where(p => p.ProductId == id).FirstOrDefault();
            ProductViewModel pvm = new ProductViewModel();
            pvm.ProductId = tb.ProductId;
            pvm.ProductName = tb.ProductName;
            pvm.ProductDescription = tb.ProductDescription;
            pvm.StatusId = tb.StatusId;
            pvm.CategoryId = tb.CategoryId;
            pvm.DealsId = tb.DealsId;
            var store = _db.tbl_ProductStore.Where(a => a.ProductId == tb.ProductId).FirstOrDefault();
            pvm.StoreId = store.StoreId != null ? store.StoreId.Value : 0;
            pvm.ProductPackingDate = tb.ProductPackingDate;
            pvm.ProductExpireDate = tb.ProductExpireDate;
            return View(pvm);
        }
        [HttpPost, ActionName("EditProducts")]
        public ActionResult EditProduct(ProductViewModel pvm)
        {
            tbl_Product product = _db.tbl_Product.Where(p => p.ProductId == pvm.ProductId).FirstOrDefault();
            tbl_ProductStore store = _db.tbl_ProductStore.Where(p => p.ProductId == pvm.ProductId).FirstOrDefault();
            product.ProductName = pvm.ProductName;
            product.ProductDescription = pvm.ProductDescription;
            product.DealsId = pvm.DealsId;
            product.StatusId = pvm.StatusId;
            product.CategoryId = pvm.CategoryId;
            product.ProductPackingDate = pvm.ProductPackingDate;
            product.ProductExpireDate = pvm.ProductExpireDate;
            store.ProductId = pvm.ProductId;
            store.StoreId = pvm.StoreId;
            _db.SaveChanges();
            ViewBag.Message = "Product Updated Successfully.";
            return RedirectToAction("Product");
        }
        public ActionResult ProductStatus()
        {
            List<StatusViewModel> status = new List<StatusViewModel>();
            var sts = _db.tbl_ProductStatus.ToList();
            foreach (tbl_ProductStatus item in sts)
            {
                status.Add(new StatusViewModel() { StatusId = item.StatusId, StatusName = item.StatusName});
            }
            return View(status);
        }
        public ActionResult InsertProductStatus()
        {
            return View();
        }
        [HttpPost]
        public ActionResult InsertProductStatus(StatusViewModel svm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    tbl_ProductStatus status = new tbl_ProductStatus();
                    status.StatusId = svm.StatusId;
                    status.StatusName = svm.StatusName;
                    _db.tbl_ProductStatus.Add(status);
                    _db.SaveChanges();
                    ViewBag.Message = "Status Created Successfully.";
                    return RedirectToAction("ProductStatus");

                }
                catch (Exception e)
                {
                    return View("Error", new HandleErrorInfo(e, "Admin", "InsertProductStatus"));
                }
            }
            return View();
        }
        public ActionResult DeleteProductStatus(int id)
        {
            tbl_ProductStatus tb = _db.tbl_ProductStatus.Where(u => u.StatusId == id).FirstOrDefault();
            StatusViewModel svm = new StatusViewModel();
            svm.StatusId = tb.StatusId;
            svm.StatusName = tb.StatusName;
            return View(svm);
        }
        [HttpPost, ActionName("DeleteProductStatus")]
        public ActionResult DeleteProductStatuss(int id)
        {
            tbl_ProductStatus tb = _db.tbl_ProductStatus.Where(u => u.StatusId == id).FirstOrDefault();
            _db.tbl_ProductStatus.Remove(tb);
            _db.SaveChanges();
            return RedirectToAction("ProductStatus");
        }
        public ActionResult EditProductStatus(int id)
        {
            tbl_ProductStatus tb = _db.tbl_ProductStatus.Where(p => p.StatusId == id).FirstOrDefault();
            StatusViewModel svm = new StatusViewModel();
            svm.StatusId = tb.StatusId;
            svm.StatusName = tb.StatusName;
            return View(svm);
        }
        [HttpPost, ActionName("EditProductStatus")]
        public ActionResult EditProductStatuss(StatusViewModel svm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    tbl_ProductStatus status = _db.tbl_ProductStatus.Where(p => p.StatusId == svm.StatusId).FirstOrDefault();
                    status.StatusId = svm.StatusId;
                    status.StatusName = svm.StatusName;
                    _db.SaveChanges();
                    ViewBag.Message = "ProductStatus Updated Successfully.";
                    return RedirectToAction("ProductStatus");
                }
                catch (Exception e)
                {
                    return View("Error", new HandleErrorInfo(e, "Admin", "EditProductStatus"));
                }
            }
            return RedirectToAction("ProductStatus");
        }
    }
}