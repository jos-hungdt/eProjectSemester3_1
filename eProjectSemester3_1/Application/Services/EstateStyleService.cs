using eProjectSemester3_1.Application.Context;
using eProjectSemester3_1.Application.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eProjectSemester3_1.Application.Services
{
    public class EstateStyleService
    {
        public readonly AppDbContext _context;
        public readonly CacheService _cacheService;

        /// <summary>
        /// Constructor
        /// </summary>
        public EstateStyleService(AppDbContext context, CacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }


        public EstateStyle Add(EstateStyle estate)
        {
            return _context.EstateStyle.Add(estate);
        }

        public EstateStyle Get(int id, bool cache = true)
        {
            if (!cache) return _context.EstateStyle.FirstOrDefault(x => x.realEstateStyleID == id);

            var cacheKey = string.Concat(CacheKeys.EstateStyle.StartsWith, "Get-", id);
            var contact = _cacheService.Get<EstateStyle>(cacheKey);
            if (contact == null)
            {
                contact = _context.EstateStyle.FirstOrDefault(x => x.realEstateStyleID == id);
                _cacheService.Set(cacheKey, contact, CacheTimes.OneHour);
            }

            return contact;
        }

        /// <summary>
        /// Get all EstateStyle
        /// </summary>
        public List<EstateStyle> GetAll()
        {
            var cacheKey = string.Concat(CacheKeys.EstateStyle.StartsWith, "GetAll");
            var list = _cacheService.Get<List<EstateStyle>>(cacheKey);
            if (list == null)
            {
                list = _context.EstateStyle.AsNoTracking()
                            .OrderByDescending(x => x.realEstateStyleName)
                            .ToList();

                _cacheService.Set(cacheKey, list, CacheTimes.OneDay);
            }
            return list;
        }


        public List<SelectListItem> GetBaseSelectListEstateStyle(List<EstateStyle> allowedEstateStyle)
        {
            var cacheKey = string.Concat(CacheKeys.EstateStyle.StartsWith, "GetBaseSelectListEstateStyle", "-", allowedEstateStyle.GetHashCode());
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                var cats = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
                foreach (var cat in allowedEstateStyle)
                {
                    cats.Add(new SelectListItem { Text = cat.realEstateStyleName, Value = cat.realEstateStyleID.ToString() });
                }
                return cats;
            });
        }
    }
}