using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using PM.Domain;
using PM.Persistence.IServices;
using Shared;
using Shared.member;
using Shared.position;
using Shared.task;
using System;
using System.Net.Sockets;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Threading.Tasks;

namespace PM.DomainServices.Logic
{
    public class TaskLogic
    {
        private readonly IApplicationUserServices _userServices;
        private readonly ITaskInPlanServices _taskInPlanServices;
        private readonly ITaskServices _taskServices;
        private readonly IMemberInTaskServices _memberServices;
        private readonly IPositionInProjectServices _positionServices;
        private readonly IRoleApplicationUserInProjectServices _roleServices;
        private readonly IRoleInProjectServices _roleServicesInProjectServices;
        private readonly IPositionWorkOfMemberServices _positionWorkOfMemberServices;
        private readonly IPlanServices _planServices;
        private readonly IProjectServices _projectServices;
        private readonly IPlanInProjectServices _planInProjectServices;
        private readonly IStatusServices _statusServices;
        
        public async Task<ServicesResult<IEnumerable<IndexTask>>> GetTaskListInPlan(string userId, string planId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(planId)) return ServicesResult<IEnumerable<IndexTask>>.Failure("");
            var getProjectId = (await _planInProjectServices.GetAllAsync()).FirstOrDefault(x => x.PlanId == planId).ProjectId;
            if (getProjectId == null) return ServicesResult<IEnumerable<IndexTask>>.Failure("");
            if((await _userServices.GetUser(userId)) == null || !(await _roleServices.GetAllAsync()).Any(s => s.ApplicationUserId == userId && s.ProjectId == getProjectId)) return ServicesResult<IEnumerable<IndexTask>>.Failure("");
            var getTaskIDs = (await _taskInPlanServices.GetAllAsync()).Where(x => x.PlanId == planId);
            if (!getTaskIDs.Any()) return ServicesResult<IEnumerable<IndexTask>>.Failure("");
            var data = new List<IndexTask>();
            foreach (var task in getTaskIDs)
            {
                var getTask = await _taskServices.GetValueByPrimaryKeyAsync(task.Id);
                if (getTask == null) return ServicesResult<IEnumerable<IndexTask>>.Failure("");
                var value = new IndexTask()
                {
                    TaskId = getTask.Id,
                    TaskName = getTask.TaskName,
                    Status = (await _statusServices.GetAllAsync()).FirstOrDefault(x => x.Id == getTask.StatusId).Value,
                };
            }
            return ServicesResult<IEnumerable<IndexTask>>.Success(data);
        }
        public async Task<ServicesResult<DetailTask>> GetTaskDetail(string userId, string taskId)
        {
            if (string.IsNullOrEmpty(taskId) || string.IsNullOrEmpty(userId)) return ServicesResult<DetailTask>.Failure("");
            var planId = (await _taskInPlanServices.GetAllAsync()).FirstOrDefault(x => x.TaskId == taskId).PlanId;
            if (planId == null) return ServicesResult<DetailTask>.Failure("");
            var getProjectId = (await _planInProjectServices.GetAllAsync()).FirstOrDefault(x => x.PlanId == planId).ProjectId;
            if (getProjectId == null) return ServicesResult<DetailTask>.Failure("");
            if ((await _userServices.GetUser(userId)) == null || !(await _roleServices.GetAllAsync()).Any(s => s.ApplicationUserId == userId && s.ProjectId == getProjectId)) 
                return ServicesResult<DetailTask>.Failure("");
            var task = await _taskServices.GetValueByPrimaryKeyAsync(taskId);
            if (task == null) return ServicesResult<DetailTask>.Failure("");
            var data = new DetailTask()
            {
                TaskId = taskId,
                TaskName = task.TaskName,
                CreateAt = task.CreateAt,
                EndAt = task.EndAt,
                IsDone = task.IsDone,
                Status = (await _statusServices.GetAllAsync()).FirstOrDefault(x => x.Id == task.StatusId).Value,
                StartAt = task.StartAt,
                IndexMembers = new List<Shared.member.IndexMember>()
            };
            var getMemberInTasks = (await _memberServices.GetAllAsync()).Where(x => x.TaskId == taskId);
            if (!getMemberInTasks.Any()) return ServicesResult<DetailTask>.Failure("");
            foreach (var member in getMemberInTasks)
            {
                var positonWork = (await _positionWorkOfMemberServices.GetValueByPrimaryKeyAsync(member.PositionWorkOfMemberId));
                if (positonWork == null) return ServicesResult<DetailTask>.Failure("");
                var getRoleUserInProject = await _roleServices.GetValueByPrimaryKeyAsync(positonWork.RoleApplicationUserInProjectId);
                if (getRoleUserInProject == null) return ServicesResult<DetailTask>.Failure("");
                var user = await _userServices.GetUser(getRoleUserInProject.ApplicationUserId);
                if (user == null) return ServicesResult<DetailTask>.Failure("");
                var value = new IndexMember()
                {
                    RoleUserInProjectId = getRoleUserInProject.Id,
                    PositionWorkName = (await _positionServices.GetValueByPrimaryKeyAsync(positonWork.PostitionInProjectId)).PositionName,
                    UserName = user.UserName,
                    UserAvata = user.PathImage,
                };
                data.IndexMembers.Add(value);
            }
            return ServicesResult<DetailTask>.Success(data);    
        }
        public async Task<ServicesResult<bool>> Add(string userId, AddTask addTask, string planId)
        {
            if (userId == null || addTask == null || string.IsNullOrEmpty(planId)) return ServicesResult<bool>.Failure("");
            var getProjectId = (await _planInProjectServices.GetAllAsync()).FirstOrDefault(x => x.PlanId == planId).ProjectId;
            if (getProjectId == null) return ServicesResult<bool>.Failure("");
            var getRoleOwnerId = (await _roleServicesInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Owner").Id;
            var getRoleLeaderId = (await _roleServicesInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Leader").Id;
            var getRoleManagerId = (await _roleServicesInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Manager").Id;
            if((await _userServices.GetUser(userId)) ==null || !(await _roleServices.GetAllAsync())
                .Any(x => x.ApplicationUserId == userId && x.ProjectId == getProjectId && ( x.RoleInProjectId == getRoleLeaderId || x.RoleInProjectId == getRoleManagerId || x.RoleInProjectId == getRoleOwnerId)))
                return ServicesResult<bool>.Failure("");
            var getTaskInPlans = (await _taskInPlanServices.GetAllAsync()).Where(x => x.PlanId == planId); 
            if (!getTaskInPlans.Any())
            {
                return await AddMethodHelper(addTask, planId);
            }
            foreach (var task in getTaskInPlans)
            {
                if((await _taskServices.GetValueByPrimaryKeyAsync(task.TaskId)).TaskName == addTask.TaskName) return ServicesResult<bool>.Failure("");
            }
            return await AddMethodHelper(addTask, planId);
        }
        private async  Task<ServicesResult<bool>> AddMethodHelper(AddTask addTask, string planId)
        {
            var random = new Random().Next(1000000, 9999999);
            var task = new TaskDTO()
            {
                Id = $"1009-{random}-{DateTime.Now}",
                TaskName = addTask.TaskName,
                CreateAt = DateTime.Now,
                TaskDescription = addTask.Description,
                IsDone = false,
                EndAt = addTask.EndAt,
                StartAt = addTask.StartAt               
            };
            if (DateTime.Now == addTask.StartAt) task.StatusId = 3;
            if (DateTime.Now < addTask.StartAt) task.StatusId = 2;
            if(!await _taskServices.AddAsync(task)) return ServicesResult<bool>.Failure("");
            var taskPlan = new TaskInPlan()
            {
                Id = $"1008-{random}-{DateTime.Now}",
                PlanId = planId,
                TaskId = task.Id,
            };
            if(!(await _taskInPlanServices.AddAsync(taskPlan))) return ServicesResult<bool>.Failure("");
            foreach (var member in addTask.IndexMembers)
            {
                var value = new MemberInTask()
                {
                    Id = $"1008-{random}-{DateTime.Now}",
                    PositionWorkOfMemberId = (await _positionServices.GetAllAsync()).FirstOrDefault(x => x.PositionName == member.PositionWorkName).PositionName,
                    TaskId = task.Id
                }; 
                if (!(await _memberServices.AddAsync(value))) return ServicesResult<bool>.Failure("");
            }
            return ServicesResult<bool>.Success(true);
        }
        public async Task<ServicesResult<bool>> UpdateInfo(string userId, string taskId, UpdateTask updateTask)
        {
            if(string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(taskId) || updateTask == null) 
                return ServicesResult<bool>.Failure("");
            var planId = (await _taskInPlanServices.GetAllAsync()).FirstOrDefault(x => x.TaskId == taskId).PlanId;
            if (planId == null) return ServicesResult<bool>.Failure("");
            var getProjectId = (await _planInProjectServices.GetAllAsync()).FirstOrDefault(x => x.PlanId == planId).ProjectId;
            if (getProjectId == null) return ServicesResult<bool>.Failure("");
            var getRoleOwnerId = (await _roleServicesInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Owner").Id;
            var getRoleLeaderId = (await _roleServicesInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Leader").Id;
            var getRoleManagerId = (await _roleServicesInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Manager").Id;
            if ((await _userServices.GetUser(userId)) == null 
                || !(await _roleServices.GetAllAsync())
                .Any(x => x.ApplicationUserId == userId 
                && x.ProjectId == getProjectId 
                && (x .RoleInProjectId == getRoleLeaderId || x.RoleInProjectId == getRoleManagerId || x.RoleInProjectId == getRoleOwnerId)))
                return ServicesResult<bool>.Failure("");
            var getTaskInPlans = (await _taskInPlanServices.GetAllAsync()).Where(x => x.PlanId == planId);
            if (!getTaskInPlans.Any()) return await UpdateTaskMethod(taskId, updateTask);
            foreach (var task in getTaskInPlans)
            {
                if ((await _taskServices.GetValueByPrimaryKeyAsync(task.TaskId)).TaskName == updateTask.TaskName) return ServicesResult<bool>.Failure("");
            }
            return await UpdateTaskMethod( planId,updateTask);
        }
        private async Task<ServicesResult<bool>> UpdateTaskMethod(string taskId, UpdateTask updateTask)
        {
            var random = new Random().Next(1000000, 9999999);
            var getTask = await _taskServices.GetValueByPrimaryKeyAsync(taskId);
            if (getTask == null) return ServicesResult<bool>.Failure("");
            getTask.TaskName = updateTask.TaskName;
            getTask.TaskDescription = updateTask.TaskDescription;
            if (updateTask.Members.Any())
            {
                var memberOld = (await _memberServices.GetAllAsync()).Where(x => x.TaskId == taskId);
                if (!memberOld.Any())
                {

                    foreach (var member in updateTask.Members)
                    {
                        var value = new MemberInTask()
                        {
                            Id = $"1008-{random}-{DateTime.Now}",
                            PositionWorkOfMemberId = (await _positionServices.GetAllAsync()).FirstOrDefault(x => x.PositionName == member.PositionWorkName).PositionName,
                            TaskId = taskId,
                        };
                        if (!(await _memberServices.AddAsync(value))) return ServicesResult<bool>.Failure("");
                    }
                }
                else
                {
                    foreach (var member in memberOld)
                    {
                        if (!await _memberServices.DeleteAsync(member.Id)) return ServicesResult<bool>.Failure("");
                    }
                    foreach (var member in updateTask.Members)
                    {
                        var value = new MemberInTask()
                        {
                            Id = $"1008-{random}-{DateTime.Now}",
                            PositionWorkOfMemberId = (await _positionServices.GetAllAsync()).FirstOrDefault(x => x.PositionName == member.PositionWorkName).PositionName,
                            TaskId = taskId,
                        };
                        if (!(await _memberServices.AddAsync(value))) return ServicesResult<bool>.Failure("");
                    }
                }
            }
            else
            {
                var memberOld = (await _memberServices.GetAllAsync()).Where(x => x.TaskId == taskId);
                foreach (var member in memberOld)
                {
                    if (!await _memberServices.DeleteAsync(member.Id)) return ServicesResult<bool>.Failure("");
                }
            }
            if((await _taskServices.UpdateAsync(taskId, getTask))) return ServicesResult<bool>.Success(true);
            return ServicesResult<bool>.Failure("");

        }
        public async Task<ServicesResult<bool>> Delete(string userId, string taskId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(taskId))
                return ServicesResult<bool>.Failure("");
            var planId = (await _taskInPlanServices.GetAllAsync()).FirstOrDefault(x => x.TaskId == taskId).PlanId;
            if (planId == null) return ServicesResult<bool>.Failure("");
            var getProjectId = (await _planInProjectServices.GetAllAsync()).FirstOrDefault(x => x.PlanId == planId).ProjectId;
            if (getProjectId == null) return ServicesResult<bool>.Failure("");
            var getRoleOwnerId = (await _roleServicesInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Owner").Id;
            var getRoleLeaderId = (await _roleServicesInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Leader").Id;
            var getRoleManagerId = (await _roleServicesInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Manager").Id;
            if ((await _userServices.GetUser(userId)) == null
                || !(await _roleServices.GetAllAsync())
                .Any(x => x.ApplicationUserId == userId
                && x.ProjectId == getProjectId
                && (x.RoleInProjectId == getRoleLeaderId || x.RoleInProjectId == getRoleManagerId || x.RoleInProjectId == getRoleOwnerId)))
                return ServicesResult<bool>.Failure("");
            var getTask = await _taskServices.GetValueByPrimaryKeyAsync(taskId);
            var memberTask = (await _memberServices.GetAllAsync()).Where(x => x.TaskId == taskId);
            if (memberTask.Any())
            {
                foreach (var member in memberTask)
                {
                    if (!await _memberServices.DeleteAsync(member.Id)) return ServicesResult<bool>.Failure("");
                }
                if (await _taskServices.DeleteAsync(taskId)) return ServicesResult<bool>.Success(true);
                return ServicesResult<bool>.Failure("");
            }
            else
            {
                if (await _taskServices.DeleteAsync(taskId)) return ServicesResult<bool>.Success(true);
                return ServicesResult<bool>.Failure("");
            }
        }
        public async Task<ServicesResult<bool>> UpdateStatus(string userId, string taskId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(taskId))
                return ServicesResult<bool>.Failure("");
            var planId = (await _taskInPlanServices.GetAllAsync()).FirstOrDefault(x => x.TaskId == taskId).PlanId;
            if (planId == null) return ServicesResult<bool>.Failure("");
            var getProjectId = (await _planInProjectServices.GetAllAsync()).FirstOrDefault(x => x.PlanId == planId).ProjectId;
            if (getProjectId == null) return ServicesResult<bool>.Failure("");
            var getRoleOwnerId = (await _roleServicesInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Owner").Id;
            var getRoleLeaderId = (await _roleServicesInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Leader").Id;
            var getRoleManagerId = (await _roleServicesInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Manager").Id;
            if ((await _userServices.GetUser(userId)) == null
                || !(await _roleServices.GetAllAsync())
                .Any(x => x.ApplicationUserId == userId
                && x.ProjectId == getProjectId
                && (x.RoleInProjectId == getRoleLeaderId || x.RoleInProjectId == getRoleManagerId || x.RoleInProjectId == getRoleOwnerId)))
                return ServicesResult<bool>.Failure("");
            var getTask = await _taskServices.GetValueByPrimaryKeyAsync(taskId);
            if (getTask == null) return ServicesResult<bool>.Failure("");
            if (getTask.IsDone == false && getTask.StartAt < DateTime.Now && DateTime.Now < getTask.EndAt) getTask.StatusId = 3;
            if (getTask.IsDone == false && getTask.StartAt < DateTime.Now && DateTime.Now > getTask.EndAt) getTask.StatusId = 6;
            if (getTask.IsDone == true && getTask.StartAt < DateTime.Now && DateTime.Now > getTask.EndAt) getTask.StatusId = 7;
            if (getTask.IsDone == true && getTask.StartAt < DateTime.Now && DateTime.Now < getTask.EndAt) getTask.StatusId = 4;
            if (getTask.IsDone == true && getTask.StartAt < DateTime.Now && DateTime.Now == getTask.EndAt) getTask.StatusId = 5;


            if ((await _taskServices.UpdateAsync(taskId, getTask))) return ServicesResult<bool>.Success(true);
            return ServicesResult<bool>.Failure("");

        }
        public async Task<ServicesResult<bool>> UpdateIsDone(string userId, string taskId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(taskId))
                return ServicesResult<bool>.Failure("");
            var planId = (await _taskInPlanServices.GetAllAsync()).FirstOrDefault(x => x.TaskId == taskId).PlanId;
            if (planId == null) return ServicesResult<bool>.Failure("");
            var getProjectId = (await _planInProjectServices.GetAllAsync()).FirstOrDefault(x => x.PlanId == planId).ProjectId;
            if (getProjectId == null) return ServicesResult<bool>.Failure("");
            var getRoleOwnerId = (await _roleServicesInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Owner").Id;
            var getRoleLeaderId = (await _roleServicesInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Leader").Id;
            var getRoleManagerId = (await _roleServicesInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Manager").Id;
            if ((await _userServices.GetUser(userId)) == null
                || !(await _roleServices.GetAllAsync())
                .Any(x => x.ApplicationUserId == userId
                && x.ProjectId == getProjectId
                && (x.RoleInProjectId == getRoleLeaderId || x.RoleInProjectId == getRoleManagerId || x.RoleInProjectId == getRoleOwnerId)))
                return ServicesResult<bool>.Failure("");
            var getTask = await _taskServices.GetValueByPrimaryKeyAsync(taskId);
            getTask.IsDone = !getTask.IsDone;

            if (!(await _taskServices.UpdateAsync(taskId, getTask))) 
            return ServicesResult<bool>.Failure("");
            return await UpdateStatus(userId, taskId);
        }
    }
}
