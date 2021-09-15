using eProjectSemester3_1.Application.Context;
using eProjectSemester3_1.Application.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eProjectSemester3_1.Application.Services
{
    public class PostEstateTypeService
    {
        public readonly AppDbContext _context;
        public readonly CacheService _cacheService;

        /// <summary>
        /// Constructor
        /// </summary>
        public PostEstateTypeService(AppDbContext context, CacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public PostEstateType Add(PostEstateType estate)
        {
            return _context.PostEstateType.Add(estate);
        }

        public PostEstateType Get(int id, bool cache = true)
        {
            if (!cache) return _context.PostEstateType.FirstOrDefault(x => x.postTypeID == id);

            var cacheKey = string.Concat(CacheKeys.EstateType.StartsWith, "Get-", id);
            var contact = _cacheService.Get<PostEstateType>(cacheKey);
            if (contact == null)
            {
                contact = _context.PostEstateType.FirstOrDefault(x => x.postTypeID == id);
                _cacheService.Set(cacheKey, contact, CacheTimes.OneHour);
            }

            return contact;
        }

        /// <summary>
        /// Get all PostEstateType
        /// </summary>
        public List<PostEstateType> GetAll()
        {
            var cacheKey = string.Concat(CacheKeys.PostEstateType.StartsWith, "GetAll");
            var list = _cacheService.Get<List<PostEstateType>>(cacheKey);
            if (list == null)
            {
                list = _context.PostEstateType.AsNoTracking()
                            .OrderByDescending(x => x.postTypeName)
                            .ToList();

                _cacheService.Set(cacheKey, list, CacheTimes.OneDay);
            }
            return list;
        }


        public List<SelectListItem> GetBaseSelectListPostEstateType(List<PostEstateType> allowedEstateStyle)
        {
            var cacheKey = string.Concat(CacheKeys.EstateStyle.StartsWith, "GetBaseSelectListPostEstateType", "-", allowedEstateStyle.GetHashCode());
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                var cats = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
                foreach (var cat in allowedEstateStyle)
                {
                    cats.Add(new SelectListItem { Text = cat.postTypeName, Value = cat.postTypeID.ToString() });
                }
                return cats;
            });
        }
    }
}