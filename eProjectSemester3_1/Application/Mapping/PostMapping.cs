using eProjectSemester3_1.Application.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace eProjectSemester3_1.Application.Mapping
{
    public class PostMapping : EntityTypeConfiguration<Post>
    {
        public PostMapping()
        {
            // PRIMARY KEY identity
            HasKey(x => x.Id);
            Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            //VARCHAR(500) NULL
            Property(x => x.Content).IsRequired().HasMaxLength(500);


            Property(x => x.CreateDate).IsRequired();
            Property(x => x.EditDate).IsOptional();

            HasRequired(x => x.UserPost).WithMany(x => x.Posts).Map(m => m.MapKey("UserPost_Id")).WillCascadeOnDelete(false);
            HasOptional(x => x.UserEdit).WithOptionalDependent().Map(m => m.MapKey("UserEdit_Id")).WillCascadeOnDelete(false);

            HasRequired(x => x.News).WithMany(x => x.Posts).Map(m => m.MapKey("News_Id")).WillCascadeOnDelete(false);

        }
    }
}