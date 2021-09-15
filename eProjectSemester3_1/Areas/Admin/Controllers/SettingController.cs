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
    public class SettingController : BaseAdminController
    { 
        public SettingController(MembershipService membershipSevice, UnitOfWorkManager unitOfWorkManager, LoggingService loggingService, CacheService cacheService, SettingService settingService)
            : base(membershipSevice, unitOfWorkManager, loggingService, cacheService, settingService)
        {
        }

        // GET: Admin/Setting
        public ActionResult Index()
        {
            var viewModel = new SettingViewModel
            {
                CompanyName = SettingService.GetSetting("CompanyName"),
                Email = SettingService.GetSetting("Email"),
                Hotline = SettingService.GetSetting("Hotline"),
                Address = SettingService.GetSetting("Address"),
                Description = SettingService.GetSetting("Description"),
            };


            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(SettingViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        SettingService.SetSetting("CompanyName", viewModel.CompanyName);
                        SettingService.SetSetting("Email", viewModel.Email);
                        SettingService.SetSetting("Hotline", viewModel.Hotline);
                        SettingService.SetSetting("Address",viewModel.Address);
                        SettingService.SetSetting("Description", viewModel.Description);

                        unitOfWork.Commit();
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Update setting successfully",
                            MessageType = GenericMessages.success
                        };
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex.Message);
                        ModelState.AddModelError(string.Empty, "Update setting Error");
                    }
                }
            }

            return View(viewModel);
        }

    }
}