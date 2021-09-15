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
    public class AdminFAQsController : BaseAdminController
    {
        public readonly FaqsService _faqsService;
        public AdminFAQsController(FaqsService faqsService,MembershipService membershipSevice, UnitOfWorkManager unitOfWorkManager, LoggingService loggingService, CacheService cacheService, SettingService settingService)
            : base(membershipSevice, unitOfWorkManager, loggingService, cacheService, settingService)
        {
            _faqsService = faqsService;
        }

        // GET: Admin/AdminFAQs
        public ActionResult Index(int? p)
        {
            int count = _faqsService.FaqsCount();

            var Paging = CalcPaging(AppConstants.AdminPageSize, p, count);

            var model = new AdminFaqsViewModel
            {
                Paging = Paging,
                ListFaqs = _faqsService.GetList(Paging.Page, AppConstants.AdminPageSize)
            };
            return View(model);
        }


        #region Create faqs
        [Authorize]
        public ActionResult Create()
        {

            var viewModel = new AdminFaqsEditViewModel
            {
                
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AdminFaqsEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var faqs = new Faqs
                        {
                            question = viewModel.question,
                            answer = viewModel.answer,
                          };

                        _faqsService.Add(faqs);
                        
                        unitOfWork.Commit();
                        CacheService.ClearStartsWith(CacheKeys.Faqs.StartsWith);
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Create Faqs success",
                            MessageType = GenericMessages.success
                        };
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex.Message);
                        ModelState.AddModelError(string.Empty, "Create Faqs Error");
                    }
                }
            }

            return View(viewModel);
        }

        #endregion

        #region Edit News
        public ActionResult Edit(int id)
        {
            var faqs = _faqsService.Get(id);
            if (faqs == null) return RedirectToAction("Index");
            
            var viewModel = new AdminFaqsEditViewModel
            {
                Id = faqs.faqID,
                question = faqs.question,
                answer = faqs.answer,
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AdminFaqsEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var faqs = _faqsService.Get(viewModel.Id,false);
                        if (faqs == null) return HttpNotFound();// RedirectToAction("Index");

                        faqs.question = viewModel.question;
                        faqs.answer = viewModel.answer;
                        

                        unitOfWork.Commit();
                        CacheService.ClearStartsWith(CacheKeys.Faqs.StartsWith);
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Update faqs success",
                            MessageType = GenericMessages.success
                        };
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex.Message);
                        ModelState.AddModelError(string.Empty, "Edit faqs Error");
                    }
                }
            }
            
            return View(viewModel);
        }
        #endregion

        #region Delete
        public ActionResult Delete(int id)
        {
            var faqs = _faqsService.Get(id);
            if (faqs == null) return RedirectToAction("Index");
            var viewModel = new AdminFaqsEditViewModel
            {
                Id = faqs.faqID,
                question = faqs.question,
                answer = faqs.answer,
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(AdminFaqsEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        AppDbContext db = ServiceFactory.Get<AppDbContext>();

                        var faqs = _faqsService.Get(viewModel.Id, false);
                        if (faqs == null) return HttpNotFound();// RedirectToAction("Index");

                        _faqsService.Repmove(faqs);

                        unitOfWork.Commit();
                        CacheService.ClearStartsWith(CacheKeys.Faqs.StartsWith);
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Delete faqs success",
                            MessageType = GenericMessages.success
                        };
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex.Message);
                        ModelState.AddModelError(string.Empty, "Delete faqs Error");
                    }
                }
            }

            return View(viewModel);
        }
        #endregion
    }
}