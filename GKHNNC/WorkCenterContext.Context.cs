﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GKHNNC
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ContextEntities : DbContext
    {
        public ContextEntities()
            : base("name=ContextEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AutoScans> AutoScans { get; set; }
        public virtual DbSet<Avtomobils> Avtomobils { get; set; }
        public virtual DbSet<Ezdkas> Ezdkas { get; set; }
        public virtual DbSet<MarkaAvtomobils> MarkaAvtomobils { get; set; }
        public virtual DbSet<TypeAvtoes> TypeAvtoes { get; set; }
    }
}