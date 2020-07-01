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
    [Authorize]
    public class UserController : Controller
    {
        Product_TrackingEntities _db = new Product_TrackingEntities();
        // GET: User
        [Authorize]
        public ActionResult Index()
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
                lst.Add(new ProductViewModel() { ProductId = item.ProductId, ProductName = item.ProductName, ProductDescription = item.ProductDescription, StoreName = storename?.StoreName, DealsName = dealsname?.DealsName, CategoryName = categoryname?.CategoryName, StatusName = Statusname?.StatusName, ProductPackingDate = item.ProductPackingDate, ProductExpireDate = item.ProductExpireDate });
            }
            return View(lst);            
        }
        public ActionResult Deals()
        {
            List<Deals> d = new List<Deals>();
            var deals = _db.tbl_Deals.ToList();
            foreach (tbl_Deals item in deals)
            {
                d.Add(new Deals() { DealsId = item.DealsId, DealsName = item.DealsName, DealsDiscription = item.DealsDiscription, DiscountPercent = item.DiscountPercent, StartDate = item.StartDate, EndDate = item.EndDate });
            }
            return View(d);
        }
        public ActionResult Store()
        {
            List<StoreViewModel> store = new List<StoreViewModel>();
            var stores = _db.tbl_Store.ToList();
            foreach (tbl_Store item in stores)
            {
                store.Add(new StoreViewModel() { StoreId = item.StoreId, StoreName = item.StoreName, StoreLocation = item.StoreLocation, StoreCapacity = item.StoreCapacity });
            }
            return View(store);
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }
    }
}