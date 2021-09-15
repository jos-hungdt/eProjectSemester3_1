using eProjectSemester3_1.Application.Context;
using eProjectSemester3_1.Application.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eProjectSemester3_1.Application.Services
{
    public class FaqsService
    {
        public readonly AppDbContext _context;
        public readonly CacheService _cacheService;

        /// <summary>
        /// Constructor
        /// </summary>
        public FaqsService(AppDbContext context, CacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public Faqs Add(Faqs faqs)
        {
            return _context.Faqs.Add(faqs);
        }

        public Faqs Get(int id, bool cache = true)
        {
            if(!cache) return _context.Faqs.FirstOrDefault(x => x.faqID == id);

            var cacheKey = string.Concat(CacheKeys.Faqs.StartsWith, "Get-",id);
            var count = _cacheService.Get<Faqs>(cacheKey);
            if (count == null)
            {
                count = _context.Faqs.FirstOrDefault(x => x.faqID == id);

                _cacheService.Set(cacheKey, count, CacheTimes.OneDay);
            }
            return count;
        }
        public void Repmove(Faqs faqs)
        {
            _context.Faqs.Remove(faqs);
        }

        /// <summary>
        /// Get all Faqs
        /// </summary>
        public List<Faqs> GetAll()
        {
            var cacheKey = string.Concat(CacheKeys.Faqs.StartsWith, "GetAll");
            var list = _cacheService.Get<List<Faqs>>(cacheKey);
            if (list == null)
            {
                list = _context.Faqs.AsNoTracking().ToList();

                _cacheService.Set(cacheKey, list, CacheTimes.OneDay);
            }
            return list;
        }

        public int FaqsCount()
        {
            var cacheKey = string.Concat(CacheKeys.Faqs.StartsWith, "FaqsCount");
            var count = _cacheService.Get<int?>(cacheKey);
            if (count == null)
            {
                count = _context.Faqs.AsNoTracking().Count();

                _cacheService.Set(cacheKey, count, CacheTimes.OneDay);
            }
            return (int)count;
        }

        /// <summary>
        /// Get list Faqs
        /// </summary>
        public List<Faqs> GetList(int pageIndex, int pageSize)
        {
            var cacheKey = string.Concat(CacheKeys.Faqs.StartsWith, "GetList-", pageIndex, "-", pageSize);
            var list = _cacheService.Get<List<Faqs>>(cacheKey);
            if (list == null)
            {
                list = _context.Faqs.AsNoTracking()
                            .OrderByDescending(x => x.question)
                            .Skip((pageIndex - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

                _cacheService.Set(cacheKey, list, CacheTimes.OneDay);
            }
            return list;
        }
    }
}