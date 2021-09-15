using eProjectSemester3_1.Application.Context;
using eProjectSemester3_1.Application.Entities;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlTypes;

namespace eProjectSemester3_1.Application.Services
{
    public enum LoginAttemptStatus
    {
        LoginSuccessful,
        UserNotFound,
        PasswordIncorrect,
        PasswordAttemptsExceeded,
        UserLockedOut,
        UserNotApproved,
        Banned
    }

    public class MembershipService
    {
        public readonly AppDbContext _context;
        public readonly CacheService _cacheService;

        /// <summary>
        /// Constructor
        /// </summary>
        public MembershipService(AppDbContext context, CacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public MembershipUser Add(MembershipUser member)
        {
            return _context.MembershipUser.Add(member);
        }

        /// <summary>
        /// Register a user
        /// </summary>
        /// /// <param name="userName"></param>
        /// /// <param name="Password"></param>
        /// /// <param name="email"></param>
        /// <returns></returns>
        public MembershipUser Register(string userName,string Password)
        {
            userName = StringUtils.SafePlainText(userName);
            Password = StringUtils.SafePlainText(Password);

            if (GetUser(userName) != null) throw new Exception("Account already exists");
            
            string PasswordSalt = CreatePasswordResetToken();

            var hash = StringUtils.GenerateSaltedHash(Password, PasswordSalt);

            DateTime time = DateTime.Now;

            var member = new MembershipUser
            {
                UserName = userName,
                Password = hash,
                PasswordSalt = PasswordSalt,
                CreateDate = time,
                LastLoginDate = time,
                LastPasswordChangedDate = (DateTime)SqlDateTime.MinValue,
                LastLockoutDate = (DateTime)SqlDateTime.MinValue,
                DateOfBirth = (DateTime)SqlDateTime.MinValue,

            };
            //var ret = _context.MembershipUser.Add(member);

            //_context.SaveChanges();

            return member;
        }


        /// <summary>
        /// Add a user
        /// </summary>
        /// /// <param name="user"></param>
        /// <returns></returns>
        public MembershipUser Get(MembershipUser user)
        {
            //user.UserName = StringUtils.SafePlainText(user.UserName);
            //user.Password = StringUtils.SafePlainText(user.Password);

            return _context.MembershipUser.Add(user);
        }

        /// <summary>
        /// Get a user by id
        /// </summary>
        /// /// <param name="id"></param>
        /// <returns></returns>
        public MembershipUser Get(int id)
        {
            MembershipUser user = _context.MembershipUser.FirstOrDefault(x => x.Id == id);
            return user;
        }

        /// <summary>
        /// Get all user
        /// </summary>
        public List<MembershipUser> GetAll()
        {
            return _context.MembershipUser.Select(x => x).ToList();
        }

        public int MemberCount()
        {
            var cacheKey = string.Concat(CacheKeys.Member.StartsWith, "MemberCount");
            var count = _cacheService.Get<int?>(cacheKey);
            if (count == null)
            {
                count = _context.MembershipUser.AsNoTracking().Count();

                _cacheService.Set(cacheKey, count, CacheTimes.OneDay);
            }
            return (int)count;
        }
        
        /// <summary>
        /// Get list user
        /// </summary>
        public List<MembershipUser> GetList(int pageIndex, int pageSize)
        {
            var cacheKey = string.Concat(CacheKeys.Member.StartsWith, "GetList-", pageIndex,"-", pageSize);
            var list = _cacheService.Get<List<MembershipUser>>(cacheKey);
            if (list == null)
            {
                list = _context.MembershipUser.AsNoTracking()
                            .Include(x => x.Roles)
                            .OrderByDescending(x => x.CreateDate)
                            .Skip((pageIndex - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

                _cacheService.Set(cacheKey, list, CacheTimes.OneDay);
            }
            return list;
        }


        /// <summary>
        /// Get a user by username
        /// </summary>
        /// <param name="username"></param>
        /// <param name="removeTracking"></param>
        /// <returns></returns>
        public MembershipUser GetUser(string username, bool removeTracking = false)
        {
            username = StringUtils.SafePlainText(username);

            var cacheKey = string.Concat(CacheKeys.Member.StartsWith, "GetUser-", username, "-", removeTracking);
            return _cacheService.CachePerRequest(cacheKey, () =>
            {
                var iQuery = _context.MembershipUser.Include(x => x.Roles);
                if (removeTracking)
                {
                    iQuery = iQuery.AsNoTracking();
                }
                MembershipUser member = iQuery.FirstOrDefault(name => name.UserName.Equals(username, StringComparison.CurrentCultureIgnoreCase));

                return member;
            });
        }

        /// <summary>
        /// Get a roles by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public string[] GetRolesForUser(string username)
        {
            //username = StringUtils.SafePlainText(username);
            var roles = new List<string>();
            var user = GetUser(username, true);

            if (user != null)
            {
                roles.AddRange(user.Roles.Select(role => role.RoleName));
            }

            return roles.ToArray();
        }

        /// <summary>
        /// Return last login status
        /// </summary>
        public LoginAttemptStatus LastLoginStatus { get; private set; } = LoginAttemptStatus.LoginSuccessful;

        /// <summary>
        /// Validate a user by password
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="maxInvalidPasswordAttempts"> </param>
        /// <returns></returns>
        public bool ValidateUser(string userName, string password, int maxInvalidPasswordAttempts)
        {
            userName = StringUtils.SafePlainText(userName);
            password = StringUtils.SafePlainText(password);

            LastLoginStatus = LoginAttemptStatus.LoginSuccessful;

            var user = GetUser(userName);

            if (user == null)
            {
                LastLoginStatus = LoginAttemptStatus.UserNotFound;
                return false;
            }

            if (user.IsBanned)
            {
                LastLoginStatus = LoginAttemptStatus.Banned;
                return false;
            }

            //if (user.IsLockedOut)
            //{
            //    LastLoginStatus = LoginAttemptStatus.UserLockedOut;
            //    return false;
            //}

            //if (!user.IsApproved)
            //{
            //    LastLoginStatus = LoginAttemptStatus.UserNotApproved;
            //    return false;
            //}

            var allowedPasswordAttempts = maxInvalidPasswordAttempts;
            if (user.FailedPasswordAttemptCount >= allowedPasswordAttempts)
            {
                LastLoginStatus = LoginAttemptStatus.PasswordAttemptsExceeded;
                return false;
            }

            var salt = user.PasswordSalt;
            var hash = StringUtils.GenerateSaltedHash(password, salt);
            var passwordMatches = hash == user.Password;

            user.FailedPasswordAttemptCount = passwordMatches ? 0 : user.FailedPasswordAttemptCount + 1;

            if (user.FailedPasswordAttemptCount >= allowedPasswordAttempts)
            {
                user.IsLockedOut = true;
                user.LastLockoutDate = DateTime.UtcNow;
            }

            if (!passwordMatches)
            {
                LastLoginStatus = LoginAttemptStatus.PasswordIncorrect;
                return false;
            }

            return LastLoginStatus == LoginAttemptStatus.LoginSuccessful;
        }

        /// <summary>
        /// Change Password for user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="newpassword"> </param>
        /// <returns></returns>
        public bool ChangePassword(string userName, string password, string newpassword)
        {
            userName = StringUtils.SafePlainText(userName);
            password = StringUtils.SafePlainText(password);
            newpassword = StringUtils.SafePlainText(newpassword);

            LastLoginStatus = LoginAttemptStatus.LoginSuccessful;

            var user = GetUser(userName);

            if (user == null)
            {
                LastLoginStatus = LoginAttemptStatus.UserNotFound;
                return false;
            }

            var salt = user.PasswordSalt;
            var hash = StringUtils.GenerateSaltedHash(password, salt);
            var passwordMatches = hash == user.Password;

            if (!passwordMatches)
            {
                LastLoginStatus = LoginAttemptStatus.PasswordIncorrect;
                return false;
            }

            var newhash = StringUtils.GenerateSaltedHash(newpassword, salt);

            user.Password = newhash;


            return LastLoginStatus == LoginAttemptStatus.LoginSuccessful;
        }

        /// <summary>
        /// Generate a password reset token, a guid is sufficient
        /// </summary>
        private static string CreatePasswordResetToken()
        {
            return Guid.NewGuid().ToString().ToLower().Replace("-", "");
        }
    }
}