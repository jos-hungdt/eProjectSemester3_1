using eProjectSemester3_1.Application;
using eProjectSemester3_1.Application.Context;
using eProjectSemester3_1.Application.Entities;
using eProjectSemester3_1.Application.Services;
using eProjectSemester3_1.Areas.Admin.ViewModels;
using eProjectSemester3_1.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eProjectSemester3_1.Controllers
{
    public class MembersController : BaseController
    {
        public readonly CacheService _cacheService;
        public readonly MembershipRoleService _membershipRoleService;
        public MembersController(MembershipRoleService membershipRoleService,CacheService cacheService,MembershipService membershipSevice, UnitOfWorkManager unitOfWorkManager, LoggingService loggingService)
            : base(membershipSevice, unitOfWorkManager, loggingService)
        {
            _cacheService = cacheService;
            _membershipRoleService = membershipRoleService;
        }

        [Authorize]
        public ActionResult Index()
        {
            var model = MembershipService.GetUser(User.Identity.Name);

            return View(model);
        }

        [HttpPost]
        public ActionResult CheckUser(string username)
        {
            var user = MembershipService.GetUser(username);
            if(user != null)
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }

        #region Login & Logout
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (UserIsAuthenticated) return RedirectToAction("index","Home");

            LogOnViewModel viewModel = new LogOnViewModel();

            var returnUrl = Request["ReturnUrl"];
            if (!string.IsNullOrEmpty(returnUrl))
            {
                viewModel.ReturnUrl = returnUrl;
            }

            return PartialView(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Login(LogOnViewModel model)
        {
            if (UserIsAuthenticated) return RedirectToAction("index", "Home");

            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var username = model.UserName;
                var password = model.Password;

                try
                {
                    if (ModelState.IsValid)
                    {
                        if (MembershipService.ValidateUser(username, password, System.Web.Security.Membership.MaxInvalidPasswordAttempts))
                        {
                            var user = MembershipService.GetUser(username);
                            if (user.IsApproved && !user.IsLockedOut && !user.IsBanned)
                            {
                                // Set last login date
                                System.Web.Security.FormsAuthentication.SetAuthCookie(username, model.RememberMe);
                                user.LastLoginDate = DateTime.UtcNow;

                                // Redirect old page
                                if (Url.IsLocalUrl(model.ReturnUrl) && model.ReturnUrl.Length > 1 && model.ReturnUrl.StartsWith("/")
                                    && !model.ReturnUrl.StartsWith("//") && !model.ReturnUrl.StartsWith("/\\"))
                                {
                                    return Redirect(model.ReturnUrl);
                                }

                                return RedirectToAction("Index", "Home", new { area = string.Empty });
                            }
                        }
                        else
                        {
                            // get here Login failed, check the login status
                            var loginStatus = MembershipService.LastLoginStatus;

                            switch (loginStatus)
                            {
                                case LoginAttemptStatus.UserNotFound:
                                case LoginAttemptStatus.PasswordIncorrect:
                                    ModelState.AddModelError(string.Empty, "Password Incorrect");
                                    break;

                                case LoginAttemptStatus.PasswordAttemptsExceeded:
                                    ModelState.AddModelError(string.Empty, "Password Attempts Exceeded");
                                    break;

                                case LoginAttemptStatus.UserLockedOut:
                                    ModelState.AddModelError(string.Empty, "User Locked Out");
                                    break;

                                case LoginAttemptStatus.Banned:
                                    ModelState.AddModelError(string.Empty, "NowBanned");
                                    break;

                                case LoginAttemptStatus.UserNotApproved:
                                    ModelState.AddModelError(string.Empty, "User Not Approved");
                                    //user = MembershipService.GetUser(username);
                                    //SendEmailConfirmationEmail(user);
                                    break;

                                default:
                                    ModelState.AddModelError(string.Empty, "Logon Generic");
                                    break;
                            }
                        }
                    }
                }
                //catch
                //{
                //    LoggingService.Error(ex);
                //}
                finally
                {
                    try
                    {
                        unitOfWork.Commit();
                    }
                    catch
                    {
                        unitOfWork.Rollback();
                        //LoggingService.Error(ex);
                    }

                }

                return View(model);
            }
        }


        [AllowAnonymous]
        public ActionResult LogOut()
        {
            System.Web.Security.FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home", new { area = string.Empty });
        }

        #endregion

        #region Regiter Shop
        public ActionResult RegisterShop()
        {
            if(IsRole(AppConstants.ShoppingRoleName)) return RedirectToAction("Index", "Home", new { area = string.Empty });

            var viewModel = new RegisterShopViewModel
            {
                UserName = LoginUser.UserName,
                FullName = LoginUser.FullName,
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterShop(RegisterShopViewModel viewModel)
        {
            if (IsRole(AppConstants.ShoppingRoleName)) return RedirectToAction("Index", "ShopInfo", new { area = "Shop" });

            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    var user = MembershipService.Get(LoginUser.Id);

                    user.ShopName = viewModel.ShopName;
                    user.ShopPhone = viewModel.ShopPhone;
                    user.ShopAddress = viewModel.ShopAddress;

                    var role = _membershipRoleService.GetByRole(AppConstants.ShoppingRoleName);
                    if (user.Roles == null) user.Roles = new List<MembershipRole>();

                    user.Roles.Add(role);
                    
                    unitOfWork.SaveChanges();

                    unitOfWork.Commit();
                    _cacheService.ClearStartsWith(CacheKeys.Member.StartsWith);

                    return RedirectToAction("Index", "ShopInfo", new { area = "Shop" });
                }
                catch(Exception ex)
                {
                    // Rollback transaction 
                    unitOfWork.Rollback();
                    
                    // Save log massage
                    LoggingService.Error(ex.Message);

                    // Show error message for user
                    ModelState.AddModelError(string.Empty, "Error writing shop");
                }
            }
            return View(viewModel);
        }
        #endregion

        #region Register

        /// <summary>
        /// Add a new user
        /// </summary>
        /// <returns></returns>
        public ActionResult Register()
        {
            if(LoginUser != null) RedirectToAction("Index", "Home");

            var viewModel = new MemberAddViewModel();

            // See if a return url is present or not and add it
            var returnUrl = Request["ReturnUrl"];
            if (!string.IsNullOrEmpty(returnUrl))
            {
                viewModel.ReturnUrl = returnUrl;
            }

            return View(viewModel);
        }

        /// <summary>
        /// Add a new user
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(MemberAddViewModel userModel)
        {
            if (LoginUser != null) RedirectToAction("Index", "Home");

            // Connect sql transaction
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                // Check password is match
                if (userModel.Password != userModel.RePassword)
                {
                    ModelState.AddModelError("RePassword", "Password does not match");
                    return View(userModel);
                }
                
                try
                {
                    var member = MembershipService.Register(userModel.UserName, userModel.Password);

                    member.Email = userModel.Email;
                    member.FullName = userModel.FullName;


                    MembershipService.Add(member);
                    //unitOfWork.SaveChanges();

                    unitOfWork.Commit();
                    System.Web.Security.FormsAuthentication.SetAuthCookie(member.UserName, false);


                    return RedirectToAction("Index", "Members");
                }
                catch(Exception ex)
                {
                    // Rollback transaction 
                    unitOfWork.Rollback();

                    // Logout 
                    System.Web.Security.FormsAuthentication.SignOut();
                    
                    // Save log massage
                    LoggingService.Error(ex.Message);

                    // Show error message for user
                    //ModelState.AddModelError(string.Empty, "Error writing account");
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            
            return View(userModel);
        }

        #endregion

        #region Change

        #region Change info
        [Authorize]
        public ActionResult ChangeInfo()
        {
            var viewModel = new AdminMembersChangeInfoViewModel
            {
                UserName = LoginUser.UserName,
                //Email = LoginUser.Email,
            };

            return View(viewModel);
        }

        [Authorize]
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
                        _cacheService.ClearStartsWith(CacheKeys.Member.StartsWith);

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
        [Authorize]
        public ActionResult ChangePassword()
        {
            var viewModel = new AdminMembersChangePasswordViewModel
            {
                UserName = LoginUser.UserName,
            };

            return View(viewModel);
        }

        [Authorize]
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
                        _cacheService.ClearStartsWith(CacheKeys.Member.StartsWith);

                        // Show notifications to users
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Change password successfully",
                            MessageType = GenericMessages.warning
                        };

                        return RedirectToAction("index", "Home");
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
        #endregion

        [Authorize]
        [ChildActionOnly]
        public PartialViewResult SidePanel()
        {
            return PartialView(LoginUser);
        }

    }
}