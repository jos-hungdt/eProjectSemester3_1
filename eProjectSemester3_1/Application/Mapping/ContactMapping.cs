namespace eProjectSemester3_1.Application.Mapping
{
    using eProjectSemester3_1.Application.Entities;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    using System.Linq;
    using System.Web;

    public class ContactMapping : EntityTypeConfiguration<Contact>
    {
        public ContactMapping()
        {
            // PRIMARY KEY identity
            HasKey(x => x.Id);
            Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            //VARCHAR(50) NOT NULL
            Property(x => x.Name).IsRequired().HasMaxLength(50);
            //VARCHAR(20) NOT NULL
            Property(x => x.Phone).IsRequired().HasMaxLength(20);
            //VARCHAR(50) NOT NULL
            Property(x => x.Email).IsRequired().HasMaxLength(50);
            
            Property(x => x.Message).IsRequired(); //NOT NULL
            Property(x => x.Status).IsRequired(); //NOT NULL
            Property(x => x.Note).IsOptional(); // NULL
        }
    }
}