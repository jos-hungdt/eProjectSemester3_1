using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eProjectSemester3_1.ViewModels
{
    public class LogOnViewModel
    {
        public string ReturnUrl { get; set; }

        [Required]
        [DisplayName("Username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; }

        [DisplayName("RememberMe")]
        public bool RememberMe { get; set; }
    }

    public class MemberAddViewModel
    {
        public string ReturnUrl { get; set; }

        [DisplayName("UserName")]
        [Required]
        public string UserName { get; set; }
        
        [DisplayName("Password")]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayName("RePassword")]
        [Required]
        [DataType(DataType.Password)]
        public string RePassword { get; set; }

        [DisplayName("Your Email")]
        public string Email { get; set; }
        [DisplayName("Your Name")]
        public string FullName { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class RegisterShopViewModel
    {
        public string UserName { get; set; }
        public string FullName { get; set; }

        public string ShopName { get; set; }
        public string ShopPhone { get; set; }
        public string ShopAddress { get; set; }

    }
}