using eProjectSemester3_1.Application;
using eProjectSemester3_1.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using eProjectSemester3_1.Application.Context;
using System.Net;
using System.Data.Entity;
using eProjectSemester3_1.Application.Entities;

namespace eProjectSemester3_1.Controllers
{
    public class HomeController : BaseController
    {
        public AppDbContext db;
        public HomeController(AppDbContext context,MembershipService membershipSevice, UnitOfWorkManager unitOfWorkManager, LoggingService loggingService)
            : base(membershipSevice, unitOfWorkManager, loggingService)
        {
            db = context;
        }
        
        public ActionResult Index(int? page)
        {
            var tbl_Estate = db.Estate.Include(x => x.EstateType); 
            int pageSize = 8;
            int pagenumber = (page ?? 1);
            ViewBag.Title = "Home page";

            return View(tbl_Estate.OrderBy(i => i.realEstateTitle).ToPagedList(pagenumber, pageSize));
        }
        public ActionResult SellerEstate(int? id, int? page)
        {
            var tbl_Estate = db.Estate.Where(t => t.User.Id == id);
            int pageSize = 8;
            int pagenumber = (page ?? 1);
            ViewBag.Title = "Home page";

            return View(tbl_Estate.OrderBy(i => i.realEstateTitle).ToPagedList(pagenumber, pageSize));
        }
        public ActionResult Search(AppHelpers.SearchModel a, int? page)
        {
            var estate = new AppHelpers.EstateSearch();
            var model = estate.GetResult(a);
            ViewBag.Title = "Search Result";
            //return View(model);
            int pageSize = 8;
            int pagenumber = (page ?? 1);

            return View(model.OrderBy(i => i.realEstateTitle).ToPagedList(pagenumber, pageSize));
        }
        public ActionResult NewsSearch(string n, int? page)
        {

            ViewBag.Title = "Search Result " + n;
            ViewBag.Keyword = n;
            int pageSize = 8;
            int pagenumber = (page ?? 1);
            var q = from i in db.News
                    where i.Title.Contains(n) || i.ShortContent.Contains(n)
                    select i;
            if (q != null)
                return View(q.OrderBy(i => i.Id).ToPagedList(pagenumber, pageSize));
            return View();
        }
        
        public ActionResult FAQs(int? id, int? page)
        {
            ViewBag.Title = "Frequently Asked Questions";
            var faq = from i in db.Faqs
                      select i;
            int pageSize = 4;
            int pagenumber = (page ?? 1);
            return View(faq.OrderBy(t => t.faqID).ToPagedList(pagenumber, pageSize));
        }
        public ActionResult Seller(int? page)
        {
            ViewBag.Title = "Seller List";
            var roleService = ServiceFactory.Get<AppDbContext>();

            var a = roleService.MembershipRole.Include(x => x.Users).FirstOrDefault(x => x.RoleName == AppConstants.ShoppingRoleName);
            int pageSize = 4;
            int pagenumber = (page ?? 1);
            return View(a.Users.OrderBy(i => i.Id).ToPagedList(pagenumber, pageSize));
        }
        // GET: Home/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Application.Entities.Estate tbl_Estate = db.Estate.Find(id);
            if (tbl_Estate == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Estate);
        }

    }
}
