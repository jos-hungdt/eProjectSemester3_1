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
    public class AdminCategoryController : BaseAdminController
    {
        public readonly CategoryService _categoryService;
        public readonly PostService _postService;

        public AdminCategoryController(CategoryService categoryService,MembershipService membershipSevice, UnitOfWorkManager unitOfWorkManager, LoggingService loggingService, CacheService cacheService, SettingService settingService)
            : base(membershipSevice, unitOfWorkManager, loggingService, cacheService,settingService)
        {
            _categoryService = categoryService;
        }

        // GET: Admin/AdminCatergory
        public ActionResult Index(int? p)
        {
            int count = _categoryService.CategoryCount();

            var Paging = CalcPaging(AppConstants.AdminPageSize, p, count);

            var model = new AdminCategoryViewModel
            {
                Paging = Paging,
                ListUsers = _categoryService.GetList(Paging.Page, AppConstants.AdminPageSize)
            };
            return View(model);
        }


        #region Create news
        [Authorize]
        public ActionResult Create()
        {

            var viewModel = new AdminCategoryEditViewModel
            {
                SortOrder = _categoryService.CategoryCount(),
                AllCategories = _categoryService.GetBaseSelectListCategories(_categoryService.GetAll()),
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AdminCategoryEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var cat = new Category
                        {
                            Name = viewModel.Name,
                            CreateDate = DateTime.Now,
                            Description = viewModel.Description,
                            SortOrder = viewModel.SortOrder,
                            Image = viewModel.Image,
                        };

                        if(viewModel.ParentCategory != null)
                        {
                            cat.ParentCategory = _categoryService.Get((int)viewModel.ParentCategory);
                        }

                        _categoryService.Add(cat);
                        //unitOfWork.SaveChanges();

                        unitOfWork.Commit();
                        CacheService.ClearStartsWith(CacheKeys.Category.StartsWith);
                        // Show notifications to users
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Category create successfully",
                            MessageType = GenericMessages.success
                        };

                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex.Message);
                        ModelState.AddModelError(string.Empty, "Create Category Error");
                    }
                }
            }

            viewModel.AllCategories = _categoryService.GetBaseSelectListCategories(_categoryService.GetAll());
            return View(viewModel);
        }

        #endregion

        #region Edit News
        public ActionResult Edit(int id)
        {
            var cat = _categoryService.Get(id);
            if (cat == null) return RedirectToAction("Index");

            var viewModel = new AdminCategoryEditViewModel
            {
                Id = cat.Id,
                Name = cat.Name,
                Description = cat.Description,
                Image = cat.Image,
                AllCategories = _categoryService.GetBaseSelectListCategories(_categoryService.GetAll().Where(x => x.Id != cat.Id)
                    .ToList()),
            };
            if(cat.ParentCategory != null)
            {
                viewModel.ParentCategory = cat.ParentCategory.Id;
            }
            

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AdminCategoryEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var cat = _categoryService.Get(viewModel.Id);
                        if (cat == null) return RedirectToAction("Index"); //HttpNotFound();

                        cat.Name = viewModel.Name;
                        cat.Description = viewModel.Description;
                        cat.Image = viewModel.Image;
                        if (viewModel.ParentCategory != null)
                        {
                            cat.ParentCategory = _categoryService.Get((int)viewModel.ParentCategory);
                        }
                        else
                        {
                            cat.ParentCategory = null;
                        }

                        unitOfWork.SaveChanges();

                        unitOfWork.Commit();
                        CacheService.ClearStartsWith(CacheKeys.Category.StartsWith);
                        // Show notifications to users
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Category edit successfully",
                            MessageType = GenericMessages.success
                        };

                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex.Message);
                        ModelState.AddModelError(string.Empty, "Edit Category Error");
                    }
                }
            }

            return View(viewModel);
        }
        #endregion
        #region Delete Category
        public ActionResult Delete(int id)
        {
            var cat = _categoryService.Get(id);
            if (cat == null) return RedirectToAction("Index");

            var viewModel = new AdminCategoryDeleteViewModel
            {
                Id = cat.Id,
                Name = cat.Name
            };
           
            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(AdminCategoryDeleteViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        AppDbContext db = ServiceFactory.Get<AppDbContext>();
                        var cat = _categoryService.Get(viewModel.Id);
                        //// lỗi nằm ở chỗ này. tại sao nó không cho lấy cơ chứ T_T
                        //News news = db.News.Where(d => d.Category.Equals(viewModel.Id)).FirstOrDefault(); //tại đây ko cho phép get ra
                        ////Post post = db.Post.Where(d=>d.News == )
                        if (cat == null) return RedirectToAction("Index"); //HttpNotFound();
                        ////lỗi xóa các bài viết
                        //delnews:
                        //news = db.News.Where(d => d.Category.Equals(viewModel.Id)).FirstOrDefault();
                        //if (news == null) goto enddel;
                        //else
                        //{
                        //    db.News.Remove(news);
                        //    unitOfWork.Commit();
                        //    CacheService.ClearStartsWith(CacheKeys.News.StartsWith);
                        //    goto delnews;
                        //}
                        //enddel:

                        _categoryService.Dele(cat);

                        unitOfWork.Commit();
                        CacheService.ClearStartsWith(CacheKeys.Category.StartsWith);
                        // Show notifications to users
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Deleted category successfully",
                            MessageType = GenericMessages.success
                        };

                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex.Message);
                        ModelState.AddModelError(string.Empty, "Can't delete category now "+ex);
                    }
                }
            }

            return View(viewModel);
        }
        #endregion
    }
}