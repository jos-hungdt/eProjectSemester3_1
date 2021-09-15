using eProjectSemester3_1.Application.Context;
using eProjectSemester3_1.Application.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace eProjectSemester3_1.Application.Services
{
    public class EstateService
    {
        public readonly AppDbContext _context;
        public readonly CacheService _cacheService;

        /// <summary>
        /// Constructor
        /// </summary>
        public EstateService(AppDbContext context, CacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }


        public Estate Add(Estate estate)
        {
            return _context.Estate.Add(estate);
        }

        public Estate Get(int id, bool cache = true)
        {
            if (!cache) return _context.Estate.Include(x => x.User).FirstOrDefault(x => x.RealEstateID == id);

            var cacheKey = string.Concat(CacheKeys.Estate.StartsWith, "Get-", id);
            var contact = _cacheService.Get<Estate>(cacheKey);
            if (contact == null)
            {
                contact = _context.Estate.FirstOrDefault(x => x.RealEstateID == id);
                _cacheService.Set(cacheKey, contact, CacheTimes.OneHour);
            }

            return contact;
        }

        public int EstateCount()
        {
            var cacheKey = string.Concat(CacheKeys.Estate.StartsWith, "EstateCount");
            var count = _cacheService.Get<int?>(cacheKey);
            if (count == null)
            {
                count = _context.Estate.AsNoTracking().Count();

                _cacheService.Set(cacheKey, count, CacheTimes.OneHour);
            }
            return (int)count;
        }

        /// <summary>
        /// Get list Estate
        /// </summary>
        public List<Estate> GetList(int pageIndex, int pageSize)
        {
            var cacheKey = string.Concat(CacheKeys.Estate.StartsWith, "GetList-", pageIndex, "-", pageSize);
            var list = _cacheService.Get<List<Estate>>(cacheKey);
            if (list == null)
            {
                list = _context.Estate.AsNoTracking()
                            .OrderByDescending(x => x.realEstateTitle)
                            .Skip((pageIndex - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

                _cacheService.Set(cacheKey, list, CacheTimes.OneHour);
            }
            return list;
        }

        public int EstateCount(MembershipUser user)
        {
            var cacheKey = string.Concat(CacheKeys.Estate.StartsWith, "EstateCount.User-", user.Id);
            var count = _cacheService.Get<int?>(cacheKey);
            if (count == null)
            {
                count = _context.Estate.AsNoTracking().Where(x => x.User.Id == user.Id).Count();

                _cacheService.Set(cacheKey, count, CacheTimes.OneHour);
            }
            return (int)count;
        }
        public Estate Remove(Estate estate)
        {
            return _context.Estate.Remove(estate);
        }
        /// <summary>
        /// Get list Estate by user
        /// </summary>
        public List<Estate> GetList(MembershipUser user,int pageIndex, int pageSize)
        {
            var cacheKey = string.Concat(CacheKeys.Estate.StartsWith, "GetList.User-", user.Id, "-", pageIndex, "-", pageSize);
            var list = _cacheService.Get<List<Estate>>(cacheKey);
            if (list == null)
            {
                list = _context.Estate.AsNoTracking()
                            .Where(x => x.User.Id == user.Id)
                            .OrderByDescending(x => x.realEstateTitle)
                            .Skip((pageIndex - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

                _cacheService.Set(cacheKey, list, CacheTimes.OneHour);
            }
            return list;
        }
    }
}