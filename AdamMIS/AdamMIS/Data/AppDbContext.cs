


using AdamMIS.Entities.MetaBase;
using AdamMIS.Entities.Messaging;
using AdamMIS.Entities.ReportsEnitites;
using AdamMIS.Entities.SystemLogs;
using AdamMIS.Entities.UserEntities;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace AdamMIS.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser , ApplicationRole,string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        public DbSet<RCategories> RCategories { get; set; }
        public DbSet<Reports> Reports { get; set; }
        public DbSet<UserReports> UserReports { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<ApplicationUserRole> ApplicationUserRole { get; set; }
        public DbSet<SystemLog> SystemLog { get; set; }

        public DbSet<AcivityLogs> acivityLogs { get; set; }

        public DbSet<Metabase> Metabases { get; set; }
        public DbSet<UsersMetabases> UsersMetabases { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }

        public DbSet<Messagee> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var cascadeFKs = modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetForeignKeys()).
                Where(fk => fk.DeleteBehavior == DeleteBehavior.Cascade && !fk.IsOwnership);
            foreach (var fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());


            modelBuilder.Entity<Messagee>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => new { e.SenderId, e.RecipientId, e.SentAt })
                      .HasDatabaseName("IX_Messages_Conversation");

                entity.HasIndex(e => e.RecipientId)
                      .HasDatabaseName("IX_Messages_Recipient");
            });
        }
    }
}
