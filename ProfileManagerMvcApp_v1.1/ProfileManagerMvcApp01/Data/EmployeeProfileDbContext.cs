using Microsoft.EntityFrameworkCore;
using ProfileManagerMvcApp01.Models;

namespace ProfileManagerMvcApp01.Data
{
    public class EmployeeProfileDbContext : DbContext
    {
        public EmployeeProfileDbContext (DbContextOptions<EmployeeProfileDbContext> options)
            : base(options)
        {
        }

        public DbSet<EmployeeProfile> EmployeeProfiles { get; set; }
    }
}
