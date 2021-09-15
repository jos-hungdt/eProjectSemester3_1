using eProjectSemester3_1.Application.Context;
using eProjectSemester3_1.Application.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eProjectSemester3_1.Application.Services
{
    public class SettingService
    {
        public readonly AppDbContext _context;
        public readonly CacheService _cacheService;

        /// <summary>
        /// Constructor
        /// </summary>
        public SettingService(AppDbContext context, CacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public Setting Add(Setting setting)
        {
            return _context.Setting.Add(setting);
        }

        public Setting Get(string key, bool cache = true)
        {
            if (!cache) return _context.Setting.FirstOrDefault(x => x.Key == key);

            var cacheKey = string.Concat(CacheKeys.Setting.StartsWith, "Get-", key);
            var list = _cacheService.Get<Setting>(cacheKey);
            if (list == null)
            {
                list = _context.Setting.AsNoTracking()
                                    .FirstOrDefault(x => x.Key == key);

                _cacheService.Set(cacheKey, list, CacheTimes.OneDay);
            }
            return list;
        }

        public string GetSetting(string key)
        {
            try
            {
                return Get(key).Value;
            }
            catch
            {
                return "";
            }
        }

        public Setting SetSetting(string key, string value)
        {
            var cacheKey = string.Concat(CacheKeys.Setting.StartsWith, "Get-", key);
            var st = Get(key, false);
            if (st != null)
            {
                st.Value = value;
                _context.SaveChanges();
            }
            else
            {
                st = new Setting
                {
                    Key = key,
                    Value = value
                };

                st = Add(st);
            }
            _cacheService.ClearStartsWith(cacheKey);

            return st;
        }

    }
}