using eProjectSemester3_1.Application;
using eProjectSemester3_1.Application.Entities;
using eProjectSemester3_1.Application.Services;
using eProjectSemester3_1.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eProjectSemester3_1.Controllers
{
    public class ContactController : BaseController
    {
        public readonly ContactService _contactService;
        public ContactController(ContactService contactService, MembershipService membershipSevice, UnitOfWorkManager unitOfWorkManager, LoggingService loggingService)
            : base(membershipSevice, unitOfWorkManager, loggingService)
        {
            _contactService = contactService;
        }

        // GET: Contact
        public ActionResult Index()
        {
            var model = new ContactViewModel
            {

            };
            @ViewBag.Title = "Contact US";

            return View("Contact", model);
        }

        [HttpPost]
        public ActionResult Index(ContactViewModel model)
        {
            DateTime? OldContact = (DateTime?)Session["OldContactTime"];
            if(OldContact != null)
            {
                if (OldContact > DateTime.Now)
                {
                    return View("WarningSpam");
                }
            }
            
            
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    var contact = new Contact
                    {
                        Name = model.Name,
                        Phone = model.Phone,
                        Email = model.Email,
                        Address = model.Address,
                        Message = model.Message
                    };

                    _contactService.Add(contact);
                    
                    unitOfWork.Commit();
                    Session["OldContactTime"] = DateTime.Now.AddMinutes(5);

                    return View("Complete");
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    LoggingService.Error(ex.Message);
                    ModelState.AddModelError(string.Empty, "Update Contact Error");
                }
            }
            return View("Contact", model);
        }
    }
}