using AutoMapper;
using job_portal_system.Models.DTOs;
using job_portal_system.Models.ViewModels;
using job_portal_system.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace job_portal_system.Controllers
{
    public class JobController : Controller
    {
        private readonly IJobService _jobService;
        private readonly IMapper _mapper;

        public JobController(IJobService jobService, IMapper mapper)
        {
            _jobService = jobService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var jobDtos = await _jobService.GetAllJobsAsync();
            var viewModels = _mapper.Map<IEnumerable<JobViewModel>>(jobDtos);
            return View(viewModels);
        }

        public async Task<IActionResult> Details(string id)
        {
            var jobDto = await _jobService.GetJobByIdAsync(id);
            if (jobDto == null)
                return NotFound();

            var viewModel = _mapper.Map<JobViewModel>(jobDto);
            return View(viewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JobViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var dto = _mapper.Map<JobDto>(vm);
            await _jobService.AddJobAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var jobDto = await _jobService.GetJobByIdAsync(id);
            if (jobDto == null)
                return NotFound();

            var vm = _mapper.Map<JobViewModel>(jobDto);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(JobViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var dto = _mapper.Map<JobDto>(vm);
            await _jobService.UpdateJobAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            var jobDto = await _jobService.GetJobByIdAsync(id);
            if (jobDto == null)
                return NotFound();

            var vm = _mapper.Map<JobViewModel>(jobDto);
            return View(vm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _jobService.DeleteJobAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> RecentJobs(int count = 5)
        {
            var jobDtos = await _jobService.GetRecentJobsAsync(count);
            var vms = _mapper.Map<IEnumerable<JobViewModel>>(jobDtos);
            return PartialView("_RecentJobsPartial", vms);
        }

    }
}
