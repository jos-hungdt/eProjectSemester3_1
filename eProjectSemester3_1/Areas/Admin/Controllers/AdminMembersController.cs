using eProjectSemester3_1.Application;
using eProjectSemester3_1.Application.Context;
using eProjectSemester3_1.Application.Entities;
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
    public class AdminMembersController : BaseAdminController
    {
        public readonly MembershipRoleService _membershipRoleService;

        public AdminMembersController(MembershipRoleService membershipRoleService,MembershipService membershipSevice, UnitOfWorkManager unitOfWorkManager, LoggingService loggingService, CacheService cacheService, SettingService settingService)
            : base(membershipSevice, unitOfWorkManager, loggingService, cacheService,settingService)
        {
            _membershipRoleService = membershipRoleService;
        }

        // GET: Admin/AdminMembers
        public ActionResult Index(int? p)
        {
            int count = MembershipService.MemberCount();

            var Paging = CalcPaging(AppConstants.AdminPageSize, p, count);

            var model = new AdminMembersViewModel
            {
                Paging = Paging,
                ListUsers = MembershipService.GetList(Paging.Page, AppConstants.AdminPageSize)
            };
            return View(model);
        }

        #region Edit
        private bool IsRoleInMember(string role,MembershipUser member)
        {
            foreach(var it in member.Roles)
            {
                if (it.RoleName == role) return true;
            }

            return false;
        }

        public ActionResult Edit(string id) // id = user
        {
            // if user equa login user then return
            if (id == LoginUser.UserName) return RedirectToAction("Index");
            //Check super user
            if (id == AppConstants.AdminUserName) return RedirectToAction("Index");

            // get user
            var user = MembershipService.GetUser(id);
            if (user == null) return RedirectToAction("Index"); // Not find user return

            if(LoginUser.UserName != AppConstants.AdminUserName) // check not super accout
            {
                if(IsRoleInMember(AppConstants.AdminRoleName, user) ) // Check accout edit admin role
                {
                    // Show notifications to users
                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = "You do not have sufficient permissions to perform this operation",
                        MessageType = GenericMessages.warning
                    };

                    return RedirectToAction("Index");
                }
            }


            var viewModel = new AdminMembersChangeInfoViewModel
            {
                UserName = user.UserName,
                //Email = LoginUser.Email,
                AllRoles = _membershipRoleService.GetAll(),
                RolesId = new List<int>(),
            };

            foreach (var it in user.Roles)
            {
                viewModel.RolesId.Add(it.Id);
            }
            
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AdminMembersChangeInfoViewModel viewModel)
        {
            // if user equa login user then return
            if (viewModel.UserName == LoginUser.UserName) return RedirectToAction("Index");

            //Check super user
            if (viewModel.UserName == AppConstants.AdminUserName) return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                // Connect sql transaction
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        // Get user
                        var user = MembershipService.GetUser(viewModel.UserName);
                        if (user == null) return RedirectToAction("Index"); // Not find user return

                        if (LoginUser.UserName != AppConstants.AdminUserName) // check not super accout
                        {
                            if (IsRoleInMember(AppConstants.AdminRoleName, user)) // Check accout edit admin role
                            {
                                // Show notifications to users
                                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                                {
                                    Message = "You do not have sufficient permissions to perform this operation",
                                    MessageType = GenericMessages.warning
                                };

                                return RedirectToAction("Index");
                            }
                        }

                        //user.Email = viewModel.Email;

                        //unitOfWork.SaveChanges();

                        //Update Role
                        user.Roles.Clear();
                        if(viewModel.RolesId != null)
                        {
                            foreach (var it in viewModel.RolesId)
                            {
                                var rl = _membershipRoleService.Get(it);
                                if(LoginUser.UserName != AppConstants.AdminUserName) // Check not super user
                                {
                                    if (rl.RoleName == AppConstants.AdminRoleName) continue;
                                }
                                user.Roles.Add(rl);
                            }
                        }
                        unitOfWork.SaveChanges();

                        
                        unitOfWork.Commit();
                        CacheService.ClearStartsWith(CacheKeys.Member.StartsWith); // clear cache MembershipService

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

            viewModel.AllRoles = _membershipRoleService.GetAll();
            return View(viewModel);
        }
        #endregion

        #region NewPassword
        public ActionResult NewPassword(string id) // id = user
        {
            // if user equa login user then return
            if (id == LoginUser.UserName) return RedirectToAction("Index");

            //Check super user
            if (id == AppConstants.AdminUserName) return RedirectToAction("Index");

            // get user
            var user = MembershipService.GetUser(id);
            if (user == null) return RedirectToAction("Index"); // Not find user return

            var viewModel = new AdminMembersNewPasswordViewModel
            {
                UserName = user.UserName,
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewPassword(AdminMembersNewPasswordViewModel viewModel)
        {
            // if user equa login user then return
            if (viewModel.UserName == LoginUser.UserName) return RedirectToAction("Index");

            //Check super user
            if (viewModel.UserName == AppConstants.AdminUserName) return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                // Connect sql transaction
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        // Get user
                        var user = MembershipService.GetUser(viewModel.UserName);
                        if (user == null) return RedirectToAction("Index"); // Not find user return

                        // Check password is match
                        if (viewModel.NewPassword != viewModel.ReNewPassword)
                        {
                            ModelState.AddModelError("ReNewPassword", "Retype password does not match");
                            return View(viewModel);
                        }

                        var salt = user.PasswordSalt;
                        var hash = StringUtils.GenerateSaltedHash(viewModel.NewPassword, salt);

                        user.Password = hash;

                        
                        //unitOfWork.SaveChanges();
                        unitOfWork.Commit();
                        CacheService.ClearStartsWith(CacheKeys.Member.StartsWith);

                        // Show notifications to users
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Change password successfully",
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
                        ModelState.AddModelError(string.Empty, "Error change pasword");
                    }
                }
            }

            return View(viewModel);
        }

        #endregion

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

        #region Create Account
        // GET: Admin/AdminMembers/Create
        public ActionResult Create()
        {
            var viewModel = new AdminMembersCreateViewModel {
                AllRoles = _membershipRoleService.GetAll()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AdminMembersCreateViewModel userModel)
        {
            // Connect sql transaction
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                // Check password is match
                if (userModel.Password != userModel.RePassword)
                {
                    ModelState.AddModelError("RePassword", "Retype password does not match");
                    return View(userModel);
                }

                try
                {
                    var member = MembershipService.Register(userModel.UserName, userModel.Password);

                    //Update Role
                    if (member.Roles == null) member.Roles = new List<MembershipRole>();
                    if (userModel.RolesId != null)
                    {
                        foreach (var it in userModel.RolesId)
                        {
                            var rl = _membershipRoleService.Get(it);
                            if (LoginUser.UserName != AppConstants.AdminUserName) // Check not super user
                            {
                                if (rl.RoleName == AppConstants.AdminRoleName) continue;
                            }
                            member.Roles.Add(rl);
                        }
                    }
                    MembershipService.Add(member);
                    unitOfWork.Commit();
                    CacheService.ClearStartsWith(CacheKeys.Member.StartsWith); //Clear MembershipService cache

                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = "Create User success",
                        MessageType = GenericMessages.success
                    };
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Rollback transaction 
                    unitOfWork.Rollback();
                    
                    // Save log massage
                    LoggingService.Error(ex.Message);

                    // Show error message for user
                    ModelState.AddModelError(string.Empty, "Error Create account");
                }
            }

            return View(userModel);
        }
        #endregion

        #region Delete Account
        public ActionResult Delete(string id) // id = user
        {
            // if user equa login user then return
            if (id == LoginUser.UserName) return RedirectToAction("Index");
            //Check super user
            if (id == AppConstants.AdminUserName) return RedirectToAction("Index");

            // get user
            var user = MembershipService.GetUser(id);
            if (user == null) return RedirectToAction("Index"); // Not find user return

            if (LoginUser.UserName != AppConstants.AdminUserName) // check not super accout
            {
                if (IsRoleInMember(AppConstants.AdminRoleName, user)) // Check accout edit admin role
                {
                    // Show notifications to users
                    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                    {
                        Message = "You do not have sufficient permissions to perform this operation",
                        MessageType = GenericMessages.warning
                    };

                    return RedirectToAction("Index");
                }
            }


            var viewModel = new AdminMembersDeleteViewModel
            {
                UserName = user.UserName
            };
            return View(viewModel);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(AdminMembersDeleteViewModel viewModel)
        {
            // if user equa login user then return
            if (viewModel.UserName == LoginUser.UserName) return RedirectToAction("Index");

            //Check super user
            if (viewModel.UserName == AppConstants.AdminUserName) return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                // Connect sql transaction
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {

                       // AppDbContext db = new ServiceFactory.Get<MembershipRole>();
                        // Get user
                        var user = MembershipService.GetUser(viewModel.UserName);
                        if (user == null) return RedirectToAction("Index"); // Not find user return

                        if (LoginUser.UserName != AppConstants.AdminUserName) // check not super accout
                        {
                            if (IsRoleInMember(AppConstants.AdminRoleName, user)) // Check accout edit admin role
                            {
                                // Show notifications to users
                                TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                                {
                                    Message = "You do not have sufficient permissions to perform this operation",
                                    MessageType = GenericMessages.warning
                                };

                                return RedirectToAction("Index");
                            }
                        }
                        user.Roles.Clear();
                        //_membershipRoleService.Delete(user);
                        unitOfWork.SaveChanges();
                      //   db.MembershipRole.Remove(user);

                        unitOfWork.Commit();
                        CacheService.ClearStartsWith(CacheKeys.Member.StartsWith); // clear cache MembershipService

                        // Show notifications to users
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Delete successfully",
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
                        ModelState.AddModelError(string.Empty, "Error Delete user");
                    }
                }
            }
            
            return View(viewModel);
        }
        #endregion
    }
}