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

    }
}
