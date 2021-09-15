using eProjectSemester3_1.Application;
using eProjectSemester3_1.Application.Entities;
using eProjectSemester3_1.Application.Services;
using eProjectSemester3_1.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eProjectSemester3_1.Controllers
{
    public class NewsController : BaseController
    {
        public readonly NewsService _newsService;
        public readonly PostService _postService;

        public NewsController(PostService postService,NewsService newsService,MembershipService membershipSevice, UnitOfWorkManager unitOfWorkManager, LoggingService loggingService)
            : base(membershipSevice, unitOfWorkManager, loggingService)
        {
            _newsService = newsService;
            _postService = postService;
        }

        // GET: News
        public ActionResult Index(int? p)
        {

            int count = MembershipService.MemberCount();

            var Paging = CalcPaging(AppConstants.NewsPageSize, p, count);

            var model = new NewsListViewModel
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

            var viewModel = new NewsShowViewModel
            {
                News = news,
                Post = post
            };

            return View(viewModel);
        }

        #region Create news
        //[Authorize]
        //public ActionResult Create()
        //{

        //    var viewModel = new NewsCreateEditViewModel {

        //    };

        //    return View(viewModel);
        //}

        //[Authorize]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(NewsCreateEditViewModel viewModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
        //        {
        //            try
        //            {
        //                var news = new News {
        //                    Title = viewModel.Title,
        //                    CreateDate = DateTime.Now,
        //                    EditDate = (DateTime)SqlDateTime.MinValue,
        //                    UserPost = LoginUser,
        //                    UserEdit = LoginUser,
        //                    ShortContent = string.Concat(StringUtils.ReturnAmountWordsFromString(StringUtils.StripHtmlFromString(viewModel.Content), 50), "...."),
        //            };

        //                _newsService.Add(news);
        //                //unitOfWork.SaveChanges();

        //                var post = new Post
        //                {
        //                    Content = viewModel.Content,
        //                    CreateDate = DateTime.Now,
        //                    EditDate = (DateTime)SqlDateTime.MinValue,
        //                    UserPost = LoginUser,
        //                    UserEdit = LoginUser,
        //                    isStart = true,
        //                    News = news,
        //                };

        //                _postService.Add(post);
        //                //unitOfWork.SaveChanges();

        //                unitOfWork.Commit();
        //                return RedirectToAction("Show",new { id = news.Id });
        //            }
        //            catch(Exception ex)
        //            {
        //                unitOfWork.Rollback();
        //                LoggingService.Error(ex.Message);
        //                ModelState.AddModelError(string.Empty, "Create News Error");
        //            }
        //        }
        //    }

        //    return View(viewModel);
        //}

       #endregion

        #region Edit News
        //public ActionResult Edit(int id)
        //{
        //    var news = _newsService.Get(id, true);
        //    if (news == null) return RedirectToAction("Index");

        //    var post = _postService.GetStart(news);

        //    var viewModel = new NewsCreateEditViewModel
        //    {
        //        Id = news.Id,
        //        Title = news.Title,
        //        Content = post.Content
        //    };

        //    return View(viewModel);
        //}

        //[Authorize]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(NewsCreateEditViewModel viewModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
        //        {
        //            try
        //            {
        //                var news = _newsService.Get(viewModel.Id);
        //                if (news == null) return HttpNotFound();// RedirectToAction("Index");

        //                var post = _postService.GetStart(news);
        //                bool isNewPost = false;
        //                if(post == null)
        //                {
        //                    post = new Post
        //                    {
        //                        CreateDate = news.CreateDate,
        //                        UserPost = news.UserPost,
        //                        isStart = true,
        //                        News = news,
        //                    };

        //                    isNewPost = true;
        //                }

        //                news.Title = viewModel.Title;
        //                news.EditDate = DateTime.Now;
        //                news.UserEdit = LoginUser;
        //                news.ShortContent = string.Concat(StringUtils.ReturnAmountWordsFromString(StringUtils.StripHtmlFromString(viewModel.Content), 50), "....");
                        
        //                unitOfWork.SaveChanges();

        //                post.Content = viewModel.Content;
        //                post.EditDate = news.EditDate;
        //                post.UserEdit = LoginUser;

        //                if (isNewPost)
        //                {
        //                    _postService.Add(post);
        //                }
                        
        //                unitOfWork.SaveChanges();

        //                unitOfWork.Commit();
        //            }
        //            catch (Exception ex)
        //            {
        //                unitOfWork.Rollback();
        //                LoggingService.Error(ex.Message);
        //                ModelState.AddModelError(string.Empty, "Edit News Error");
        //            }
        //        }
        //    }

        //    return View(viewModel);
        //}
        #endregion

    }
}