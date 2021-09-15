using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eProjectSemester3_1.Application.Entities
{
    public class Post : Entity
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public bool isStart { get; set; }
        public DateTime EditDate { get; set; }
        public DateTime CreateDate { get; set; }
        public virtual MembershipUser UserPost { get; set; }
        public virtual MembershipUser UserEdit { get; set; }
        public virtual News News { get; set; }


    }
}