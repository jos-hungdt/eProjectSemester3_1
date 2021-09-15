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
    public class AdminNewsListViewModel
    {
        public List<News> ListNews { get; set; }
        public AdminPageingViewModel Paging { get; set; }
    }

    public class AdminNewsShowViewModel
    {
        public News News { get; set; }
        public Post Post { get; set; }
        public List<Post> ListPost { get; set; }
        public AdminPageingViewModel Paging { get; set; }
    }

    public class AdminNewsCreateEditViewModel
    {
        public int Id { get; set; }

        [DisplayName("Title")]
        [Required]
        public string Title { get; set; }

        [DisplayName("Category")]
        public int Category { get; set; }
        public List<SelectListItem> AllCategory { get; set; }
        [DisplayName("Image")]
        public string Image { get; set; }
        [DisplayName("Content")]
        [Required]
        [AllowHtml]
        public string Content { get; set; }
    }

    public class AdminNewsDeleteViewModel
    {
        public int Id { get; set; }

        [DisplayName("Title")]
        public string Title { get; set; }
    }
}