using GKHNNC.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace GKHNNC.DAL
{
    public class WorkContext : DbContext
    {
        
            public WorkContext() : base("WorkContext")
            {
            Database.SetInitializer<WorkContext>(null);
        }

        //public DbSet<Student> Students { get; set; }
        // public DbSet<Enrollment> Enrollments { get; set; }
        // public DbSet<Course> Courses { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Ezdka> Ezdkas { get; set; }
        public DbSet<Zapravka> Zapravkas { get; set; }
        public DbSet<Work> Works { get; set; }
        public DbSet<GKHNNC.Models.GEU> GEUs { get; set; }
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
        public DbSet<AS24> AS24 { get; set; }


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

        public System.Data.Entity.DbSet<GKHNNC.Models.RoomDoor> RoomDoors { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.RoomOverlap> RoomOverlaps { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.RoomType> RoomTypes { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.RoomWindow> RoomWindows { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.DOMRoom> DOMRooms { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.ASControl> ASControls { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.Voditel> Voditels { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.Zakazchik> Zakazchiks { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.Mechanic> Mechanics { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.Tarif> Tarifs { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.Musor> Musors { get; set; }
        public System.Data.Entity.DbSet<GKHNNC.Models.Defect> Defects { get; set; }
        public System.Data.Entity.DbSet<GKHNNC.Models.Element> Elements { get; set; }
        public System.Data.Entity.DbSet<GKHNNC.Models.DefDescription> DefDescriptions { get; set; }
        public System.Data.Entity.DbSet<GKHNNC.Models.DefWork> DefWorks { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.Material> Materials { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.DOMCW> DOMCWs { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.DOMHW> DOMHWs { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.DOMElectro> DOMElectroes { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.DOMVodootvod> DOMVodootvods { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.DOMOtoplenie> DOMOtoplenies { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.ActiveDefect> ActiveDefects { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.Osmotr> Osmotrs { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.ActiveElement> ActiveElements { get; set; }
        public System.Data.Entity.DbSet<GKHNNC.Models.Iskluchit> Iskluchits { get; set; }
        public System.Data.Entity.DbSet<GKHNNC.Models.DOMPart> DOMParts { get; set; }
        public System.Data.Entity.DbSet<GKHNNC.Models.Izmerenie> Izmerenies { get; set; }
        public System.Data.Entity.DbSet<GKHNNC.Models.Build> Builds { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.KontrAgent> KontrAgents { get; set; }
        public System.Data.Entity.DbSet<GKHNNC.Models.Street> Streets { get; set; }
        public System.Data.Entity.DbSet<GKHNNC.Models.AllStreet> AllStreets { get; set; }
        public System.Data.Entity.DbSet<GKHNNC.Models.MusorPloshadka> MusorPloshadkas { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.BuildElement> BuildElements { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.Poligon> Poligons { get; set; }
        public System.Data.Entity.DbSet<GKHNNC.Models.Marshrut> Marshruts { get; set; }
        public DbSet<MarshrutsALL> MarshrutsAlls { get; set; }
        public DbSet<ContainersType> ContainersTypes { get; set; }
        public DbSet<MusorPloshadkaActive> MusorPloshadkaActives { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.OsmotrWork> OsmotrWorks { get; set; }
        public System.Data.Entity.DbSet<GKHNNC.Models.OsmotrRecommendWork> OsmotrRecommendWorks { get; set; }
        public System.Data.Entity.DbSet<GKHNNC.Models.ActiveOsmotrWork> ActiveOsmotrWorks { get; set; }
        public System.Data.Entity.DbSet<GKHNNC.Models.LogAutoscan> LogAutoscans { get; set; }
        public System.Data.Entity.DbSet<GKHNNC.Models.EU> EU { get; set; }
        public System.Data.Entity.DbSet<GKHNNC.Models.AdresType> AdresTypes { get; set; }
        public System.Data.Entity.DbSet<GKHNNC.Models.EventLog> EventLogs { get; set; }
        public System.Data.Entity.DbSet<GKHNNC.Models.Stati> Statis { get; set; }
        public System.Data.Entity.DbSet<GKHNNC.Models.WorkSoderganie> WorkSoderganies { get; set; }
        public System.Data.Entity.DbSet<GKHNNC.Models.ActiveWorkSoderganie> ActiveWorkSoderganies { get; set; }
        public System.Data.Entity.DbSet<GKHNNC.Models.Tip> Tips { get; set; }
        public System.Data.Entity.DbSet<GKHNNC.Models.TechElement> TechElements { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.TypeElement> TypeElements { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.ConstructiveType> ConstructiveTypes { get; set; }
        public System.Data.Entity.DbSet<GKHNNC.Models.TelefonsSpravochnik> TelefonsSpravochniks { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.CanCreateOsmotr> CanCreateOsmotrs { get; set; }
        public System.Data.Entity.DbSet<GKHNNC.Models.WhiteList> WhiteLists { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.Group> Groups { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.Schetchik> Schetchiks { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.SchetchikTip> SchetchikTips { get; set; }
        public System.Data.Entity.DbSet<GKHNNC.Models.WorkStandart> WorkStandarts { get; set; }

        public System.Data.Entity.DbSet<GKHNNC.Models.MaterialStandart> MaterialStandasrts { get; set; }
        public System.Data.Entity.DbSet<GKHNNC.Models.MaterialToWorkStandart> MaterialToWorkStandarts { get; set; }




        //protected override void OnModelCreating(DbModelBuilder modelBuilder)

        //{
        //        modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

        //}


    }
}