using PM.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Persistence.IServices
{
    public interface IPositionInProjectServices
    {
        public Task<bool> AddAsync(PostitionInProject postitionInProjectDTO);
        public Task<bool> DeleteAsync(string Id);
        public Task<bool> UpdateAsync(string Id, PostitionInProject postitionInProjectDTO);
        public Task<IEnumerable<PostitionInProject>> GetAllAsync();
        public Task<PostitionInProject> GetPostitionInProjectById(string Id);
        public Task<IEnumerable<PostitionInProject>> GetAllPositionInProjectByProjectId(string Id);
    }
}
