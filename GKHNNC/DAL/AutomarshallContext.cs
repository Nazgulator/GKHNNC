using GKHNNC.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace GKHNNC.DAL
{
    public class AutomarshallContext : DbContext
    {

        public AutomarshallContext() : base("AutomarshallContext")
        {

    
                //disable initializer
                Database.SetInitializer<AutomarshallContext>(null);
            
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //DONT DO THIS ANYMORE
            //base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<Vote>().ToTable("Votes")


            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();


        }

        //public DbSet<Student> Students { get; set; }
        // public DbSet<Enrollment> Enrollments { get; set; }
        // public DbSet<Course> Courses { get; set; }

        public DbSet<AutomarshallView> AutomarshallViews { get; set; }
        public DbSet<VehicleRegistrationLog> VehicleRegistrationLog { get; set; }
        


        //protected override void OnModelCreating(DbModelBuilder modelBuilder)

        //{
        //        modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

        //}


    }
}