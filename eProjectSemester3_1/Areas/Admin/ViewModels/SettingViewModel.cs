using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace eProjectSemester3_1.Areas.Admin.ViewModels
{
    public class SettingViewModel
    {
        [DisplayName("CompanyName")]
        public string CompanyName { get; set; }
        [DisplayName("Hotline")]
        public string Hotline { get; set; }
        [DisplayName("Email")]
        public string Email { get; set; }
        [DisplayName("Description")]
        public string Description { get; set; }
        [DisplayName("Address")]
        public string Address { get; set; }

    }
}