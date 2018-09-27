using System;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

using System.Data.Entity;

using Main.Models;


namespace Main.Common {
    internal class AddDbContext : DbContext {
        public static string DebugConnectionString { get; set; }
        public static string ReleaseConnectionString { get; set; }

#if DEBUG
        public AddDbContext() : base("Main.Properties.Settings.DebugConnectionString") {

            //Database.SetInitializer<AddDbContext>(new DropCreateDatabaseIfModelChanges<AddDbContext>());
            //Database.SetInitializer<AddDbContext>(new CreateDatabaseIfNotExists<AddDbContext>());
            //Database.SetInitializer<AddDbContext>(new DropCreateDatabaseAlways<AddDbContext>());
        }
#else
        public AddDbContext() : base("Main.Properties.Settings.ReleaseConnectionString") { 
            Database.SetInitializer<DbContext>(new CreateDatabaseIfNotExists<DbContext>()); 
        }
#endif

        public override async Task<int> SaveChangesAsync() {
            var now = DateTime.UtcNow;

            var changes = ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);

            foreach (var entry in changes) {
                if (entry.Property("Update").CurrentValue == null) {
                    entry.Property("Update").CurrentValue = now;
                }
            }

            return await base.SaveChangesAsync();
        }

        public DbSet<Member> M_Members { get; set; }
        public DbSet<Authority> M_Authorities { get; set; }
    }
}
