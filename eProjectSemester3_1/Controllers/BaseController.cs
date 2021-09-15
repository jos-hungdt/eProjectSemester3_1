using eProjectSemester3_1.Application;
using eProjectSemester3_1.Application.Entities;
using eProjectSemester3_1.Application.Services;
using eProjectSemester3_1.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace eProjectSemester3_1.Controllers
{
    public class BaseController : Controller
    {
        // Service
        public readonly MembershipService MembershipService;
        public readonly UnitOfWorkManager UnitOfWorkManager;
        public readonly LoggingService LoggingService;

        // Now login user
        public MembershipUser LoginUser;

        protected bool UserIsAuthenticated => System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
        protected string Username => UserIsAuthenticated ? System.Web.HttpContext.Current.User.Identity.Name : null;

        /// <summary>
        /// Constructor
        /// </summary>
        public BaseController(MembershipService membershipSevice, UnitOfWorkManager unitOfWorkManager, LoggingService loggingService)
        {
            MembershipService = membershipSevice;
            UnitOfWorkManager = unitOfWorkManager;
            LoggingService = loggingService;
        }


        protected override void OnAuthentication(AuthenticationContext filterContext)
        {
            base.OnAuthentication(filterContext);

            // Get account for Authentication
            LoginUser = UserIsAuthenticated ? MembershipService.GetUser(Username) : null;

            // If not account then clear Authentication
            if (!Username.IsNullEmpty() && LoginUser == null)
            {
                System.Web.Security.FormsAuthentication.SignOut();
                filterContext.Result = RedirectToAction("index", "Home");
            }
        }


        protected internal PageingViewModel CalcPaging(int limit, int? page, int count)
        {
            var paging = new PageingViewModel
            {
                Count = count,
                Page = page ?? 1,
                MaxPage = (count / limit) + ((count % limit > 0) ? 1 : 0),
            };
            if (paging.MaxPage == 0) paging.MaxPage = 1;

            if (paging.Page > paging.MaxPage) paging.Page = paging.MaxPage;

            return paging;
        }

        #region User
        protected static bool IsRole(string role)
        {
            return System.Web.HttpContext.Current.User.IsInRole(role);
        }
        #endregion
    }
}