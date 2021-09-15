namespace eProjectSemester3_1.Application.Context
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using eProjectSemester3_1.Application.Entities;

    public partial class AppDbContext : DbContext
    {
        //public int rd;
        public AppDbContext()
            : base("name=AppDbContent")
        {
            Configuration.LazyLoadingEnabled = true;

            //Random random = new Random();
            //rd = random.Next();
        }
        
        public virtual DbSet<MembershipRole> MembershipRole { get; set; }
        public virtual DbSet<MembershipUser> MembershipUser { get; set; }
        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<Post> Post { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Contact> Contact { get; set; }
        public virtual DbSet<Setting> Setting { get; set; }

        public virtual DbSet<Estate> Estate { get; set; }
        public virtual DbSet<EstateStyle> EstateStyle { get; set; }
        public virtual DbSet<EstateType> EstateType { get; set; }
        public virtual DbSet<PostEstateType> PostEstateType { get; set; }
        public virtual DbSet<Banks> Banks { get; set; }
        public virtual DbSet<BankLoan> BankLoan { get; set; }
        public virtual DbSet<Faqs> Faqs { get; set; }

    }
}

