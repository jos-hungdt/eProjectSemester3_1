using eProjectSemester3_1.Application.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eProjectSemester3_1.Areas.Admin.ViewModels
{
    public class AdminContactViewModel
    {
        public List<Contact> ListContact { get; set; }
        public AdminPageingViewModel Paging { get; set; }
    }

    public class AdminEditContactViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public string Address { get; set; }
        public int Status { get; set; }

        [AllowHtml]
        public string Note { get; set; }
        public DateTime CreateDate { get; set; }
    }
}