using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    public class MatkaContext : DbContext
    {
        public MatkaContext(DbContextOptions<MatkaContext> options)
            : base(options)
        { 
        }
        public DbSet<Matkaaja> Matkaajat { get; set; }
        public DbSet<Matka> Matkat { get; set; }
        public DbSet<Tarina> Tarinat { get; set; }
        public DbSet<Kuva> Kuvat { get; set; }
        public DbSet<Matkakohde> Matkakohteet { get; set; }
    }
}
