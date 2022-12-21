using Microsoft.EntityFrameworkCore;
using ValesFise_api.Modelos;

namespace ValesFise_api.Contexto
{
    public class AppNGCDbContext : DbContext
    {
        public AppNGCDbContext(DbContextOptions<AppNGCDbContext> options) : base(options)
        {

        }

        //Poner aqui los modelos 
        public DbSet<Dni> Dni { get; set; }
        public DbSet<Vale> Vale { get; set; }
    }
}
