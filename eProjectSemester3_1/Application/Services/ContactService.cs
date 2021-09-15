using eProjectSemester3_1.Application.Context;
using eProjectSemester3_1.Application.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eProjectSemester3_1.Application.Services
{
    public class ContactService
    {
        public readonly AppDbContext _context;
        public readonly CacheService _cacheService;

        /// <summary>
        /// Constructor
        /// </summary>
        public ContactService(AppDbContext context, CacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public Contact Add(Contact contact)
        {
            contact.CreateDate = DateTime.Now;
            return _context.Contact.Add(contact);
        }

        public Contact Get(int id,bool cache = true)
        {
            if (!cache) return _context.Contact.FirstOrDefault(x => x.Id == id);

            var cacheKey = string.Concat(CacheKeys.Contact.StartsWith, "Get-",id);
            var contact = _cacheService.Get<Contact>(cacheKey);
            if (contact == null)
            {
                contact = _context.Contact.FirstOrDefault(x => x.Id == id);
                _cacheService.Set(cacheKey, contact, CacheTimes.OneHour);
            }

            return contact;
        }

        public int ContactCount()
        {
            var cacheKey = string.Concat(CacheKeys.Contact.StartsWith, "ContactCount");
            var count = _cacheService.Get<int?>(cacheKey);
            if (count == null)
            {
                count = _context.Contact.AsNoTracking().Count();

                _cacheService.Set(cacheKey, count, CacheTimes.OneHour);
            }
            return (int)count;
        }

        /// <summary>
        /// Get list user
        /// </summary>
        public List<Contact> GetList(int pageIndex, int pageSize)
        {
            var cacheKey = string.Concat(CacheKeys.Contact.StartsWith, "GetList-", pageIndex, "-", pageSize);
            var list = _cacheService.Get<List<Contact>>(cacheKey);
            if (list == null)
            {
                list = _context.Contact.AsNoTracking()
                            .OrderByDescending(x => x.CreateDate)
                            .Skip((pageIndex - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

                _cacheService.Set(cacheKey, list, CacheTimes.OneHour);
            }
            return list;
        }

    }
}