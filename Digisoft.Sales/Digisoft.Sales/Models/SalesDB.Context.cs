﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Digisoft.Sales.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class SalesEntities : DbContext
    {
        public SalesEntities()
            : base("name=SalesEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<AppliedUnder> AppliedUnders { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Developer> Developers { get; set; }
        public virtual DbSet<Platform> Platforms { get; set; }
        public virtual DbSet<ProjectType> ProjectTypes { get; set; }
        public virtual DbSet<TeamLead> TeamLeads { get; set; }
        public virtual DbSet<Technology> Technologies { get; set; }
        public virtual DbSet<ErrorLog> ErrorLogs { get; set; }
        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<Billing> Billings { get; set; }
        public virtual DbSet<Bidding> Biddings { get; set; }
        public virtual DbSet<UserDetail> UserDetails { get; set; }
    }
}