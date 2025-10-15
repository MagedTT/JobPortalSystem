using AutoMapper;
using job_portal_system.Models.Domain;
using job_portal_system.Models.DTOs;
using job_portal_system.Repositories.Interfaces;
using job_portal_system.Services.Interfaces;

namespace job_portal_system.Services.Implementations
{
    public class JobService : IJobService
    {
        private readonly IJobRepository _repo;
        private readonly IMapper _mapper;

        public JobService(IJobRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<JobDto>> GetAllJobsAsync()
        {
            var jobs = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<JobDto>>(jobs);
        }

        public async Task<JobDto?> GetJobByIdAsync(string id)
        {
            var job = await _repo.GetByIdAsync(id);
            return _mapper.Map<JobDto?>(job);
        }

        public async Task<IEnumerable<JobDto>> GetRecentJobsAsync(int count)
        {
            var jobs = await _repo.GetRecentJobsAsync(count);
            return _mapper.Map<IEnumerable<JobDto>>(jobs);
        }

        public async Task AddJobAsync(JobDto dto)
        {
            var job = _mapper.Map<Job>(dto);
            await _repo.AddAsync(job);
            await _repo.SaveAsync();
        }

        public async Task UpdateJobAsync(JobDto dto)
        {
            var job = _mapper.Map<Job>(dto);
            _repo.Update(job);
            await _repo.SaveAsync();
        }

        public async Task DeleteJobAsync(string id)
        {
            var job = await _repo.GetByIdAsync(id);
            if (job != null)
            {
                _repo.Delete(job);
                await _repo.SaveAsync();
            }
        }
    }
}
