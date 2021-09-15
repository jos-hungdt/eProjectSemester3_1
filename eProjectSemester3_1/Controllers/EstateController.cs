using eProjectSemester3_1.Application;
using eProjectSemester3_1.Application.Context;
using eProjectSemester3_1.Application.Services;
using eProjectSemester3_1.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using eProjectSemester3_1.Application.Entities;

namespace eProjectSemester3_1.Controllers
{
    public class EstateController : BaseController
    {
        private readonly AppDbContext db;
        public EstateController(AppDbContext appDbContext ,MembershipService membershipSevice, UnitOfWorkManager unitOfWorkManager, LoggingService loggingService)
            : base(membershipSevice, unitOfWorkManager, loggingService)
        {
            db = appDbContext;
        }

        // GET: Estate
        public ActionResult Index(int? id)
        {
            //var db1 = ServiceFactory.Get<AppDbContext>();
            //AppDbContext db2 = new AppDbContext();
            //AppDbContext db = new 

            var Estate = db.Estate.Find(id);
            if (Estate == null)
            {
                return HttpNotFound();
            }
            ViewBag.Title = Estate.realEstateTitle;
            return View(Estate);
        }
       // public AppDbContext db = new AppDbContext();
        
        // GET: Estate/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
        public ActionResult PROPERTIES(int? id,int? page)
        {
            var tbl_Estate = db.Estate;
            int pageSize = 8;
            int pagenumber = (page ?? 1);
            ViewBag.Title = "Properties List";

            return View(tbl_Estate.OrderBy(i => i.realEstateTitle).ToPagedList(pagenumber, pageSize));
        }
        // GET: Estate/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Estate/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Estate/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Estate/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Estate/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Estate/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}