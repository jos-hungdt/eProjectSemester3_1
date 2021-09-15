using eProjectSemester3_1.Application.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Data.Entity.Infrastructure.Annotations;

namespace eProjectSemester3_1.Application.Mapping
{
    public class MembershipUserMapping : EntityTypeConfiguration<MembershipUser>
    {
        public MembershipUserMapping()
        {
            // PRIMARY KEY identity
            HasKey(x => x.Id);
            Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            // NVARCHAR(256) NOT NULL UNIQUE
            //Property(x => x.UserName).IsRequired().HasMaxLength(256).HasDatabaseGeneratedOption(DatabaseGeneratedOption.u);
            Property(x => x.UserName).IsRequired().HasMaxLength(256).HasColumnAnnotation("Index",
                    new IndexAnnotation(new IndexAttribute("IX_MembershipUser_UserName", 1) { IsUnique = true }));

            // VARCHAR(256) NOT NULL 
            Property(x => x.Password).IsRequired().HasMaxLength(256);

            //VARCHAR(256) NULL
            Property(x => x.PasswordSalt).IsOptional().HasMaxLength(256);

            Property(x => x.Avatar).IsOptional().HasMaxLength(500);//VARCHAR(500) NULL
            Property(x => x.IdentityCard).IsOptional().HasMaxLength(30);//VARCHAR(30) NULL
            Property(x => x.Phone).IsOptional().HasMaxLength(20);//VARCHAR(20) NULL
            Property(x => x.FullName).IsOptional().HasMaxLength(256);//VARCHAR(256) NULL
            Property(x => x.Email).IsOptional().HasMaxLength(256);//VARCHAR(256) NULL
            Property(x => x.Address).IsOptional().HasMaxLength(500);//VARCHAR(500) NULL

            Property(x => x.IsApproved).IsRequired(); // NOT NULL
            Property(x => x.IsLockedOut).IsRequired();// NOT NULL
            Property(x => x.IsBanned).IsRequired();// NOT NULL

            Property(x => x.FailedPasswordAttemptCount).IsRequired();// NOT NULL

            Property(x => x.CreateDate).IsRequired();// NOT NULL
            Property(x => x.LastLoginDate).IsRequired();// NOT NULL
            Property(x => x.LastPasswordChangedDate).IsRequired();// NOT NULL
            Property(x => x.LastLockoutDate).IsRequired();// NOT NULL
            Property(x => x.LastActivityDate).IsOptional(); // NULL

            HasOptional(x => x.Banks).WithOptionalDependent().Map(m => m.MapKey("Banks_Id")).WillCascadeOnDelete(false);

            // Create table MembershipUsersInRoles
            // UserIdentifier PROGENKEY MembershipUser.Id
            // RoleIdentifier PROGENKEY MembershipRole.Id
            HasMany(x => x.Roles).WithMany(t => t.Users).Map(m =>
            {
                m.ToTable("MembershipUsersInRoles");
                m.MapLeftKey("UserIdentifier");
                m.MapRightKey("RoleIdentifier");
            });
        }
    }
}