using ProfileMgrApp.Models;
using System.Linq;

namespace ProfileMgrApp.Data
{
    /// <summary>
    /// Utility class to seed data for EmployeeProfileDb
    /// </summary>
    public class SeedEmployeeProfiles
    {
        public static void Initialize(EmployeeProfileDbContext context)
        {
            // Look for any movies.
            if (context.EmployeeProfiles.Any())
            {
                return;   // DB has been seeded
            }

            context.EmployeeProfiles.AddRange(
                new EmployeeProfile
                {
                    FirstName = "Armen",
                    LastName = "Romo",
                    Title = "Software Engineer",
                    Department = "Engineering"
                },

                new EmployeeProfile
                {
                    FirstName = "Corinne",
                    LastName = "Horn",
                    Title = "Business Analyst",
                    Department = "Human Resources"
                },

                new EmployeeProfile
                {
                    FirstName = "Dan",
                    LastName = "Drayton",
                    Title = "System Administrator",
                    Department = "IT"
                },

                new EmployeeProfile
                {
                    FirstName = "Joann",
                    LastName = "Chambers",
                    Title = "Senior Accoutant",
                    Department = "Finance"
                }
            );
            context.SaveChanges();
        }
    }
}
