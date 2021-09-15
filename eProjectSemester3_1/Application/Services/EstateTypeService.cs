using eProjectSemester3_1.Application.Context;
using eProjectSemester3_1.Application.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eProjectSemester3_1.Application.Services
{
    public class EstateTypeService
    {
        public readonly AppDbContext _context;
        public readonly CacheService _cacheService;

        /// <summary>
        /// Constructor
        /// </summary>
        public EstateTypeService(AppDbContext context, CacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }


        public EstateType Add(EstateType estate)
        {
            return _context.EstateType.Add(estate);
        }

        public EstateType Get(int id, bool cache = true)
        {
            if (!cache) return _context.EstateType.FirstOrDefault(x => x.realStateTypeID == id);

            var cacheKey = string.Concat(CacheKeys.EstateType.StartsWith, "Get-", id);
            var contact = _cacheService.Get<EstateType>(cacheKey);
            if (contact == null)
            {
                contact = _context.EstateType.FirstOrDefault(x => x.realStateTypeID == id);
                _cacheService.Set(cacheKey, contact, CacheTimes.OneHour);
            }

            return contact;
        }

        /// <summary>
        /// Get all EstateStyle
        /// </summary>
        public List<EstateType> GetAll()
        {
            var cacheKey = string.Concat(CacheKeys.EstateType.StartsWith, "GetAll");
            var list = _cacheService.Get<List<EstateType>>(cacheKey);
            if (list == null)
            {
                list = _context.EstateType.AsNoTracking()
                            .OrderByDescending(x => x.realStateTypeName)
                            .ToList();

                _cacheService.Set(cacheKey, list, CacheTimes.OneDay);
            }
            return list;
        }


        public List<SelectListItem> GetBaseSelectListEstateType(List<EstateType> allowedEstateType)
        {
            var cacheKey = string.Concat(CacheKeys.EstateType.StartsWith, "GetBaseSelectListEstateType", "-", allowedEstateType.GetHashCode());
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                var cats = new List<SelectListItem> { new SelectListItem { Text = "", Value = "" } };
                foreach (var cat in allowedEstateType)
                {
                    cats.Add(new SelectListItem { Text = cat.realStateTypeName, Value = cat.realStateTypeID.ToString() });
                }
                return cats;
            });
        }
    }
}