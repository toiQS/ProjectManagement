using PM.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Persistence.IServices
{
    public interface IPositionInProjectServices
    {
        public Task<bool> AddAsync(PostitionInProjectDTO postitionInProjectDTO);
        public Task<bool> DeleteAsync(string Id);
        public Task<bool> UpdateAsync(string Id, PostitionInProjectDTO postitionInProjectDTO);
        public Task<IEnumerable<PostitionInProjectDTO>> GetAllAsync();
        public Task<PostitionInProjectDTO> GetPostitionInProjectById(string Id);
        public Task<IEnumerable<PostitionInProjectDTO>> GetAllPositionInProjectByProjectId(string Id);
    }
}
