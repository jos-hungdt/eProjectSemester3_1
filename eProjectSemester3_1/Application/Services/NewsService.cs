namespace eProjectSemester3_1.Application.Services
{
    using eProjectSemester3_1.Application.Context;
    using eProjectSemester3_1.Application.Entities;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Web;

    public class NewsService
    {
        public readonly AppDbContext _context;
        public readonly CacheService _cacheService;

        /// <summary>
        /// Constructor
        /// </summary>
        public NewsService(AppDbContext context, CacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public News Add(News news)
        {
            return _context.News.Add(news);
        }

        public News Get(int id,bool removeTracking = false)
        {
            var cacheKey = string.Concat(CacheKeys.News.StartsWith, "Get-",id,"-",removeTracking);
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                if (removeTracking)
                {
                    return _context.News.AsNoTracking().FirstOrDefault(x => x.Id == id);
                }
                
                return _context.News.FirstOrDefault(x => x.Id == id);

            });
        }

        public void Del(News news)
        {
            _context.News.Remove(news);
        }

        /// <summary>
        /// Get count news
        /// </summary>
        public int NewsCount()
        {
            var cacheKey = string.Concat(CacheKeys.News.StartsWith, "MemberCount");
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                return _context.News.AsNoTracking().Count();
            });
        }

        /// <summary>
        /// Get list news
        /// </summary>
        public List<News> GetList(int pageIndex, int pageSize)
        {
            var cacheKey = string.Concat(CacheKeys.News.StartsWith, "GetList-", pageIndex, "-", pageSize);
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                return _context.News.AsNoTracking()
                            .Include(x => x.UserPost)
                            .Include(x => x.UserEdit)
                            .OrderByDescending(x => x.CreateDate)
                            .Skip((pageIndex - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
            });

        }

    }
}