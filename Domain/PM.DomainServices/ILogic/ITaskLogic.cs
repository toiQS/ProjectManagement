using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.DomainServices.ILogic
{
    public interface ITaskLogic
    {
        public Task<bool> Add(string userId, string taskName, DateTime startAt, DateTime endAt, List<int> memberIds);
    }
}
