using Microsoft.EntityFrameworkCore;

namespace MinimalApiPeliculas
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }


    }
}
