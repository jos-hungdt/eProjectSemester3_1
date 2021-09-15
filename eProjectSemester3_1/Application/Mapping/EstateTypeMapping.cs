namespace eProjectSemester3_1.Application.Mapping
{
    using eProjectSemester3_1.Application.Entities;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    using System.Linq;
    using System.Web;

    public class EstateTypeMapping : EntityTypeConfiguration<EstateType>
    {
        public EstateTypeMapping()
        {
        }
    }
}