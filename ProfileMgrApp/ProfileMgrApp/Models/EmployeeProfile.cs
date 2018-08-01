using System.ComponentModel.DataAnnotations;

namespace ProfileMgrApp.Models
{
    public class EmployeeProfile
    {
        [Required]
        [Display(Name = "Employee ID")]
        public int ID { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Job Title")]
        public string Title { get; set; }

        [Required]
        public string Department { get; set; }

        public string PhotoType { get; set; }

        public string ThumbnailBase64 { get; set; }

        [Display(Name = "Photo")]
        public byte[] PhotoData { get; set; }
    }
}
