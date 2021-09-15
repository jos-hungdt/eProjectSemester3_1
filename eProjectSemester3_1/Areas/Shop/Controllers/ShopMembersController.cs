using eProjectSemester3_1.Application;
using eProjectSemester3_1.Application.Entities;
using eProjectSemester3_1.Application.Services;
using eProjectSemester3_1.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eProjectSemester3_1.Areas.Shop.Controllers
{
    [Authorize(Roles = AppConstants.ShoppingRoleName)]
    public class ShopMembersController : BaseShopController
    {
        public readonly MembershipRoleService _membershipRoleService;

        public ShopMembersController(MembershipRoleService membershipRoleService,MembershipService membershipSevice, UnitOfWorkManager unitOfWorkManager, LoggingService loggingService, CacheService cacheService, SettingService settingService)
            : base(membershipSevice, unitOfWorkManager, loggingService, cacheService,settingService)
        {
            _membershipRoleService = membershipRoleService;
        }

        // GET: Admin/AdminMembers
        public ActionResult Index()
        {
            return View(LoginUser);
        }

        #region Change info
        public ActionResult ChangeInfo()
        {
            var viewModel = new AdminMembersChangeInfoViewModel
            {
                UserName = LoginUser.UserName,
                //Email = LoginUser.Email,
            };
            
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeInfo(AdminMembersChangeInfoViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Connect sql transaction
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        // Get user
                        var user = MembershipService.GetUser(LoginUser.UserName);

                        //user.Email = viewModel.Email;


                        //unitOfWork.SaveChanges();
                        unitOfWork.Commit();
                        CacheService.ClearStartsWith(CacheKeys.Member.StartsWith);

                        // Show notifications to users
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Change info successfully",
                            MessageType = GenericMessages.warning
                        };
                        
                    }
                    catch (Exception ex)
                    {
                        // Rollback transaction 
                        unitOfWork.Rollback();

                        // Save log massage
                        LoggingService.Error(ex.Message);

                        // Show error message for user
                        ModelState.AddModelError(string.Empty, "Error change info");
                    }
                }
            }

            return View(viewModel);
        }
        #endregion

        #region ChangePassword
        public ActionResult ChangePassword()
        {
            var viewModel = new AdminMembersChangePasswordViewModel
            {
                UserName = LoginUser.UserName,
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(AdminMembersChangePasswordViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Check password is match
                if (viewModel.NewPassword != viewModel.ReNewPassword)
                {
                    ModelState.AddModelError("ReNewPassword", "Retype password does not match");
                    return View(viewModel);
                }

                if (viewModel.Password == viewModel.NewPassword)
                {
                    ModelState.AddModelError("NewPassword", "New password can not be identical with old password");
                    return View(viewModel);
                }

                // Connect sql transaction
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        // Change password for login user
                        var ischange = MembershipService.ChangePassword(LoginUser.UserName, viewModel.Password, viewModel.NewPassword);

                        if (!ischange)
                        {
                            // Rollback transaction 
                            unitOfWork.Rollback();

                            ModelState.AddModelError("Password", "Password Incorrect");
                            return View(viewModel);
                        }

                        //unitOfWork.SaveChanges();
                        unitOfWork.Commit();
                        CacheService.ClearStartsWith(CacheKeys.Member.StartsWith);

                        // Show notifications to users
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Change password successfully",
                            MessageType = GenericMessages.warning
                        };

                        return RedirectToAction("index", "Dashboard");
                    }
                    catch (Exception ex)
                    {
                        // Rollback transaction 
                        unitOfWork.Rollback();

                        // Save log massage
                        LoggingService.Error(ex.Message);

                        // Show error message for user
                        ModelState.AddModelError(string.Empty, "Error change pasword");
                    }
                }
            }

            return View(viewModel);
        }

        #endregion
        
    }
}