using Microsoft.EntityFrameworkCore;
using SecureSite.Models;

namespace SecureSite.Data
{
    public class SecureSiteContext : DbContext
    {
        public SecureSiteContext(DbContextOptions<SecureSiteContext> options)
            : base(options)
        {
        }

        public DbSet<Comment> Comment { get; set; }
        public DbSet<SiteFile> SiteFile { get; set; }
    }
}