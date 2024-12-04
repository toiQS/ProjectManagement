using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using PM.Domain;
using PM.DomainServices.ILogic;
using PM.Persistence.IServices;
using Shared;
using System.Dynamic;
using System.Numerics;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace PM.DomainServices.Logic
{
    public class TaskLogic : ITaskLogic
    {
        #region Fields and Constructor

        private readonly ITaskServices _taskServices;
        private readonly ITaskInPlanServices _taskInPlanServices;
        private readonly IMemberInTaskServices _memberInTaskServices;
        private readonly IApplicationUserServices _applicationUserServices;
        private readonly IPositionInProjectServices _positionInProjectServices;
        private readonly IPositionWorkOfMemberServices _positionWorkOfMemberServices;
        private readonly IRoleApplicationUserInProjectServices _roleApplicationUserInProjectServices;
        private readonly IRoleInProjectServices _roleInProjectServices;

        public TaskLogic(
            ITaskServices taskServices,
            ITaskInPlanServices taskInPlanServices,
            IMemberInTaskServices memberInTaskServices,
            IApplicationUserServices applicationUserServices,
            IPositionInProjectServices positionInProjectServices,
            IPositionWorkOfMemberServices positionWorkOfMemberServices,
            IRoleApplicationUserInProjectServices roleApplicationUserInProjectServices,
            IRoleInProjectServices roleInProjectServices)
        {
            _taskServices = taskServices;
            _taskInPlanServices = taskInPlanServices;
            _memberInTaskServices = memberInTaskServices;
            _applicationUserServices = applicationUserServices;
            _positionInProjectServices = positionInProjectServices;
            _positionWorkOfMemberServices = positionWorkOfMemberServices;
            _roleApplicationUserInProjectServices = roleApplicationUserInProjectServices;
            _roleInProjectServices = roleInProjectServices;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets tasks associated with a specific plan.
        /// </summary>
        public async Task<ServicesResult<IEnumerable<TaskIndex>>> GetTasksInPlan(string planId)
        {
            if (string.IsNullOrEmpty(planId))
                return ServicesResult<IEnumerable<TaskIndex>>.Failure("Plan ID is required.");

            var taskPlans = (await _taskInPlanServices.GetAllAsync()).Where(x => x.PlanId == planId);
            if (!taskPlans.Any())
                return ServicesResult<IEnumerable<TaskIndex>>.Failure("No tasks found in the plan.");

            var data = new List<TaskIndex>();
            foreach (var taskPlan in taskPlans)
            {
                var task = (await _taskServices.GetAllAsync()).FirstOrDefault(x => x.Id == taskPlan.TaskId);
                if (task == null)
                    return ServicesResult<IEnumerable<TaskIndex>>.Failure("Task not found.");

                var taskIndex = new TaskIndex
                {
                    Id = task.Id,
                    StartAt = task.StartAt,
                    EndAt = task.EndAt,
                    TaskStatus = MapTaskStatus(task.Status)
                };

                data.Add(taskIndex);
            }

            return ServicesResult<IEnumerable<TaskIndex>>.Success(data);
        }

        /// <summary>
        /// Gets detailed information about a task.
        /// </summary>
        public async Task<ServicesResult<TaskDetail>> GetTaskDetail(string taskId)
        {
            if (string.IsNullOrEmpty(taskId))
                return ServicesResult<TaskDetail>.Failure("Task ID is required.");

            var task = (await _taskServices.GetAllAsync()).FirstOrDefault(x => x.Id == taskId);
            if (task == null)
                return ServicesResult<TaskDetail>.Failure("Task not found.");

            var taskDetail = new TaskDetail
            {
                Id = taskId,
                TaskName = task.TaskName,
                CreateAt = task.CreateAt,
                StartAt = task.StartAt,
                EndAt = task.EndAt,
                TaskDescription = task.TaskDescription,
                TaskStatus = MapTaskStatus(task.Status),
                Members = new List<MemberIndex>()
            };

            var membersInTask = (await _memberInTaskServices.GetAllAsync()).Where(x => x.TaskId == taskId);
            foreach (var member in membersInTask)
            {
                var positionWork = (await _positionWorkOfMemberServices.GetAllAsync()).FirstOrDefault(x => x.Id == member.PositionWorkOfMemberId);
                if (positionWork == null) continue;

                var position = (await _positionInProjectServices.GetAllAsync()).FirstOrDefault(x => x.Id == positionWork.PostitionInProjectId);
                var roleUser = (await _roleApplicationUserInProjectServices.GetAllAsync()).FirstOrDefault(r => r.Id == positionWork.RoleApplicationUserInProjectId);
                var user = await _applicationUserServices.GetUser(roleUser?.ApplicationUserId);

                if (position != null && user != null)
                {
                    taskDetail.Members.Add(new MemberIndex
                    {
                        Id = roleUser.Id,
                        MemberName = user.UserName,
                        PositionNameInProject = position.PositionName,
                        MemberImage = user.PathImage,
                        PositionWorkId = member.PositionWorkOfMemberId
                    });
                }
            }

            return ServicesResult<TaskDetail>.Success(taskDetail);
        }

        /// <summary>
        /// Adds a new task and associates it with a plan.
        /// </summary>
        public async Task<ServicesResult<bool>> Add(string userId, NewTask newTask, string planId)
        {
            if (string.IsNullOrWhiteSpace(userId) || newTask == null || string.IsNullOrEmpty(planId))
                return ServicesResult<bool>.Failure("Invalid input.");

            if ((await _taskServices.GetAllAsync()).Any(x => x.TaskName.Equals(newTask.TaskName, StringComparison.OrdinalIgnoreCase)))
                return ServicesResult<bool>.Failure("Task name already exists.");

            var random = new Random().Next(1000000, 9999999);
            var task = new TaskDTO
            {
                Id = $"1009-{random}-{DateTime.Now.Ticks}",
                TaskName = newTask.TaskName,
                CreateAt = DateTime.Now,
                StartAt = newTask.StartAt,
                EndAt = newTask.EndAt,
                TaskDescription = newTask.TaskDescription,
                Status = newTask.StartAt > DateTime.Now ? 2 : 3
            };

            foreach (var member in newTask.Members)
            {
                var memberInTask = new MemberInTask
                {
                    Id = $"1010-{random}-{DateTime.Now.Ticks}",
                    PositionWorkOfMemberId = member.PositionWorkId,
                    TaskId = task.Id
                };
                if (!await _memberInTaskServices.AddAsync(memberInTask))
                    return ServicesResult<bool>.Failure("Failed to add member to task.");
            }

            if (!await _taskServices.AddAsync(task))
                return ServicesResult<bool>.Failure("Failed to add task.");

            var taskInPlan = new TaskInPlan
            {
                Id = $"1008-{random}-{DateTime.Now.Ticks}",
                PlanId = planId,
                TaskId = task.Id
            };

            if (!await _taskInPlanServices.AddAsync(taskInPlan))
                return ServicesResult<bool>.Failure("Failed to associate task with plan.");

            return ServicesResult<bool>.Success(true);
        }

        /// <summary>
        /// Deletes a task, ensuring proper permissions and cascading deletions.
        /// </summary>
        public async Task<ServicesResult<bool>> Delete(string userId, string taskId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(taskId))
                return ServicesResult<bool>.Failure("User ID and Task ID are required.");

            var roleUser = (await _roleApplicationUserInProjectServices.GetAllAsync())
                .FirstOrDefault(x => x.ApplicationUserId == userId);

            if (roleUser == null)
                return ServicesResult<bool>.Failure("User role not found.");

            var roleProject = (await _roleInProjectServices.GetAllAsync())
                .FirstOrDefault(x => x.Id == roleUser.RoleInProjectId);

            if (roleProject == null || roleProject.RoleName != "Owner")
                return ServicesResult<bool>.Failure("User lacks permission to delete the task.");

            var taskInPlan = (await _taskInPlanServices.GetAllAsync()).FirstOrDefault(x => x.TaskId == taskId);
            var membersInTask = (await _memberInTaskServices.GetAllAsync()).Where(x => x.TaskId == taskId);

            foreach (var member in membersInTask)
            {
                if (!await _memberInTaskServices.DeleteAsync(member.Id))
                    return ServicesResult<bool>.Failure("Failed to delete a task member.");
            }

            if (taskInPlan != null && !await _taskInPlanServices.DeleteAsync(taskInPlan.Id))
                return ServicesResult<bool>.Failure("Failed to delete task from plan.");

            if (!await _taskServices.DeleteAsync(taskId))
                return ServicesResult<bool>.Failure("Failed to delete the task.");

            return ServicesResult<bool>.Success(true);
        }

        /// <summary>
        /// Updates a task and its members, ensuring proper permissions.
        /// </summary>
        public async Task<ServicesResult<bool>> Update(string userId, string taskId, TaskUpdate taskUpdate)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(taskId) || taskUpdate == null)
                return ServicesResult<bool>.Failure("Invalid input.");

            var roleUser = (await _roleApplicationUserInProjectServices.GetAllAsync())
                .FirstOrDefault(x => x.ApplicationUserId == userId);

            if (roleUser == null)
                return ServicesResult<bool>.Failure("User role not found.");

            var roleProject = (await _roleInProjectServices.GetAllAsync())
                .FirstOrDefault(x => x.Id == roleUser.RoleInProjectId);

            if (roleProject == null || roleProject.RoleName != "Owner")
                return ServicesResult<bool>.Failure("User lacks permission to update the task.");

            var task = (await _taskServices.GetAllAsync()).FirstOrDefault(x => x.Id == taskId);
            if (task == null)
                return ServicesResult<bool>.Failure("Task not found.");

            // Update task properties
            task.TaskName = taskUpdate.TaskName;
            task.TaskDescription = taskUpdate.TaskDescription;
            task.StartAt = taskUpdate.StartAt;
            task.EndAt = taskUpdate.EndAt;

            if (taskUpdate.StartAt > DateTime.Now) task.Status = 2; // Waiting
            if (taskUpdate.StartAt <= DateTime.Now) task.Status = 3; // Processing

            var existingMembers = (await _memberInTaskServices.GetAllAsync()).Where(x => x.TaskId == taskId).ToList();
            foreach (var member in existingMembers)
            {
                if (!await _memberInTaskServices.DeleteAsync(member.Id))
                    return ServicesResult<bool>.Failure("Failed to remove a member from the task.");
            }

            foreach (var newMember in taskUpdate.Members)
            {
                var random = new Random().Next(1000000, 9999999);
                var memberInTask = new MemberInTask
                {
                    Id = $"1010-{random}-{DateTime.Now.Ticks}",
                    TaskId = taskId,
                    PositionWorkOfMemberId = newMember.PositionWorkId
                };

                if (!await _memberInTaskServices.AddAsync(memberInTask))
                    return ServicesResult<bool>.Failure("Failed to add a new member to the task.");
            }

            if (!await _taskServices.UpdateAsync(taskId, task))
                return ServicesResult<bool>.Failure("Failed to update the task.");

            return ServicesResult<bool>.Success(true);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Maps task status integer to a string representation.
        /// </summary>
        private static string MapTaskStatus(int status) => status switch
        {
            1 => "Node",
            2 => "Waiting",
            3 => "Processing",
            4 => "Completed",
            _ => string.Empty
        };

        #endregion
    }
}
