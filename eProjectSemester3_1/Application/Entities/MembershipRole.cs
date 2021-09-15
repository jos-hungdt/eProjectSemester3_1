namespace eProjectSemester3_1.Application.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class MembershipRole : Entity
    {
        public int Id { get; set; }
        
        public string RoleName { get; set; }

        public virtual IList<MembershipUser> Users { get; set; }
    }
}
