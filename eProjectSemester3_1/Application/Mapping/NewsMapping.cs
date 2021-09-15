using eProjectSemester3_1.Application.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace eProjectSemester3_1.Application.Mapping
{
    public class NewsMapping : EntityTypeConfiguration<News>
    {
        public NewsMapping()
        {
            // PRIMARY KEY identity
            HasKey(x => x.Id);
            Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);


            //VARCHAR(256) NOT NULL
            Property(x => x.Title).IsRequired().HasMaxLength(256);

            //VARCHAR(500) NULL
            Property(x => x.ShortContent).IsOptional().HasMaxLength(500);

            Property(x => x.CreateDate).IsRequired();
            Property(x => x.EditDate).IsOptional();

            HasRequired(x => x.UserPost).WithMany(x => x.News).Map(m => m.MapKey("UserPost_Id")).WillCascadeOnDelete(false);
            HasOptional(x => x.UserEdit).WithOptionalDependent().Map(m => m.MapKey("UserEdit_Id")).WillCascadeOnDelete(false);
            HasOptional(x => x.Category).WithOptionalDependent().Map(m => m.MapKey("Catergory_Id")).WillCascadeOnDelete(false);

            HasMany(x => x.Posts).WithRequired(x => x.News).Map(m => m.MapKey("News_Id")).WillCascadeOnDelete(false);
        }
    }
}