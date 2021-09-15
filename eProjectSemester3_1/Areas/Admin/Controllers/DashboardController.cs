using eProjectSemester3_1.Application;
using eProjectSemester3_1.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eProjectSemester3_1.Areas.Admin.Controllers
{
    [Authorize(Roles = AppConstants.AdminRoleName)]
    public class DashboardController : BaseAdminController
    {
        public DashboardController(MembershipService membershipSevice, UnitOfWorkManager unitOfWorkManager, LoggingService loggingService,CacheService cacheService,SettingService settingService)
            : base(membershipSevice, unitOfWorkManager, loggingService, cacheService,settingService)
        {
        }

        // GET: Admin/Dashboard
        public ActionResult Index()
        {
            return View();
        }
    }
}