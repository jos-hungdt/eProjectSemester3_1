using eProjectSemester3_1.Application;
using eProjectSemester3_1.Application.Services;
using eProjectSemester3_1.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eProjectSemester3_1.Areas.Shop.Controllers
{
    [Authorize(Roles = AppConstants.ShoppingRoleName)]
    public class ShopInfoController : BaseShopController
    {
        public ShopInfoController(MembershipService membershipSevice, UnitOfWorkManager unitOfWorkManager, LoggingService loggingService, CacheService cacheService, SettingService settingService)
            : base(membershipSevice, unitOfWorkManager, loggingService, cacheService, settingService)
        {
        }

        // GET: Shop/ShopInfo
        public ActionResult Index()
        {
            var viewModel = new RegisterShopViewModel
            {
                UserName = LoginUser.UserName,
                FullName = LoginUser.FullName,

                ShopName = LoginUser.ShopName,
                ShopPhone = LoginUser.ShopPhone,
                ShopAddress = LoginUser.ShopAddress,
            };

            return View(viewModel);
        }

        public ActionResult Edit()
        {
            var viewModel = new RegisterShopViewModel
            {
                UserName = LoginUser.UserName,
                FullName = LoginUser.FullName,

                ShopName = LoginUser.ShopName,
                ShopPhone = LoginUser.ShopPhone,
                ShopAddress = LoginUser.ShopAddress,
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RegisterShopViewModel viewModel)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    var user = MembershipService.Get(LoginUser.Id);

                    user.ShopName = viewModel.ShopName;
                    user.ShopPhone = viewModel.ShopPhone;
                    user.ShopAddress = viewModel.ShopAddress;

                    unitOfWork.SaveChanges();

                    unitOfWork.Commit();
                    CacheService.ClearStartsWith(CacheKeys.Member.StartsWith);
                }
                catch (Exception ex)
                {
                    // Rollback transaction 
                    unitOfWork.Rollback();

                    // Save log massage
                    LoggingService.Error(ex.Message);

                    // Show error message for user
                    ModelState.AddModelError(string.Empty, "Error edit shop");
                }
            }
            return View(viewModel);
        }
    }
}