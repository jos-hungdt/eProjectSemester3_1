using eProjectSemester3_1.Application.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;

namespace eProjectSemester3_1.Application.Context
{
    internal sealed class Configuration : DbMigrationsConfiguration<AppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(AppDbContext context)
        {
            #region Initial Installer Code
            // Create the admin role if it doesn't exist
            var adminRole = context.MembershipRole.FirstOrDefault(x => x.RoleName == AppConstants.AdminRoleName);
            if (adminRole == null)
            {
                adminRole = new MembershipRole { RoleName = AppConstants.AdminRoleName };
                context.MembershipRole.Add(adminRole);
                context.SaveChanges();
            }

            // Create the shopping role if it doesn't exist
            var shoppingRole = context.MembershipRole.FirstOrDefault(x => x.RoleName == AppConstants.ShoppingRoleName);
            if (shoppingRole == null)
            {
                shoppingRole = new MembershipRole { RoleName = AppConstants.ShoppingRoleName };
                context.MembershipRole.Add(shoppingRole);
                context.SaveChanges();
            }

            // If the admin user exists then don't do anything else
            if (context.MembershipUser.FirstOrDefault(x => x.UserName == AppConstants.AdminUserName) == null)
            {
                DateTime time = DateTime.Now;

                // create the admin user and put him in the admin role
                var admin = new MembershipUser // tự động tạo admin nếu không có
                {
                    UserName = AppConstants.AdminUserName,
                    Password = "123",
                    IsApproved = true,
                    CreateDate = time,
                    LastLockoutDate = (DateTime)SqlDateTime.MinValue,
                    LastPasswordChangedDate = (DateTime)SqlDateTime.MinValue,
                    LastLoginDate = time,
                    LastActivityDate = null,
                    IsLockedOut = false,
                    DateOfBirth = (DateTime)SqlDateTime.MinValue,
                };

                // Hash the password
                var salt = StringUtils.CreateSalt(AppConstants.SaltSize);
                var hash = StringUtils.GenerateSaltedHash(admin.Password, salt);
                admin.Password = hash;
                admin.PasswordSalt = salt;

                // Put the admin in the admin role
                admin.Roles = new List<MembershipRole> { adminRole };

                context.MembershipUser.Add(admin);
                context.SaveChanges();
            }


            // instart PostEstateType
            if (context.PostEstateType.Select(x => x).Any())
            {
                context.PostEstateType.Add(new PostEstateType
                {
                    postTypeID = 1,
                    postTypeName = "Normal Post",
                    postTypePrice = "0",
                    postTypeStatus = 1,
                });

                context.PostEstateType.Add(new PostEstateType
                {
                    postTypeID = 2,
                    postTypeName = "VIP Post",
                    postTypePrice = "0",
                    postTypeStatus = 1,
                });

            }


            #endregion
        }
    }
}