﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ajanvarausprojekti.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class aikapalauteEntities : DbContext
    {
        public aikapalauteEntities()
            : base("name=aikapalauteEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Ajat> Ajat { get; set; }
        public virtual DbSet<Kayttajatunnukset> Kayttajatunnukset { get; set; }
        public virtual DbSet<Kestot> Kestot { get; set; }
        public virtual DbSet<Opettajat> Opettajat { get; set; }
        public virtual DbSet<Palautetyypit> Palautetyypit { get; set; }
        public virtual DbSet<Palautteet> Palautteet { get; set; }
        public virtual DbSet<Varaukset> Varaukset { get; set; }
        public virtual DbSet<Yllapitooikeudet> Yllapitooikeudet { get; set; }
    
        public virtual int Poistarivit()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Poistarivit");
        }
    }
}