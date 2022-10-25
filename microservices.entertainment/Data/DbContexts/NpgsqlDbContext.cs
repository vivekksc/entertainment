using microservices.entertainment.Data.DbModels;
using Microsoft.EntityFrameworkCore;

namespace microservices.entertainment.Data.DbContexts
{
    public class NpgsqlDbContext : DbContext
    {
        public NpgsqlDbContext() { }

        public NpgsqlDbContext(DbContextOptions<NpgsqlDbContext> options) : base(options) { }

        public DbSet<Redemption> Redemptions { get; set; }
    }
}
