﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataAccess
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class MyDbEntities : DbContext
    {
        public MyDbEntities()
            : base("name=MyDbEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AdminPhoto> AdminPhotoes { get; set; }
        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<Celebrity> Celebrities { get; set; }
        public virtual DbSet<CelebrityPhoto> CelebrityPhotoes { get; set; }
        public virtual DbSet<CelebritySuggestion> CelebritySuggestions { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<RateSuggestion> RateSuggestions { get; set; }
        public virtual DbSet<Report> Reports { get; set; }
        public virtual DbSet<Search> Searches { get; set; }
        public virtual DbSet<UserPhoto> UserPhotoes { get; set; }
        public virtual DbSet<Login> Logins { get; set; }
        public virtual DbSet<a> a { get; set; }
        public virtual DbSet<CelebrityDataExtended> CelebrityDataExtendeds { get; set; }
        public virtual DbSet<LocationData> LocationDatas { get; set; }
        public virtual DbSet<reportedPerson> reportedPersons { get; set; }
        public virtual DbSet<SearchHistory> SearchHistories { get; set; }
        public virtual DbSet<UserDataExtended> UserDataExtendeds { get; set; }
        public virtual DbSet<UserData> UserDatas { get; set; }
        public virtual DbSet<UserDataExtended2> UserDataExtended2 { get; set; }
    }
}
