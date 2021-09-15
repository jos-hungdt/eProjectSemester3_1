using eProjectSemester3_1.Application;
using eProjectSemester3_1.Application.Context;
using eProjectSemester3_1.Application.Entities;
using eProjectSemester3_1.Application.Services;
using eProjectSemester3_1.Areas.Admin.ViewModels;
using System;
using System.Data.SqlTypes;
using System.Web.Mvc;

namespace eProjectSemester3_1.Areas.Admin.Controllers
{
    [Authorize(Roles = AppConstants.AdminRoleName)]
    public class AdminNewsController : BaseAdminController
    {
        public readonly NewsService _newsService;
        public readonly PostService _postService;
        public readonly CategoryService _categoryService;

        public AdminNewsController(CategoryService categoryService,PostService postService, NewsService newsService, MembershipService membershipSevice, UnitOfWorkManager unitOfWorkManager, LoggingService loggingService, CacheService cacheService, SettingService settingService)
            : base(membershipSevice, unitOfWorkManager, loggingService, cacheService, settingService)
        {
            _newsService = newsService;
            _postService = postService;
            _categoryService = categoryService;
        }

        // GET: Admin/AdminNews
        public ActionResult Index(int? p)
        {

            int count = MembershipService.MemberCount();

            var Paging = CalcPaging(AppConstants.NewsPageSize, p, count);

            var model = new AdminNewsListViewModel
            {
                Paging = Paging,
                ListNews = _newsService.GetList(Paging.Page, AppConstants.NewsPageSize)
            };
            return View(model);

        }


        public ActionResult Show(int id)
        {
            var news = _newsService.Get(id, true);
            if (news == null) return RedirectToAction("Index");

            var post = _postService.GetStart(news);

            var viewModel = new AdminNewsShowViewModel
            {
                News = news,
                Post = post
            };

            return View(viewModel);
        }

        #region Create news
        [Authorize]
        public ActionResult Create()
        {

            var viewModel = new AdminNewsCreateEditViewModel
            {
                AllCategory = _categoryService.GetBaseSelectListCategories(_categoryService.GetAll()),
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AdminNewsCreateEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var news = new News
                        {
                            Title = viewModel.Title,
                            Category = _categoryService.Get(viewModel.Category),
                            CreateDate = DateTime.Now,
                            Image = viewModel.Image,
                            EditDate = (DateTime)SqlDateTime.MinValue,
                            UserPost = LoginUser,
                            UserEdit = LoginUser,
                            ShortContent = string.Concat(StringUtils.ReturnAmountWordsFromString(StringUtils.StripHtmlFromString(viewModel.Content), 50), "...."),
                        };

                        _newsService.Add(news);
                        //unitOfWork.SaveChanges();

                        var post = new Post
                        {
                            Content = viewModel.Content,
                            CreateDate = DateTime.Now,
                            EditDate = (DateTime)SqlDateTime.MinValue,
                            UserPost = LoginUser,
                            UserEdit = LoginUser,
                            isStart = true,
                            News = news,
                        };

                        _postService.Add(post);
                        //unitOfWork.SaveChanges();

                        unitOfWork.Commit();
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Create News success",
                            MessageType = GenericMessages.success
                        };
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex.Message);
                        ModelState.AddModelError(string.Empty, "Create News Error");
                    }
                }
            }

            viewModel.AllCategory = _categoryService.GetBaseSelectListCategories(_categoryService.GetAll());
            return View(viewModel);
        }

        #endregion

        #region Edit News
        public ActionResult Edit(int id)
        {
            var news = _newsService.Get(id, true);
            if (news == null) return RedirectToAction("Index");

            var post = _postService.GetStart(news);

            var viewModel = new AdminNewsCreateEditViewModel
            {
                Id = news.Id,
                Image = news.Image,
                Title = news.Title,
                Content = post.Content,
                Category = news.Category.Id,
                AllCategory = _categoryService.GetBaseSelectListCategories(_categoryService.GetAll()),
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AdminNewsCreateEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var news = _newsService.Get(viewModel.Id);
                        if (news == null) return HttpNotFound();// RedirectToAction("Index");

                        var post = _postService.GetStart(news);
                        bool isNewPost = false;
                        if (post == null)
                        {
                            post = new Post
                            {
                                CreateDate = news.CreateDate,
                                UserPost = news.UserPost,
                                isStart = true,
                                News = news,
                            };

                            isNewPost = true;
                        }
                        news.Image = viewModel.Image;
                        news.Title = viewModel.Title;
                        news.Category = _categoryService.Get(viewModel.Category);
                        news.EditDate = DateTime.Now;
                        news.UserEdit = LoginUser;
                        news.ShortContent = string.Concat(StringUtils.ReturnAmountWordsFromString(StringUtils.StripHtmlFromString(viewModel.Content), 50), "....");

                        unitOfWork.SaveChanges();

                        post.Content = viewModel.Content;
                        post.EditDate = news.EditDate;
                        post.UserEdit = LoginUser;

                        if (isNewPost)
                        {
                            _postService.Add(post);
                        }

                        unitOfWork.SaveChanges();

                        unitOfWork.Commit();
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Update News success",
                            MessageType = GenericMessages.success
                        };
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex.Message);
                        ModelState.AddModelError(string.Empty, "Edit News Error");
                    }
                }
            }

            viewModel.AllCategory = _categoryService.GetBaseSelectListCategories(_categoryService.GetAll());
            return View(viewModel);
        }
        #endregion

        #region delete news
        public ActionResult Delete(int id)
        {
            var news = _newsService.Get(id, true);
            if (news == null) return RedirectToAction("Index");

            var post = _postService.GetStart(news);

            var viewModel = new AdminNewsDeleteViewModel
            {
                Id = news.Id,
                Title = news.Title
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(AdminNewsDeleteViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var news = _newsService.Get(viewModel.Id);
                        if (news == null) return HttpNotFound();// RedirectToAction("Index");

                        _postService.Del(news);
                        _newsService.Del(news);

                        unitOfWork.Commit();
                        CacheService.ClearStartsWith(CacheKeys.News.StartsWith);
                        TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = "Delete News success",
                            MessageType = GenericMessages.success
                        };
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex.Message);
                        ModelState.AddModelError(string.Empty, "Delete News Error");
                    }
                }
            }
            
            return View(viewModel);
        }
        #endregion
    }
}