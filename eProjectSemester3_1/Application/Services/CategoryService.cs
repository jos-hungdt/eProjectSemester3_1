using eProjectSemester3_1.Application.Context;
using eProjectSemester3_1.Application.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace eProjectSemester3_1.Application.Services
{
    public class CategoryService
    {
        public readonly AppDbContext _context;
        public readonly CacheService _cacheService;

        /// <summary>
        /// Constructor
        /// </summary>
        public CategoryService(AppDbContext context, CacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        /// <summary>
        /// Add a new category
        /// </summary>
        /// <param name="category"></param>
        public Category Add(Category category)
        {
            // Add the category
            return _context.Category.Add(category);
        }

        /// <summary>
        /// Return category by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Category Get(int id)
        {
            var cacheKey = string.Concat(CacheKeys.Category.StartsWith, "Get-", id);
            return _cacheService.CachePerRequest(cacheKey, () => _context.Category.FirstOrDefault(x => x.Id == id));
        }

        public News Getnews(int cate)
        {
            //var news = from db in _context.News where db.Category.Equals(cate) select db;
            //return news;
            News news = _context.News.Where(d => d.Category.Equals(cate)).SingleOrDefault();
            return news;
        }

        ///<summary>
        /// Delete Category
        /// </summary>
        public void Del(News news)
        {
            _context.News.Remove(news);
        }

        public void Dele(Category category)
        {
            _context.Category.Remove(category);
        }

        /// <summary>
        /// Get all catergory
        /// </summary>
        public List<Category> GetAll()
        {
            var cacheKey = string.Concat(CacheKeys.Category.StartsWith, "GetAll");
            var list = _cacheService.Get<List<Category>>(cacheKey);
            if (list == null)
            {
                list = _context.Category.AsNoTracking()
                            .OrderByDescending(x => x.CreateDate)
                            .ToList();

                _cacheService.Set(cacheKey, list, CacheTimes.OneDay);
            }
            return list;
        }

        /// <summary>
        /// Get Count catergory
        /// </summary>
        public int CategoryCount()
        {
            var cacheKey = string.Concat(CacheKeys.Category.StartsWith, "CategoryCount");
            var count = _cacheService.Get<int?>(cacheKey);
            if (count == null)
            {
                count = _context.Category.AsNoTracking().Count();

                _cacheService.Set(cacheKey, count, CacheTimes.OneDay);
            }
            return (int)count;
        }

        /// <summary>
        /// Get list catergory
        /// </summary>
        public List<Category> GetList(int pageIndex, int pageSize)
        {
            var cacheKey = string.Concat(CacheKeys.Category.StartsWith, "GetList-", pageIndex, "-", pageSize);
            var list = _cacheService.Get<List<Category>>(cacheKey);
            if (list == null)
            {
                list = _context.Category.AsNoTracking()
                            .OrderByDescending(x => x.CreateDate)
                            .Skip((pageIndex - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

                _cacheService.Set(cacheKey, list, CacheTimes.OneDay);
            }
            return list;
        }


        public List<SelectListItem> GetBaseSelectListCategories(List<Category> allowedCategories)
        {
            var cacheKey = string.Concat(CacheKeys.Category.StartsWith, "GetBaseSelectListCategories", "-", allowedCategories.GetHashCode());
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                var cats = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
                foreach (var cat in allowedCategories)
                {
                    cats.Add(new SelectListItem { Text = cat.Name, Value = cat.Id.ToString() });
                }
                return cats;
            });
        }

    }
}