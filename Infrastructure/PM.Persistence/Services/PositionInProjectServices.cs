using Microsoft.EntityFrameworkCore;
using PM.Domain.DTOs;
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
        private readonly IRepository<PostitionInProjectDTO> _repository;
        public PositionInProjectServices(ApplicationDbContext context, IRepository<PostitionInProjectDTO> repository)
        {
            _context = context;
            _repository = repository;
        }
        public async Task<bool> AddAsync(PostitionInProjectDTO postitionInProjectDTO)
        {
            return await _repository.AddAsync(postitionInProjectDTO);
        }
        public Task<bool> DeleteAsync(string Id)
        {
            return _repository.DeleteAsync(Id);
        }
        public async Task<bool> UpdateAsync(string Id, PostitionInProjectDTO postitionInProjectDTO)
        {
            return await _repository.UpdateAsync(Id, postitionInProjectDTO);
        }
        public Task<IEnumerable<PostitionInProjectDTO>> GetAllAsync()
        {
            return _repository.GetAllAsync();
        }
        public Task<PostitionInProjectDTO> GetPostitionInProjectById(string Id)
        {
            return _repository.GetValueAsync(Id);
        }
        public async Task<IEnumerable<PostitionInProjectDTO>> GetAllPositionInProjectByProjectId(string projectId)
        {
            try
            {
                return await _context.PostitionInProject.Where(x => x.ProjectId == projectId).ToArrayAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Enumerable.Empty<PostitionInProjectDTO>();
            }
        }
    }
}
