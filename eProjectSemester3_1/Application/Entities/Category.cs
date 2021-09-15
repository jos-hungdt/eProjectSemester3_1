using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eProjectSemester3_1.Application.Entities
{
    public class Category : Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreateDate { get; set; }
        public virtual Category ParentCategory { get; set; }
    }
}