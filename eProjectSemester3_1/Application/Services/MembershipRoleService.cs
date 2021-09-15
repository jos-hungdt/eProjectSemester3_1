using eProjectSemester3_1.Application.Context;
using eProjectSemester3_1.Application.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace eProjectSemester3_1.Application.Services
{
    public class MembershipRoleService
    {
        public readonly AppDbContext _context;
        public readonly CacheService _cacheService;

        /// <summary>
        /// Constructor
        /// </summary>
        public MembershipRoleService(AppDbContext context, CacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public MembershipRole Add(MembershipRole membershipRole)
        {
            return _context.MembershipRole.Add(membershipRole);
        }

        public MembershipRole Get(int id)
        {
            return GetAll().LastOrDefault(x => x.Id == id);
        }

        public MembershipRole GetByRole(string role)
        {
            return GetAll().LastOrDefault(x => x.RoleName == role);
        }

        public List<MembershipRole> GetAll()
        {
            var cacheKey = string.Concat(CacheKeys.Member.StartsWith, "GetAll");
            var list = _cacheService.Get<List<MembershipRole>>(cacheKey);
            if(list == null)
            {
                list = _context.MembershipRole.Select(x => x).ToList();
            }
            return list;
        }
         public void Delete(MembershipRole membership)
        {
            _context.MembershipRole.Remove(membership);
        }
    }
}