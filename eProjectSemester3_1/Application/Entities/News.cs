using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eProjectSemester3_1.Application.Entities
{
    public class News : Entity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string ShortContent { get; set; }
        public DateTime EditDate { get; set; }
        public DateTime CreateDate { get; set; }
        public virtual MembershipUser UserPost { get; set; }
        public virtual MembershipUser UserEdit { get; set; }
        public virtual IList<Post> Posts { get; set; }
        public virtual Category Category { get; set; }
    }
}