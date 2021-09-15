using eProjectSemester3_1.Application;
using eProjectSemester3_1.Application.Services;
using eProjectSemester3_1.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eProjectSemester3_1.Areas.Admin.Controllers
{
    
    [Authorize(Roles = AppConstants.AdminRoleName)]
    public class AdminContactController : BaseAdminController
    {
        
        public readonly ContactService _contactService;
        public AdminContactController(ContactService contactService,MembershipService membershipSevice, UnitOfWorkManager unitOfWorkManager, LoggingService loggingService, CacheService cacheService, SettingService settingService)
            : base(membershipSevice, unitOfWorkManager, loggingService, cacheService, settingService)
        {
            _contactService = contactService;
        }

        // GET: Admin/AdminContact
        public ActionResult Index(int? p)
        {
            int count = _contactService.ContactCount();

            var Paging = CalcPaging(AppConstants.AdminPageSize, p, count);

            var model = new AdminContactViewModel
            {
                Paging = Paging,
                ListContact = _contactService.GetList(Paging.Page, AppConstants.AdminPageSize)
            };
            return View(model);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null) return RedirectToAction("Index");

            var contact = _contactService.Get((int)id);
            if (contact == null) return RedirectToAction("Index");

            

            return View();
        }

        public ActionResult Edit(int? id)
        {
            if(id == null) return RedirectToAction("Index");

            var contact = _contactService.Get((int)id);
            if (contact == null) return RedirectToAction("Index");

            var viewModel = new AdminEditContactViewModel
            {
                Id = contact.Id,
                Name = contact.Name,
                Phone = contact.Phone,
                Email = contact.Email,
                Message = contact.Message,
                Address = contact.Address,
                Status = contact.Status,
                Note = contact.Note,
                CreateDate = contact.CreateDate
            };
            
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(AdminEditContactViewModel viewModel)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    var contact = _contactService.Get(viewModel.Id, false);
                    if (contact == null) return RedirectToAction("Index");
                    
                    contact.Status = viewModel.Status;
                    contact.Note = viewModel.Note;

                    unitOfWork.SaveChanges();
                    unitOfWork.Commit();
                    CacheService.ClearStartsWith(CacheKeys.Contact.StartsWith);
                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = "Update Contact success",
                        MessageType = GenericMessages.success
                    };
                }
                catch(Exception ex)
                {
                    unitOfWork.Rollback();
                    LoggingService.Error(ex.Message);
                    ModelState.AddModelError(string.Empty, "Update Contact Error");
                }
            }

            return View(viewModel);
        }
    }
}