using GKHNNC.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace GKHNNC.DAL
{
    public class Gladiators : DbContext
    {
            public Gladiators() : base("GladiatorsContext")
            {
            }

        public DbSet<ChatMessage> ChatMessages { get; set; }
    }
}