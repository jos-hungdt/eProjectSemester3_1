namespace eProjectSemester3_1.Application.Mapping
{
    using eProjectSemester3_1.Application.Entities;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    using System.Linq;
    using System.Web;

    public class EstateMapping : EntityTypeConfiguration<Estate>
    {
        public EstateMapping()
        {
            Property(e => e.Price)
                .HasPrecision(19, 4);

            Property(e => e.Area)
                .HasPrecision(18, 0);

            HasRequired(x => x.EstateStyle).WithOptional().Map(m => m.MapKey("EstateStyle_Id")).WillCascadeOnDelete(false);
            HasRequired(x => x.EstateType).WithOptional().Map(m => m.MapKey("EstateType_Id")).WillCascadeOnDelete(false);
            HasRequired(x => x.PostType).WithOptional().Map(m => m.MapKey("PostEstateType_Id")).WillCascadeOnDelete(false);

            HasOptional(x => x.User).WithOptionalDependent().Map(m => m.MapKey("User_Id")).WillCascadeOnDelete(false);
        }
    }
}