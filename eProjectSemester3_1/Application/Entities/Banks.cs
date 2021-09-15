using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eProjectSemester3_1.Application.Entities
{
    public class Banks : Entity
    {
        [Key]
        public int BankID { get; set; }

        [StringLength(100)]
        public string BankName { get; set; }

        [StringLength(50)]
        public string BankPhone { get; set; }

        public int? BankStatus { get; set; }
    }
}