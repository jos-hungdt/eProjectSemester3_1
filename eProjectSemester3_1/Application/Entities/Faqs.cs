using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eProjectSemester3_1.Application.Entities
{
    public class Faqs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int faqID { get; set; }

        public string question { get; set; }

        public string answer { get; set; }
    }
}