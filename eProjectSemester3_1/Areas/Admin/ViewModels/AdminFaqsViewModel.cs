using eProjectSemester3_1.Application.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eProjectSemester3_1.Areas.Admin.ViewModels
{
    public class AdminFaqsViewModel
    {
        public List<Faqs> ListFaqs { get; set; }
        public AdminPageingViewModel Paging { get; set; }
    }

    public class AdminFaqsEditViewModel
    {
        public int Id { get; set; }

        [Required]
        public string question { get; set; }
        public string answer { get; set; }
    }
}