using GKHNNC.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace GKHNNC.DAL
{
    public class WorkContext : DbContext
    {
        
            public WorkContext() : base("WorkContext")
            {
            }

        //public DbSet<Student> Students { get; set; }
        // public DbSet<Enrollment> Enrollments { get; set; }
        // public DbSet<Course> Courses { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Ezdka> Ezdkas { get; set; }
        public DbSet<Zapravka> Zapravkas { get; set; }
        public DbSet<Work> Works { get; set; }
        public DbSet<GEU> GEUs { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Sopostavlenie> Sopostavlenies { get; set; }
        public DbSet<Agent> Agents { get; set; }
        public DbSet<CompleteWork> CompleteWorks { get; set; }
        public DbSet<Periodichnost> Periodichnosts { get; set; }
        public DbSet<Usluga> Usluga { get; set; }
        public DbSet<VipolnennieUslugi> VipolnennieUslugis { get; set; }
        public DbSet<AutoScan> AutoScans { get; set; }
        public DbSet<AutoScansSverka> AutoScansSverkas { get; set; }
        public DbSet<MarkaAvtomobil> MarkaAvtomobils { get; set; }
        public DbSet<TypeAvto> TypeAvtos { get; set; }
        public DbSet<Avtomobil> Avtomobils { get; set; }
        public DbSet<TableService> TableServices { get; set; }     
        public System.Data.Entity.DbSet<GKHNNC.Models.Adres> Adres { get; set; }
        public System.Data.Entity.DbSet<GKHNNC.Models.SVN> SVNs { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.UEV> UEVs { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.OBSD> OBSDs { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.IPU> IPUs { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.OPU> OPUs { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.Arendator> Arendators { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.House> Houses { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.Negilaya> Negilayas { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.DOMFundament> DOMFundaments { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.FundamentMaterial> FundamentMaterials { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.FundamentType> FundamentTypes { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.RoofForm> RoofForms { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.RoofType> RoofTypes { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.RoofVid> RoofVids { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.RoofUteplenie> RoofUteplenies { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.DOMRoof> DOMRoofs { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.FasadType> FasadTypes { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.FasadMaterial> FasadMaterials { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.FasadUteplenie> FasadUteplenies { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.DOMFasad> DOMFasads { get; set; }


        //protected override void OnModelCreating(DbModelBuilder modelBuilder)

        //{
        //        modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

        //}


    }
}