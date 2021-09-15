using eProjectSemester3_1.Application.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eProjectSemester3_1.ViewModels
{
    public class NewsListViewModel
    {
        public List<News> ListNews { get; set; }
        public PageingViewModel Paging { get; set; }
    }

    public class NewsShowViewModel {
        public News News { get; set; }
        public Post Post { get; set; }
        public List<Post> ListPost { get; set; }
        public PageingViewModel Paging { get; set; }
    }

    public class NewsCreateEditViewModel
    {
        public int Id { get; set; }

        [DisplayName("Title")]
        [Required]
        public string Title { get; set; }

        [DisplayName("Content")]
        [Required]
        public string Content { get; set; }
    }
}