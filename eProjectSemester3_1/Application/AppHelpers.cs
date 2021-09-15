using eProjectSemester3_1.Application.Context;
using eProjectSemester3_1.Application.Entities;
using eProjectSemester3_1.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eProjectSemester3_1.Application
{
    public class AppHelpers
    {
        #region Setting
        public static string GetSetting(string key)
        {
            return ServiceFactory.Get<SettingService>().GetSetting(key);
        }
        #endregion

        #region User
        public static bool IsRole(string role)
        {
            return HttpContext.Current.User.IsInRole(role);
        }
        #endregion

        public static List<Category> GetCategories() // get latest news
        {
            AppDbContext db = ServiceFactory.Get<AppDbContext>();

            var ltn = (from i in db.Category
                       select i).OrderBy(T => T.SortOrder).Take(5);
            if (ltn != null) return (ltn.ToList());
            return (null);
        }
        public static List<News> GetNews() // get latest news
        {
            AppDbContext db = ServiceFactory.Get<AppDbContext>();

            var ltn = (from i in db.News
                       select i).Where(t => t.Category.Name != "FAQs").Take(5);
            if (ltn != null) return (ltn.ToList());
            return (null);
        }
        public static List<News> GetRelatedNews(int? id) // get latest news
        {
            AppDbContext db = ServiceFactory.Get<AppDbContext>();

            var ltn = (from i in db.News
                       select i).Where(t => t.Category.Id == id).Take(5);
            if (ltn != null) return (ltn.ToList());
            return (null);
        }
        public static List<Estate> Related(int typeid, string city) // Get related property
        {
            AppDbContext db = ServiceFactory.Get<AppDbContext>();
            var Estate = (from q in db.Estate
                          where q.City == city &&
                          q.EstateType.realStateTypeID == typeid
                          select q).Take(5);
            return Estate.ToList();
        }
        public static List<Estate> vip_estate() // Get paid estate post
        {
            AppDbContext db = ServiceFactory.Get<AppDbContext>();
            var Estate = (from q in db.Estate
                          where q.PostType.postTypeID == 2
                          select q).Take(6);
            return Estate.ToList();
        }
        public static List<EstateStyle> GetEstateStyle()
        {
            AppDbContext db2 = ServiceFactory.Get<AppDbContext>();
            var EstateStyle = from q in db2.EstateStyle
                              where q.realEstateStyleStatus == 1
                              select q;
            return EstateStyle.ToList();
        }
        public static List<EstateType> GetEstateType()
        {
            AppDbContext db2 = ServiceFactory.Get<AppDbContext>();
            var type = from q in db2.EstateType
                       where q.realStateTypeStatus == 1
                       select q;
            return type.ToList();
        }


        public class SearchModel
        {
            public int? salerent { get; set; }
            public int? type { get; set; }
            public string q { get; set; }
            public string location { get; set; }
            public int? bedroom { get; set; }
            public int? bathroom { get; set; }
            public decimal? areafrom { get; set; }
            public decimal? areato { get; set; }
            public int? style { get; set; }
        }
        public class EstateSearch
        {
            private AppDbContext db;
            public EstateSearch()
            {
                db = ServiceFactory.Get<AppDbContext>();
            }

            public IQueryable<Estate> GetResult(SearchModel searchModel)
            {
                var result = db.Estate.AsQueryable();
                if (searchModel != null)
                {
                    if (searchModel.type.HasValue)
                        result = result.Where(x => x.EstateType.realStateTypeID == searchModel.salerent);
                    if (searchModel.type.HasValue)
                        result = result.Where(x => x.EstateType.realStateTypeID == searchModel.type);
                    if (!string.IsNullOrEmpty(searchModel.q))
                        result = result.Where(x => x.realEstateTitle.Contains(searchModel.q));
                    if (!string.IsNullOrEmpty(searchModel.location))
                        result = result.Where(x => x.City.Contains(searchModel.location));
                    if (searchModel.style.HasValue)
                        result = result.Where(x => x.EstateType.realStateTypeID == searchModel.style);
                    if (searchModel.bedroom.HasValue)
                        result = result.Where(x => x.NoOfBedrooms == searchModel.bedroom);
                    if (searchModel.bathroom.HasValue)
                        result = result.Where(x => x.NoOfBathrooms == searchModel.bathroom);
                    if (searchModel.style.HasValue)
                        result = result.Where(x => x.EstateType.realStateTypeID == searchModel.style);
                    if (searchModel.areafrom.HasValue)
                        result = result.Where(x => x.Price >= searchModel.areafrom);
                    if (searchModel.areato.HasValue)
                        result = result.Where(x => x.Price <= searchModel.areato);
                }
                return result;

            }
        }
    }
}