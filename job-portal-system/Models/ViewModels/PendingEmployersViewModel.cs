using job_portal_system.Models.Domain;
using System.Collections.Generic;

namespace job_portal_system.Models.ViewModels
{
    public class PendingEmployersViewModel
    {
        public IEnumerable<Employer> Employers { get; set; } = new List<Employer>();
        public IEnumerable<JobSeeker> JobSeekers { get; set; } = new List<JobSeeker>();
        public int EmployerPage { get; set; }
        public int JobSeekerPage { get; set; }
        public int PageSize { get; set; }
        public int TotalEmployers { get; set; }
        public int TotalJobSeekers { get; set; }
        
        public int EmployerTotalPages => (int)Math.Ceiling((double)TotalEmployers / PageSize);
        public int JobSeekerTotalPages => (int)Math.Ceiling((double)TotalJobSeekers / PageSize);
    }
}
