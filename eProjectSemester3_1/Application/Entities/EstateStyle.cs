using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eProjectSemester3_1.Application.Entities
{
    public class EstateStyle : Entity
    {
        [Key]
        public int realEstateStyleID { get; set; }

        [StringLength(50)]
        public string realEstateStyleName { get; set; }

        public int? realEstateStyleStatus { get; set; }
        
    }
}