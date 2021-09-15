using eProjectSemester3_1.Application.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eProjectSemester3_1.Areas.Admin.ViewModels
{
    public class AdminMembersViewModel
    {
        public List<MembershipUser> ListUsers { get; set; }
        public AdminPageingViewModel Paging { get; set; }
        public string Search { get; set; }
    }

    public class AdminMembersChangeInfoViewModel
    {
        [DisplayName("UserName")]
        public string UserName { get; set; }

        [DisplayName("Email")]
        public string Email { get; set; }

        public List<MembershipRole> AllRoles { get; set; }
        public List<int> RolesId { get; set; }
    }

    public class AdminMembersChangePasswordViewModel
    {
        [DisplayName("UserName")]
        public string UserName { get; set; }

        [DisplayName("Password")]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayName("New Password")]
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DisplayName("Confirm new Password")]
        [Required]
        [DataType(DataType.Password)]
        public string ReNewPassword { get; set; }
    }

    public class AdminMembersNewPasswordViewModel
    {
        [DisplayName("UserName")]
        public string UserName { get; set; }
        
        [DisplayName("New Password")]
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DisplayName("Confirm new Password")]
        [Required]
        [DataType(DataType.Password)]
        public string ReNewPassword { get; set; }
    }

    public class AdminMembersCreateViewModel
    {
        [DisplayName("UserName")]
        [Required]
        public string UserName { get; set; }

        [DisplayName("Email")]
        public string Email { get; set; }

        [DisplayName("Password")]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayName("Confirm new Password")]
        [Required]
        [DataType(DataType.Password)]
        public string RePassword { get; set; }

        public List<MembershipRole> AllRoles { get; set; }
        public List<int> RolesId { get; set; }
    }
    public class AdminMembersDeleteViewModel
    {
        [DisplayName("UserName")]
        public string UserName { get; set; }
    }
}