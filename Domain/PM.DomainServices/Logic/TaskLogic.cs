using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using PM.Domain;
using PM.DomainServices.ILogic;
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
    public class TaskLogic : ITaskLogic
    {
        // Define regions for better code organization
        #region Dependencies

        private readonly IApplicationUserServices _applicationUserServices;
        private readonly ITaskInPlanServices _taskInPlanServices;
        private readonly ITaskServices _taskServices;
        private readonly IMemberInTaskServices _memberInTaskServices;
        private readonly IPositionInProjectServices _positionInProjectServices;
        private readonly IRoleApplicationUserInProjectServices _roleApplicationUserInProjectServices;
        private readonly IRoleInProjectServices _roleInProjectServices;
        private readonly IPositionWorkOfMemberServices _positionWorkOfMemberServices;
        private readonly IPlanServices _planServices;
        private readonly IProjectServices _projectServices;
        private readonly IPlanInProjectServices _planInProjectServices;
        private readonly IStatusServices _statusServices;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor for TaskLogic class to inject required services.
        /// </summary>
        /// <param name="applicationUserServices">Service for managing application users.</param>
        /// <param name="taskInPlanServices">Service for managing tasks in plans.</param>
        /// <param name="taskServices">Service for managing tasks.</param>
        /// <param name="memberInTaskServices">Service for managing members in tasks.</param>
        /// <param name="positionInProjectServices">Service for managing positions in projects.</param>
        /// <param name="roleApplicationUserInProjectServices">Service for managing user roles in projects.</param>
        /// <param name="roleInProjectServices">Service for managing roles in projects.</param>
        /// <param name="positionWorkOfMemberServices">Service for managing work positions of members.</param>
        /// <param name="planServices">Service for managing plans.</param>
        /// <param name="projectServices">Service for managing projects.</param>
        /// <param name="planInProjectServices">Service for managing plans within projects.</param>
        /// <param name="statusServices">Service for managing task statuses.</param>
        public TaskLogic(
            IApplicationUserServices applicationUserServices,
            ITaskInPlanServices taskInPlanServices,
            ITaskServices taskServices,
            IMemberInTaskServices memberInTaskServices,
            IPositionInProjectServices positionInProjectServices,
            IRoleApplicationUserInProjectServices roleApplicationUserInProjectServices,
            IRoleInProjectServices roleInProjectServices,
            IPositionWorkOfMemberServices positionWorkOfMemberServices,
            IPlanServices planServices,
            IProjectServices projectServices,
            IPlanInProjectServices planInProjectServices,
            IStatusServices statusServices)
        {
            // Initialize all injected services
            _applicationUserServices = applicationUserServices;
            _taskInPlanServices = taskInPlanServices;
            _taskServices = taskServices;
            _memberInTaskServices = memberInTaskServices;
            _positionInProjectServices = positionInProjectServices;
            _roleApplicationUserInProjectServices = roleApplicationUserInProjectServices;
            _roleInProjectServices = roleInProjectServices;
            _positionWorkOfMemberServices = positionWorkOfMemberServices;
            _planServices = planServices;
            _projectServices = projectServices;
            _planInProjectServices = planInProjectServices;
            _statusServices = statusServices;
        }

        #endregion

        // Define regions for better readability and organization
        #region Get Task List in Plan

        /// <summary>
        /// Retrieves the list of tasks associated with a specific plan and user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="planId">The ID of the plan.</param>
        /// <returns>A list of tasks if found, or an error message if not.</returns>
        public async Task<ServicesResult<IEnumerable<IndexTask>>> GetTaskListInPlan(string userId, string planId)
        {
            // Check for invalid parameters
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(planId))
                return ServicesResult<IEnumerable<IndexTask>>.Failure("Invalid parameters.");

            // Retrieve the project ID associated with the plan
            var getProjectId = (await _planInProjectServices.GetAllAsync()).FirstOrDefault(x => x.PlanId == planId)?.ProjectId;
            if (getProjectId == null)
                return ServicesResult<IEnumerable<IndexTask>>.Failure("Project not found.");

            // Check if the user exists and has a valid role in the project
            if ((await _applicationUserServices.GetUser(userId)) == null ||
                !(await _roleApplicationUserInProjectServices.GetAllAsync()).Any(s => s.ApplicationUserId == userId && s.ProjectId == getProjectId))
                return ServicesResult<IEnumerable<IndexTask>>.Failure("User not authorized.");

            // Retrieve task IDs associated with the plan
            var getTaskIDs = (await _taskInPlanServices.GetAllAsync()).Where(x => x.PlanId == planId);
            if (!getTaskIDs.Any())
                return ServicesResult<IEnumerable<IndexTask>>.Failure("No tasks found.");

            var data = new List<IndexTask>();

            // Retrieve task details for each task in the plan
            foreach (var task in getTaskIDs)
            {
                var getTask = await _taskServices.GetValueByPrimaryKeyAsync(task.Id);
                if (getTask == null)
                    return ServicesResult<IEnumerable<IndexTask>>.Failure("Task not found.");

                // Create the IndexTask object
                var value = new IndexTask()
                {
                    TaskId = getTask.Id,
                    TaskName = getTask.TaskName,
                    Status = (await _statusServices.GetAllAsync()).FirstOrDefault(x => x.Id == getTask.StatusId)?.Value,
                };

                // Add task to the data list
                data.Add(value);
            }

            return ServicesResult<IEnumerable<IndexTask>>.Success(data);
        }

        #endregion

        #region Get Task Detail

        /// <summary>
        /// Retrieves detailed information for a specific task and user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="taskId">The ID of the task.</param>
        /// <returns>The task details if found, or an error message if not.</returns>
        public async Task<ServicesResult<DetailTask>> GetTaskDetail(string userId, string taskId)
        {
            // Check for invalid parameters
            if (string.IsNullOrEmpty(taskId) || string.IsNullOrEmpty(userId))
                return ServicesResult<DetailTask>.Failure("Invalid parameters.");

            // Retrieve the plan ID associated with the task
            var planId = (await _taskInPlanServices.GetAllAsync()).FirstOrDefault(x => x.TaskId == taskId)?.PlanId;
            if (planId == null)
                return ServicesResult<DetailTask>.Failure("Plan not found.");

            // Retrieve the project ID associated with the plan
            var getProjectId = (await _planInProjectServices.GetAllAsync()).FirstOrDefault(x => x.PlanId == planId)?.ProjectId;
            if (getProjectId == null)
                return ServicesResult<DetailTask>.Failure("Project not found.");

            // Check if the user exists and has a valid role in the project
            if ((await _applicationUserServices.GetUser(userId)) == null ||
                !(await _roleApplicationUserInProjectServices.GetAllAsync()).Any(s => s.ApplicationUserId == userId && s.ProjectId == getProjectId))
                return ServicesResult<DetailTask>.Failure("User not authorized.");

            // Retrieve the task details
            var task = await _taskServices.GetValueByPrimaryKeyAsync(taskId);
            if (task == null)
                return ServicesResult<DetailTask>.Failure("Task not found.");

            // Create the DetailTask object
            var data = new DetailTask()
            {
                TaskId = taskId,
                TaskName = task.TaskName,
                CreateAt = task.CreateAt,
                EndAt = task.EndAt,
                IsDone = task.IsDone,
                Status = (await _statusServices.GetAllAsync()).FirstOrDefault(x => x.Id == task.StatusId)?.Value,
                StartAt = task.StartAt,
                IndexMembers = new List<Shared.member.IndexMember>()
            };

            // Retrieve members associated with the task
            var getMemberInTasks = (await _memberInTaskServices.GetAllAsync()).Where(x => x.TaskId == taskId);
            if (!getMemberInTasks.Any())
                return ServicesResult<DetailTask>.Success(data);

            // Retrieve and add members to the task details
            foreach (var member in getMemberInTasks)
            {
                var positionWork = await _positionWorkOfMemberServices.GetValueByPrimaryKeyAsync(member.PositionWorkOfMemberId);
                if (positionWork == null)
                    return ServicesResult<DetailTask>.Failure("Position work not found.");

                var getRoleUserInProject = await _roleApplicationUserInProjectServices.GetValueByPrimaryKeyAsync(positionWork.RoleApplicationUserInProjectId);
                if (getRoleUserInProject == null)
                    return ServicesResult<DetailTask>.Failure("Role in project not found.");

                var user = await _applicationUserServices.GetUser(getRoleUserInProject.ApplicationUserId);
                if (user == null)
                    return ServicesResult<DetailTask>.Failure("User not found.");

                var value = new IndexMember()
                {
                    RoleUserInProjectId = getRoleUserInProject.Id,
                    PositionWorkName = (await _positionInProjectServices.GetValueByPrimaryKeyAsync(positionWork.PostitionInProjectId))?.PositionName,
                    UserName = user.UserName,
                    UserAvata = user.PathImage,
                };

                // Add member to the task details
                data.IndexMembers.Add(value);
            }

            return ServicesResult<DetailTask>.Success(data);
        }

        #endregion

        // Define regions for better readability and organization
        #region Add Task

        /// <summary>
        /// Adds a new task to a specific plan.
        /// </summary>
        /// <param name="userId">The ID of the user who is adding the task.</param>
        /// <param name="addTask">The task details to add.</param>
        /// <param name="planId">The ID of the plan to which the task is being added.</param>
        /// <returns>A result indicating success or failure.</returns>
        public async Task<ServicesResult<bool>> Add(string userId, AddTask addTask, string planId)
        {
            // Validate input parameters
            if (userId == null || addTask == null || string.IsNullOrEmpty(planId))
                return ServicesResult<bool>.Failure("Invalid input parameters.");

            // Retrieve the project ID associated with the plan
            var getProjectId = (await _planInProjectServices.GetAllAsync()).FirstOrDefault(x => x.PlanId == planId)?.ProjectId;
            if (getProjectId == null)
                return ServicesResult<bool>.Failure("Project not found.");

            // Retrieve role IDs for 'Owner', 'Leader', and 'Manager' roles in the project
            var getRoleOwnerId = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Owner")?.Id;
            var getRoleLeaderId = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Leader")?.Id;
            var getRoleManagerId = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Manager")?.Id;

            // Check if the user has valid roles in the project (Owner, Leader, or Manager)
            if ((await _applicationUserServices.GetUser(userId)) == null ||
                !(await _roleApplicationUserInProjectServices.GetAllAsync()).Any(x => x.ApplicationUserId == userId && x.ProjectId == getProjectId &&
                (x.RoleInProjectId == getRoleLeaderId || x.RoleInProjectId == getRoleManagerId || x.RoleInProjectId == getRoleOwnerId)))
                return ServicesResult<bool>.Failure("User not authorized to add task.");

            // Check if a task with the same name already exists in the plan
            var getTaskInPlans = (await _taskInPlanServices.GetAllAsync()).Where(x => x.PlanId == planId);
            if (getTaskInPlans.Any())
            {
                foreach (var task in getTaskInPlans)
                {
                    if ((await _taskServices.GetValueByPrimaryKeyAsync(task.TaskId)).TaskName == addTask.TaskName)
                        return ServicesResult<bool>.Failure("Task with the same name already exists.");
                }
            }

            // Proceed with adding the new task if no conflicts
            return await AddMethodHelper(addTask, planId);
        }

        #endregion

        #region Add Method Helper

        /// <summary>
        /// Helper method to actually add the task and its associated members to the plan.
        /// </summary>
        /// <param name="addTask">The task details to add.</param>
        /// <param name="planId">The ID of the plan to which the task is being added.</param>
        /// <returns>A result indicating success or failure.</returns>
        private async Task<ServicesResult<bool>> AddMethodHelper(AddTask addTask, string planId)
        {
            // Generate a random number for unique task and member IDs
            var random = new Random().Next(1000000, 9999999);

            // Create a new task DTO
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

            // Set the initial status based on the task's start date
            if (DateTime.Now == addTask.StartAt) task.StatusId = 3;
            if (DateTime.Now < addTask.StartAt) task.StatusId = 2;

            // Add the task to the database
            if (!await _taskServices.AddAsync(task))
                return ServicesResult<bool>.Failure("Failed to add task.");

            // Create the task-plan relationship
            var taskPlan = new TaskInPlan()
            {
                Id = $"1008-{random}-{DateTime.Now}",
                PlanId = planId,
                TaskId = task.Id,
            };

            // Add the task-plan relationship to the database
            if (!(await _taskInPlanServices.AddAsync(taskPlan)))
                return ServicesResult<bool>.Failure("Failed to associate task with plan.");

            // Add members to the task
            foreach (var member in addTask.IndexMembers)
            {
                var positionWork = await _positionInProjectServices.GetAllAsync();
                var positionWorkId = positionWork.FirstOrDefault(x => x.PositionName == member.PositionWorkName)?.PositionName;

                if (string.IsNullOrEmpty(positionWorkId))
                    return ServicesResult<bool>.Failure("Invalid position work name.");

                var value = new MemberInTask()
                {
                    Id = $"1008-{random}-{DateTime.Now}",
                    PositionWorkOfMemberId = positionWorkId,
                    TaskId = task.Id
                };

                // Add each member to the task
                if (!(await _memberInTaskServices.AddAsync(value)))
                    return ServicesResult<bool>.Failure("Failed to add member to task.");
            }

            // Return success if all operations are completed successfully
            return ServicesResult<bool>.Success(true);
        }

        #endregion
        #region Update Task Information

        /// <summary>
        /// Updates the information of a specific task.
        /// </summary>
        /// <param name="userId">The ID of the user requesting the update.</param>
        /// <param name="taskId">The ID of the task to update.</param>
        /// <param name="updateTask">The updated task information.</param>
        /// <returns>A result indicating success or failure.</returns>
        public async Task<ServicesResult<bool>> UpdateInfo(string userId, string taskId, UpdateTask updateTask)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(taskId) || updateTask == null)
                return ServicesResult<bool>.Failure("Invalid input parameters.");

            // Retrieve the associated plan ID
            var planId = (await _taskInPlanServices.GetAllAsync()).FirstOrDefault(x => x.TaskId == taskId)?.PlanId;
            if (planId == null) return ServicesResult<bool>.Failure("Plan not found.");

            // Retrieve the project ID for the given plan
            var getProjectId = (await _planInProjectServices.GetAllAsync()).FirstOrDefault(x => x.PlanId == planId)?.ProjectId;
            if (getProjectId == null) return ServicesResult<bool>.Failure("Project not found.");

            // Retrieve role IDs for the 'Owner', 'Leader', and 'Manager' roles
            var getRoleOwnerId = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Owner")?.Id;
            var getRoleLeaderId = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Leader")?.Id;
            var getRoleManagerId = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Manager")?.Id;

            // Check if the user is authorized to update the task
            var userHasRole = (await _roleApplicationUserInProjectServices.GetAllAsync())
                .Any(x => x.ApplicationUserId == userId && x.ProjectId == getProjectId &&
                    (x.RoleInProjectId == getRoleLeaderId || x.RoleInProjectId == getRoleManagerId || x.RoleInProjectId == getRoleOwnerId));
            if ((await _applicationUserServices.GetUser(userId)) == null || !userHasRole)
                return ServicesResult<bool>.Failure("User not authorized to update task.");

            // Check if a task with the same name already exists in the plan
            var getTaskInPlans = (await _taskInPlanServices.GetAllAsync()).Where(x => x.PlanId == planId);
            if (getTaskInPlans.Any())
            {
                foreach (var task in getTaskInPlans)
                {
                    if ((await _taskServices.GetValueByPrimaryKeyAsync(task.TaskId)).TaskName == updateTask.TaskName)
                        return ServicesResult<bool>.Failure("A task with the same name already exists.");
                }
            }

            // Proceed to update the task details
            return await UpdateTaskMethod(taskId, updateTask);
        }

        #endregion

        #region Update Task Method

        /// <summary>
        /// Helper method to perform the actual update of a task's information.
        /// </summary>
        /// <param name="taskId">The ID of the task to update.</param>
        /// <param name="updateTask">The updated task information.</param>
        /// <returns>A result indicating success or failure.</returns>
        private async Task<ServicesResult<bool>> UpdateTaskMethod(string taskId, UpdateTask updateTask)
        {
            var random = new Random().Next(1000000, 9999999);

            // Retrieve the task by its ID
            var getTask = await _taskServices.GetValueByPrimaryKeyAsync(taskId);
            if (getTask == null) return ServicesResult<bool>.Failure("Task not found.");

            // Update the task details
            getTask.TaskName = updateTask.TaskName;
            getTask.TaskDescription = updateTask.TaskDescription;

            // Update the task members if any changes are provided
            if (updateTask.Members.Any())
            {
                var memberOld = (await _memberInTaskServices.GetAllAsync()).Where(x => x.TaskId == taskId).ToList();

                // Remove old members if any
                foreach (var member in memberOld)
                {
                    if (!await _memberInTaskServices.DeleteAsync(member.Id))
                        return ServicesResult<bool>.Failure("Failed to remove old members.");
                }

                // Add new members to the task
                foreach (var member in updateTask.Members)
                {
                    var positionWork = await _positionInProjectServices.GetAllAsync();
                    var positionWorkId = positionWork.FirstOrDefault(x => x.PositionName == member.PositionWorkName)?.PositionName;

                    if (string.IsNullOrEmpty(positionWorkId))
                        return ServicesResult<bool>.Failure("Invalid position work name.");

                    var value = new MemberInTask()
                    {
                        Id = $"1008-{random}-{DateTime.Now}",
                        PositionWorkOfMemberId = positionWorkId,
                        TaskId = taskId,
                    };

                    if (!(await _memberInTaskServices.AddAsync(value)))
                        return ServicesResult<bool>.Failure("Failed to add new members.");
                }
            }
            else
            {
                // If no members are provided, remove all existing members from the task
                var memberOld = (await _memberInTaskServices.GetAllAsync()).Where(x => x.TaskId == taskId);
                foreach (var member in memberOld)
                {
                    if (!await _memberInTaskServices.DeleteAsync(member.Id))
                        return ServicesResult<bool>.Failure("Failed to remove old members.");
                }
            }

            // Update the task in the database
            if (await _taskServices.UpdateAsync(taskId, getTask))
                return ServicesResult<bool>.Success(true);

            return ServicesResult<bool>.Failure("Failed to update task.");
        }

        #endregion
        #region Delete Task

        /// <summary>
        /// Deletes a task, including associated members, if the user has the appropriate permissions.
        /// </summary>
        /// <param name="userId">The ID of the user attempting to delete the task.</param>
        /// <param name="taskId">The ID of the task to delete.</param>
        /// <returns>A result indicating success or failure of the deletion.</returns>
        public async Task<ServicesResult<bool>> Delete(string userId, string taskId)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(taskId))
                return ServicesResult<bool>.Failure("Invalid input parameters.");

            // Retrieve the associated plan ID for the task
            var planId = (await _taskInPlanServices.GetAllAsync()).FirstOrDefault(x => x.TaskId == taskId)?.PlanId;
            if (planId == null) return ServicesResult<bool>.Failure("Plan not found.");

            // Retrieve the project ID for the given plan
            var getProjectId = (await _planInProjectServices.GetAllAsync()).FirstOrDefault(x => x.PlanId == planId)?.ProjectId;
            if (getProjectId == null) return ServicesResult<bool>.Failure("Project not found.");

            // Retrieve role IDs for the 'Owner', 'Leader', and 'Manager' roles
            var getRoleOwnerId = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Owner")?.Id;
            var getRoleLeaderId = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Leader")?.Id;
            var getRoleManagerId = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Manager")?.Id;

            // Check if the user is authorized to delete the task
            var userHasRole = (await _roleApplicationUserInProjectServices.GetAllAsync())
                .Any(x => x.ApplicationUserId == userId && x.ProjectId == getProjectId &&
                    (x.RoleInProjectId == getRoleLeaderId || x.RoleInProjectId == getRoleManagerId || x.RoleInProjectId == getRoleOwnerId));
            if ((await _applicationUserServices.GetUser(userId)) == null || !userHasRole)
                return ServicesResult<bool>.Failure("User not authorized to delete task.");

            // Retrieve the task and its associated members
            var getTask = await _taskServices.GetValueByPrimaryKeyAsync(taskId);
            var memberTask = (await _memberInTaskServices.GetAllAsync()).Where(x => x.TaskId == taskId);

            // Delete associated members before deleting the task if there are any
            if (memberTask.Any())
            {
                foreach (var member in memberTask)
                {
                    if (!await _memberInTaskServices.DeleteAsync(member.Id))
                        return ServicesResult<bool>.Failure("Failed to delete task members.");
                }
            }

            // Proceed to delete the task
            if (await _taskServices.DeleteAsync(taskId))
                return ServicesResult<bool>.Success(true);

            return ServicesResult<bool>.Failure("Failed to delete task.");
        }

        #endregion

        #region Update Task Status

        /// <summary>
        /// Updates the status of a task based on the current time and its completion status.
        /// </summary>
        /// <param name="userId">The ID of the user updating the status.</param>
        /// <param name="taskId">The ID of the task whose status is to be updated.</param>
        /// <returns>A result indicating the success or failure of the status update.</returns>
        public async Task<ServicesResult<bool>> UpdateStatus(string userId, string taskId)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(taskId))
                return ServicesResult<bool>.Failure("Invalid input parameters.");

            // Retrieve associated plan ID for the task
            var planId = (await _taskInPlanServices.GetAllAsync()).FirstOrDefault(x => x.TaskId == taskId)?.PlanId;
            if (planId == null) return ServicesResult<bool>.Failure("Plan not found.");

            // Retrieve associated project ID for the plan
            var getProjectId = (await _planInProjectServices.GetAllAsync()).FirstOrDefault(x => x.PlanId == planId)?.ProjectId;
            if (getProjectId == null) return ServicesResult<bool>.Failure("Project not found.");

            // Retrieve role IDs for "Owner", "Leader", and "Manager"
            var getRoleOwnerId = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Owner")?.Id;
            var getRoleLeaderId = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Leader")?.Id;
            var getRoleManagerId = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Manager")?.Id;

            // Check if the user has permission to update the task's status
            var userHasRole = (await _roleApplicationUserInProjectServices.GetAllAsync())
                .Any(x => x.ApplicationUserId == userId && x.ProjectId == getProjectId &&
                    (x.RoleInProjectId == getRoleLeaderId || x.RoleInProjectId == getRoleManagerId || x.RoleInProjectId == getRoleOwnerId));
            if ((await _applicationUserServices.GetUser(userId)) == null || !userHasRole)
                return ServicesResult<bool>.Failure("User not authorized to update task status.");

            // Retrieve the task
            var getTask = await _taskServices.GetValueByPrimaryKeyAsync(taskId);
            if (getTask == null) return ServicesResult<bool>.Failure("Task not found.");

            // Update task status based on conditions
            if (getTask.IsDone == false && getTask.StartAt < DateTime.Now && DateTime.Now < getTask.EndAt)
                getTask.StatusId = 3; // Task in progress
            if (getTask.IsDone == false && getTask.StartAt < DateTime.Now && DateTime.Now > getTask.EndAt)
                getTask.StatusId = 6; // Task overdue
            if (getTask.IsDone == true && getTask.StartAt < DateTime.Now && DateTime.Now > getTask.EndAt)
                getTask.StatusId = 7; // Task completed (overdue)
            if (getTask.IsDone == true && getTask.StartAt < DateTime.Now && DateTime.Now < getTask.EndAt)
                getTask.StatusId = 4; // Task completed (on time)
            if (getTask.IsDone == true && getTask.StartAt < DateTime.Now && DateTime.Now == getTask.EndAt)
                getTask.StatusId = 5; // Task completed (just finished)

            // Update the task status in the database
            if (await _taskServices.UpdateAsync(taskId, getTask))
                return ServicesResult<bool>.Success(true);

            return ServicesResult<bool>.Failure("Failed to update task status.");

        }

        #endregion

        #region Update Task Completion Status

        /// <summary>
        /// Updates the completion status of a task and adjusts its status accordingly.
        /// </summary>
        /// <param name="userId">The ID of the user updating the task's completion status.</param>
        /// <param name="taskId">The ID of the task to update.</param>
        /// <returns>A result indicating the success or failure of the update operation.</returns>
        public async Task<ServicesResult<bool>> UpdateIsDone(string userId, string taskId)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(taskId))
                return ServicesResult<bool>.Failure("Invalid input parameters.");

            // Retrieve associated plan ID for the task
            var planId = (await _taskInPlanServices.GetAllAsync()).FirstOrDefault(x => x.TaskId == taskId)?.PlanId;
            if (planId == null) return ServicesResult<bool>.Failure("Plan not found.");

            // Retrieve associated project ID for the plan
            var getProjectId = (await _planInProjectServices.GetAllAsync()).FirstOrDefault(x => x.PlanId == planId)?.ProjectId;
            if (getProjectId == null) return ServicesResult<bool>.Failure("Project not found.");

            // Retrieve role IDs for "Owner", "Leader", and "Manager"
            var getRoleOwnerId = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Owner")?.Id;
            var getRoleLeaderId = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Leader")?.Id;
            var getRoleManagerId = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Manager")?.Id;

            // Check if the user has permission to update the task's completion status
            var userHasRole = (await _roleApplicationUserInProjectServices.GetAllAsync())
                .Any(x => x.ApplicationUserId == userId && x.ProjectId == getProjectId &&
                    (x.RoleInProjectId == getRoleLeaderId || x.RoleInProjectId == getRoleManagerId || x.RoleInProjectId == getRoleOwnerId));
            if ((await _applicationUserServices.GetUser(userId)) == null || !userHasRole)
                return ServicesResult<bool>.Failure("User not authorized to update task completion status.");

            // Retrieve the task
            var getTask = await _taskServices.GetValueByPrimaryKeyAsync(taskId);
            if (getTask == null) return ServicesResult<bool>.Failure("Task not found.");

            // Toggle the completion status of the task
            getTask.IsDone = !getTask.IsDone;

            // Update the task in the database
            if (!(await _taskServices.UpdateAsync(taskId, getTask)))
                return ServicesResult<bool>.Failure("Failed to update task completion status.");

            // After updating the completion status, also update the task status based on the new state
            return await UpdateStatus(userId, taskId);

        }

        #endregion

    }
}
