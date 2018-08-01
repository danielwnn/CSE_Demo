﻿using Microsoft.EntityFrameworkCore;
using ProfileManagerMvcApp01.Models;

namespace ProfileManagerMvcApp01.Data
{
    /// <summary>
    /// Employee Pofile DB Context class for database operations
    /// </summary>
    public class EmployeeProfileDbContext : DbContext
    {
        public EmployeeProfileDbContext(DbContextOptions<EmployeeProfileDbContext> options)  : base(options)
        {
        }

        public DbSet<EmployeeProfile> EmployeeProfiles { get; set; }
    }
}
