namespace eProjectSemester3_1.Application.Mapping
{
    using eProjectSemester3_1.Application.Entities;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    using System.Linq;
    using System.Web;

    public class CategoryMapping : EntityTypeConfiguration<Category>
    {
        public CategoryMapping()
        {
            // PRIMARY KEY identity
            HasKey(x => x.Id);
            Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            
            //VARCHAR(256) NOT NULL
            Property(x => x.Name).IsRequired().HasMaxLength(256);
            //VARCHAR(500) NULL
            Property(x => x.Description).IsOptional().HasMaxLength(500);
            //VARCHAR(256) NULL
            Property(x => x.Image).IsOptional().HasMaxLength(256);

            Property(x => x.SortOrder).IsOptional();

            Property(x => x.CreateDate).IsRequired();

            HasOptional(x => x.ParentCategory).WithOptionalDependent().Map(m => m.MapKey("Catergory_Id")).WillCascadeOnDelete(false);
        }
    }
}