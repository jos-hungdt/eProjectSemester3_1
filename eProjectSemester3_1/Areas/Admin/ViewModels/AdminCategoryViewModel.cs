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
    public class AdminCategoryViewModel
    {
        public List<Category> ListUsers { get; set; }
        public AdminPageingViewModel Paging { get; set; }
        public string Search { get; set; }
    }

    public class AdminCategoryEditViewModel
    {
        public int Id { get; set; }
        [DisplayName("Title")]
        [Required]
        public string Name { get; set; }
        [DisplayName("Description")]
        public string Description { get; set; }
        [DisplayName("Image")]
        public string Image { get; set; }
        [DisplayName("SortOrder")]
        public int SortOrder { get; set; }


        [DisplayName("Parent Category")]
        public int? ParentCategory { get; set; }
        public List<SelectListItem> AllCategories { get; set; }

    }
    public class AdminCategoryDeleteViewModel
    {
        public int Id { get; set; }
        [DisplayName("Title")]
        public string Name { get; set; }        
    }
}