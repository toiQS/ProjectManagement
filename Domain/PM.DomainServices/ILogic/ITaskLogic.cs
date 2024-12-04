using Shared;

namespace PM.DomainServices.ILogic
{
    public interface ITaskLogic
    {
        // public Task<bool> Add(string userId, string taskName, DateTime startAt, DateTime endAt, List<int> memberIds);
        public Task<ServicesResult<IEnumerable<TaskIndex>>> GetTasksInPlan(string planId);
        public Task<ServicesResult<TaskDetail>> GetTaskDetail(string taskId);
        public Task<ServicesResult<bool>> Add(string userId, NewTask newTask, string planId);
        public Task<ServicesResult<bool>> Delete(string userId, string taskId);
        public Task<ServicesResult<bool>> Update(string userId, string taskId, TaskUpdate taskUpdate);
    }
}
