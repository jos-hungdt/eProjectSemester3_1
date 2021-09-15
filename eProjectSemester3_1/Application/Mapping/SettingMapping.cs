using eProjectSemester3_1.Application.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace eProjectSemester3_1.Application.Mapping
{
    public class SettingMapping : EntityTypeConfiguration<Setting>
    {
        public SettingMapping()
        {
            // PRIMARY KEY
            HasKey(x => x.Key);

            // VARCHAR(max) NULL;
            Property(x => x.Value).IsOptional();
            
        }
    }
}