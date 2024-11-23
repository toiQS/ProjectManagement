using Microsoft.EntityFrameworkCore;
using PM.Domain;
using PM.Persistence.Context;
using PM.Persistence.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Persistence.Services
{
    public class PositionInProjectServices : IPositionInProjectServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<PostitionInProject> _repository;
        public PositionInProjectServices(ApplicationDbContext context, IRepository<PostitionInProject> repository)
        {
            _context = context;
            _repository = repository;
        }
        public async Task<bool> AddAsync(PostitionInProject postitionInProjectDTO)
        {
            return await _repository.AddAsync(postitionInProjectDTO);
        }
        public Task<bool> DeleteAsync(string Id)
        {
            return _repository.DeleteAsync(Id);
        }
        public async Task<bool> UpdateAsync(string Id, PostitionInProject postitionInProjectDTO)
        {
            return await _repository.UpdateAsync(Id, postitionInProjectDTO);
        }
        public Task<IEnumerable<PostitionInProject>> GetAllAsync()
        {
            return _repository.GetAllAsync();
        }
        public Task<PostitionInProject> GetPostitionInProjectById(string Id)
        {
            return _repository.GetValueAsync(Id);
        }
        public async Task<IEnumerable<PostitionInProject>> GetAllPositionInProjectByProjectId(string projectId)
        {
            try
            {
                return await _context.PostitionInProject.Where(x => x.ProjectId == projectId).ToArrayAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Enumerable.Empty<PostitionInProject>();
            }
        }
    }
}
