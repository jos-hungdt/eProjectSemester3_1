namespace eProjectSemester3_1.Application.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class MembershipUser : Entity
    {
        [Key]
        public int Id { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }

        public string Avatar { get; set; }
        public string IdentityCard { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }

        public string ShopName { get; set; }
        public string ShopPhone { get; set; }
        public string ShopAddress { get; set; }

        public bool IsApproved { get; set; }
        public bool IsLockedOut { get; set; }
        public bool IsBanned { get; set; }

        public int FailedPasswordAttemptCount { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime LastLoginDate { get; set; }
        public DateTime LastPasswordChangedDate { get; set; }
        public DateTime LastLockoutDate { get; set; }
        public DateTime? LastActivityDate { get; set; }

        public virtual IList<MembershipRole> Roles { get; set; }
        public virtual IList<News> News { get; set; }
        public virtual IList<Post> Posts { get; set; }
        public virtual Banks Banks { get; set; }
    }
}
