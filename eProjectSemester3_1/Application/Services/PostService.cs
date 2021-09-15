using eProjectSemester3_1.Application.Context;
using eProjectSemester3_1.Application.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eProjectSemester3_1.Application.Services
{
    public class PostService
    {
        public readonly AppDbContext _context;
        public readonly CacheService _cacheService;

        /// <summary>
        /// Constructor
        /// </summary>
        public PostService(AppDbContext context, CacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public Post Add(Post post)
        {
            return _context.Post.Add(post);
            // ở đây ko cần avechange
        }

        public void Del(Post post)
        {
            _context.Post.Remove(post);
        }

        public void Del(News news)
        {
            _context.Post.RemoveRange(news.Posts);
        }

        public Post Get(int id, bool removeTracking = false)
        {
            var cacheKey = string.Concat(CacheKeys.Post.StartsWith, "Get-", id, "-", removeTracking);
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                if (removeTracking)
                {
                    return _context.Post.AsNoTracking().FirstOrDefault(x => x.Id == id);
                }

                return _context.Post.FirstOrDefault(x => x.Id == id);

            });
        }

        public Post GetStart(News news)
        {
            var cacheKey = string.Concat(CacheKeys.Post.StartsWith, "GetStart-", news);
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                return _context.Post.AsNoTracking().FirstOrDefault(x => x.News.Id == news.Id && x.isStart == true);

            });
        }


    }
}