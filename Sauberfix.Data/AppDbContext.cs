using Microsoft.EntityFrameworkCore;

namespace Sauberfix.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Kunde> Kunden { get; set; }
        public DbSet<Mitarbeiter> Mitarbeiter { get; set; }
        public DbSet<Termin> Termine { get; set; }
        
        // Neu hinzugefügt für 3NF
        public DbSet<Ort> Orte { get; set; }
    }
}
