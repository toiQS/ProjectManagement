using Azure.Core;
using Microsoft.AspNetCore.Hosting;
using PM.Domain;
using PM.DomainServices.Models;
using PM.DomainServices.Models.plans;
using PM.DomainServices.Models.tasks;
using PM.Persistence.IServices;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace PM.DomainServices.Logic
{
    public class TaskLogic
    {

        //intialize services
        private readonly ITaskServices _taskServices;
        private readonly ITaskInPlanServices _taskInPlanServices;
        private readonly IStatusServices _statusServices;
        private readonly IMemberInTaskServices _memberInTaskServices;
        //intialize logic

        //intialize primary value
        private List<TaskDTO> _tasks;
        private List<TaskInPlan> _taskInPlan;
        private List<Status> _statuses;
        private List<MemberInTask> _memberInTask;

        public TaskLogic(ITaskServices taskServices, ITaskInPlanServices taskInPlanServices, IStatusServices statusServices, IMemberInTaskServices memberInTaskServices)
        {
            _taskServices = taskServices;
            _taskInPlanServices = taskInPlanServices;
            _statusServices = statusServices;
            _memberInTaskServices = memberInTaskServices;
            Intialize();
        }

        #region private method
        private async Task<IEnumerable<TaskDTO>> GetTasks()
        {
            var task = await _taskServices.GetAllAsync();
            if (task.Status == false) return null;
            return task.Data;
        }
        private async Task<IEnumerable<TaskInPlan>> GetTaskInPlans()
        {
            var task = await _taskInPlanServices.GetAllAsync();
            if (task.Status == false) return null;
            return task.Data;
        }
        private void Intialize()
        {
            _tasks = GetTasks().GetAwaiter().GetResult().ToList();
            _taskInPlan = GetTaskInPlans().GetAwaiter().GetResult().ToList();

        }
        private async Task<ServicesResult<string>> GetStatusInfo(int statusId)
        {
            if (statusId == 0) return ServicesResult<string>.Failure("");


            var getInfo = _statuses.Where(x => x.Id == statusId).FirstOrDefault();
            if (getInfo == null) return ServicesResult<string>.Failure($"can't get any this status {statusId}");
            return ServicesResult<string>.Success(getInfo.Value, string.Empty);
        }
        private async Task<ServicesResult<IEnumerable<MemberInTask>>> GetAllMembersTodoTask()
        {
            var member = await _memberInTaskServices.GetAllAsync();
            if(member.Status == false) return ServicesResult<IEnumerable<MemberInTask>>.Failure(member.Message);
            return ServicesResult<IEnumerable<MemberInTask>>.Success(member.Data, string.Empty);
        }
        #endregion


        #region suport method
        public async Task<ServicesResult<IEnumerable<IndexTask>>> GetTasksInPlan(string planId)
        {
            if (string.IsNullOrEmpty(planId)) return ServicesResult<IEnumerable<IndexTask>>.Failure("plan id is request");
            var taskPlan = _taskInPlan.Where(x => x.PlanId == planId).ToList();
            var tasks = new List<IndexTask>();
            
            if (!tasks.Any()) return ServicesResult<IEnumerable<IndexTask>>.Success(null, "no task in this plan");
            foreach (var task in taskPlan)
            {
                var item = await _taskServices.GetValueByPrimaryKeyAsync(task.TaskId);
                if(item.Data == null || item.Status == false) return ServicesResult<IEnumerable<IndexTask>>.Failure(item.Message);
                var status = await GetStatusInfo(item.Data.StatusId);
                if(status.Status == false) return ServicesResult<IEnumerable<IndexTask>>.Failure(status.Message);
                var index = new IndexTask()
                {
                    TaskName = item.Data.TaskName,
                    Status = status.Data,
                    TaskId = task.TaskId,
                };
                tasks.Add(index);
            }
            return ServicesResult<IEnumerable<IndexTask>>.Success(tasks, string.Empty);
        }

        #endregion


        #region primary method
        public async Task<ServicesResult<DetailTask>> GetDetailInfoTask(string taskId)
        {
            if (string.IsNullOrEmpty(taskId)) return ServicesResult<DetailTask>.Failure("task id is request");
            var item = await _taskServices.GetValueByPrimaryKeyAsync(taskId);
            if (item.Data == null || item.Status == false) return ServicesResult<DetailTask>.Failure(item.Message);
            var status = await GetStatusInfo(item.Data.StatusId);
            if (status.Status == false) return ServicesResult<DetailTask>.Failure(status.Message);
            var index = new DetailTask()
            {
                TaskName = item.Data.TaskName,
                CreateAt = item.Data.CreateAt,
                EndAt = item.Data.EndAt,
                IsDone = item.Data.IsDone,
                StartAt = item.Data.StartAt,
                Status = status.Data,
                TaskId = taskId,

            };
            return ServicesResult<DetailTask>.Success(index, string.Empty);
        }
        #endregion
    }
}
