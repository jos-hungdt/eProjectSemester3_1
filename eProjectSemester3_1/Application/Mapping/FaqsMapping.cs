namespace eProjectSemester3_1.Application.Mapping
{
    using eProjectSemester3_1.Application.Entities;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    using System.Linq;
    using System.Web;

    public class FaqsMapping : EntityTypeConfiguration<Faqs>
    {
        public FaqsMapping()
        {
            // PRIMARY KEY identity
            HasKey(x => x.faqID);
            Property(x => x.faqID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.question).IsRequired().HasMaxLength(500);
            Property(x => x.answer).IsRequired();
        }
    }
}