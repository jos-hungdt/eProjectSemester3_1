using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eProjectSemester3_1.Application.Entities
{
    public class PostEstateType : Entity
    {
        [Key]
        public int postTypeID { get; set; }

        [StringLength(100)]
        public string postTypeName { get; set; }

        [StringLength(100)]
        public string postTypePrice { get; set; }

        public int? postTypeStatus { get; set; }
    }
}