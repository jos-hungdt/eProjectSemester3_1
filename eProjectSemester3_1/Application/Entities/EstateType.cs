using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eProjectSemester3_1.Application.Entities
{
    public class EstateType : Entity
    {
        [Key]
        public int realStateTypeID { get; set; }

        [StringLength(50)]
        public string realStateTypeName { get; set; }

        public int? realStateTypeStatus { get; set; }
       
    }
}