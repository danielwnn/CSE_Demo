using ProfileMgrApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProfileMgrApp.Data
{
    public class EmployeeProfileRepository : GenericRepository<EmployeeProfile, int>
    {
        private readonly EmployeeProfileDbContext _context;

        public EmployeeProfileRepository(EmployeeProfileDbContext context) : base(context)
        {
            _context = context;
        }

        public override ICollection<EmployeeProfile> List()
        {
            // only select the columns needed for the list page,
            // no need to fetch the big photo
            var result = _context.EmployeeProfiles.Select(
                x => new EmployeeProfile
                {
                    ID = x.ID,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Title = x.Title,
                    Department = x.Department,
                    PhotoType = x.PhotoType,
                    ThumbnailBase64 = x.ThumbnailBase64
                });

            return result.ToList();
        }
    }
}
