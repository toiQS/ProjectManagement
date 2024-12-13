using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using PM.Domain;
using PM.DomainServices.ILogic;
using PM.Persistence.IServices;
using Shared;
using Shared.plan;
using Shared.task;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace PM.DomainServices.Logic
{
    public class PlanLogic : IPlanLogic
    {
        private readonly IApplicationUserServices _applicationUserServices;
        private readonly ITaskInPlanServices _taskInPlanServices;
        private readonly ITaskServices _taskServices;
        private readonly IRoleApplicationUserInProjectServices _roleApplicationUserInProjectServices;
        private readonly IPlanServices _planServices;
        private readonly IPlanInProjectServices _planInProjectServices;
        private readonly IStatusServices _statusServices;
        private readonly ITaskLogic _taskLogic;

        /// <summary>
        /// Constructor for the PlanLogic class with dependency injection.
        /// </summary>
        /// <param name="applicationUserServices">Service for user management.</param>
        /// <param name="taskInPlanServices">Service for managing tasks within plans.</param>
        /// <param name="taskServices">Service for task management.</param>
        /// <param name="roleApplicationUserInProjectServices">Service for managing user roles in projects.</param>
        /// <param name="planServices">Service for plan management.</param>
        /// <param name="planInProjectServices">Service for linking plans to projects.</param>
        /// <param name="statusServices">Service for managing statuses.</param>
        /// <param name="taskLogic">Logic layer for task-related operations.</param>
        public PlanLogic(
            IApplicationUserServices applicationUserServices,
            ITaskInPlanServices taskInPlanServices,
            ITaskServices taskServices,
            IRoleApplicationUserInProjectServices roleApplicationUserInProjectServices,
            IPlanServices planServices,
            IPlanInProjectServices planInProjectServices,
            IStatusServices statusServices,
            ITaskLogic taskLogic)
        {
            _applicationUserServices = applicationUserServices ?? throw new ArgumentNullException(nameof(applicationUserServices));
            _taskInPlanServices = taskInPlanServices ?? throw new ArgumentNullException(nameof(taskInPlanServices));
            _taskServices = taskServices ?? throw new ArgumentNullException(nameof(taskServices));
            _roleApplicationUserInProjectServices = roleApplicationUserInProjectServices ?? throw new ArgumentNullException(nameof(roleApplicationUserInProjectServices));
            _planServices = planServices ?? throw new ArgumentNullException(nameof(planServices));
            _planInProjectServices = planInProjectServices ?? throw new ArgumentNullException(nameof(planInProjectServices));
            _statusServices = statusServices ?? throw new ArgumentNullException(nameof(statusServices));
            _taskLogic = taskLogic ?? throw new ArgumentNullException(nameof(taskLogic));
        }


        #region Plan Retrieval

        /// <summary>
        /// Retrieves a list of plans within a specified project for a given user.
        /// </summary>
        /// <param name="userId">The ID of the user requesting the plans.</param>
        /// <param name="projectId">The ID of the project to retrieve plans from.</param>
        /// <returns>
        /// A <see cref="ServicesResult{T}"/> containing a list of <see cref="IndexPlan"/> objects if successful,
        /// or an error message if the operation fails.
        /// </returns>
        public async Task<ServicesResult<IEnumerable<IndexPlan>>> GetPlansInProjectId(string userId, string projectId)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId))
                return ServicesResult<IEnumerable<IndexPlan>>.Failure("Invalid userId or projectId.");

            // Verify if the user exists and is associated with the project
            if ((await _applicationUserServices.GetUserDetailByUserId(userId)) == null
                || !(await _roleApplicationUserInProjectServices.GetAllAsync()).Any(x => x.ProjectId == projectId && x.ApplicationUserId == userId))
                return ServicesResult<IEnumerable<IndexPlan>>.Failure("User is not associated with the project.");

            // Retrieve plans linked to the project
            var planProject = (await _planInProjectServices.GetAllAsync()).Where(x => x.ProjectId == projectId);

            if (planProject.Any())
            {
                var data = new List<IndexPlan>();

                foreach (var plan in planProject)
                {
                    // Fetch plan details
                    var planItem = await _planServices.GetValueByPrimaryKeyAsync(plan.Id);
                    if (planItem != null)
                    {
                        // Map plan details to IndexPlan
                        var value = new IndexPlan
                        {
                            PlanId = plan.Id,
                            PlanName = planItem.PlanName,
                            Status = (await _statusServices.GetAllAsync())
                                .FirstOrDefault(x => x.Id == planItem.StatusId)?.Value ?? "Unknown"
                        };
                        data.Add(value);
                    }
                }

                return ServicesResult<IEnumerable<IndexPlan>>.Success(data);
            }

            // Return failure if no plans are found
            return ServicesResult<IEnumerable<IndexPlan>>.Failure("No plans found for the specified project.");
        }

        #endregion
        #region Plan Details

        /// <summary>
        /// Retrieves detailed information about a specific plan by its ID for a given user.
        /// </summary>
        /// <param name="userId">The ID of the user requesting the plan details.</param>
        /// <param name="planId">The ID of the plan to retrieve.</param>
        /// <returns>
        /// A <see cref="ServicesResult{T}"/> containing a <see cref="DetailPlan"/> object if successful,
        /// or an error message if the operation fails.
        /// </returns>
        public async Task<ServicesResult<DetailPlan>> GetDetailPlanById(string userId, string planId)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(planId) || string.IsNullOrEmpty(userId))
                return ServicesResult<DetailPlan>.Failure("Invalid planId or userId.");

            // Retrieve the plan-project association
            var planProject = (await _planInProjectServices.GetAllAsync()).FirstOrDefault(x => x.PlanId == planId);
            if (planProject == null)
                return ServicesResult<DetailPlan>.Failure("Plan not associated with any project.");

            // Verify the user's existence and association with the project
            if ((await _applicationUserServices.GetUserDetailByUserId(userId)) == null
                || !(await _roleApplicationUserInProjectServices.GetAllAsync())
                    .Any(x => x.ApplicationUserId == userId && x.ProjectId == planProject.ProjectId))
                return ServicesResult<DetailPlan>.Failure("User is not associated with the project.");

            // Retrieve plan details
            var plan = await _planServices.GetValueByPrimaryKeyAsync(planId);
            if (plan == null)
                return ServicesResult<DetailPlan>.Failure("Plan not found.");

            // Initialize the detailed plan object
            var data = new DetailPlan
            {
                PlanId = plan.Id,
                PlanName = plan.PlanName,
                IsDone = plan.IsDone,
                Status = (await _statusServices.GetAllAsync())
                            .FirstOrDefault(x => x.Id == plan.StatusId)?.Value ?? "Unknown"
            };

            // Retrieve tasks associated with the plan
            var taskPlan = (await _taskInPlanServices.GetAllAsync()).Where(x => x.PlanId == planId);

            // If no tasks are associated, return the plan details as-is
            if (!taskPlan.Any())
                return ServicesResult<DetailPlan>.Success(data);

            // Add task details to the plan
            foreach (var task in taskPlan)
            {
                var taskItem = await _taskServices.GetValueByPrimaryKeyAsync(task.TaskId);
                if (taskItem != null)
                {
                    var value = new IndexTask
                    {
                        TaskName = taskItem.TaskName,
                        Status = (await _statusServices.GetAllAsync())
                                    .FirstOrDefault(x => x.Id == taskItem.StatusId)?.Value ?? "Unknown",
                        TaskId = taskItem.Id
                    };
                    data.Tasks.Add(value);
                }
            }

            return ServicesResult<DetailPlan>.Success(data);
        }

        #endregion
        #region Add Plan

        /// <summary>
        /// Adds a new plan to a project, ensuring no duplicate plan names exist within the project.
        /// </summary>
        /// <param name="userId">The ID of the user adding the plan.</param>
        /// <param name="projectId">The ID of the project to add the plan to.</param>
        /// <param name="addPlan">The plan details to add.</param>
        /// <returns>
        /// A <see cref="ServicesResult{T}"/> indicating whether the operation was successful.
        /// </returns>
        public async Task<ServicesResult<bool>> Add(string userId, string projectId, AddPlan addPlan)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId) || addPlan == null)
                return ServicesResult<bool>.Failure("Invalid input parameters.");

            // Verify user existence and their association with the project
            if ((await _applicationUserServices.GetUserDetailByUserId(userId)) == null
                || !(await _roleApplicationUserInProjectServices.GetAllAsync())
                    .Any(x => x.ProjectId == projectId && x.ApplicationUserId == userId))
                return ServicesResult<bool>.Failure("User is not associated with the project.");

            // Check for duplicate plan names in the project
            var existingPlans = (await _planInProjectServices.GetAllAsync()).Where(x => x.ProjectId == projectId);
            if (existingPlans.Any())
            {
                foreach (var plan in existingPlans)
                {
                    var existingPlan = await _planServices.GetValueByPrimaryKeyAsync(plan.Id);
                    if (existingPlan != null && existingPlan.PlanName == addPlan.Name)
                        return ServicesResult<bool>.Failure("Plan with the same name already exists.");
                }
            }

            // Proceed to add the plan
            return await AddMethod(projectId, addPlan);
        }

        /// <summary>
        /// Adds a new plan and its association with the project.
        /// </summary>
        /// <param name="projectId">The ID of the project to associate the plan with.</param>
        /// <param name="addPlan">The plan details to add.</param>
        /// <returns>
        /// A <see cref="ServicesResult{T}"/> indicating whether the operation was successful.
        /// </returns>
        private async Task<ServicesResult<bool>> AddMethod(string projectId, AddPlan addPlan)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(projectId) || addPlan == null)
                return ServicesResult<bool>.Failure("Invalid input parameters.");

            // Generate unique IDs for the plan and its association
            var random = new Random().Next(1000000, 9999999);
            var planId = $"1007-{random}-{DateTime.Now.Ticks}";
            var planProjectId = $"1006-{random}-{DateTime.Now.Ticks}";

            // Create the new plan
            var plan = new Plan
            {
                Id = planId,
                PlanName = addPlan.Name,
                CreateAt = DateTime.Now,
                StartAt = addPlan.StartAt,
                EndAt = addPlan.EndAt,
                IsDone = false,
                StatusId = DeterminePlanStatus(addPlan.StartAt)
            };

            // Add the plan to the database
            if (!await _planServices.AddAsync(plan))
                return ServicesResult<bool>.Failure("Failed to add the plan.");

            // Associate the plan with the project
            var planProject = new PlanInProject
            {
                Id = planProjectId,
                PlanId = plan.Id,
                ProjectId = projectId
            };

            if (await _planInProjectServices.AddAsync(planProject))
                return ServicesResult<bool>.Success(true);

            return ServicesResult<bool>.Failure("Failed to associate the plan with the project.");
        }

        /// <summary>
        /// Determines the status of a plan based on its start date.
        /// </summary>
        /// <param name="startAt">The start date of the plan.</param>
        /// <returns>The status ID representing the plan's status.</returns>
        private int DeterminePlanStatus(DateTime startAt)
        {
            if (startAt == DateTime.Now.Date)
                return 3; // Status: In Progress
            if (startAt > DateTime.Now.Date)
                return 2; // Status: Scheduled
            return 1; // Status: Overdue
        }

        #endregion
        #region Update Plan Information

        /// <summary>
        /// Updates the information of a plan.
        /// </summary>
        /// <param name="userId">The ID of the user performing the update.</param>
        /// <param name="planId">The ID of the plan to update.</param>
        /// <param name="updatePlan">The updated plan details.</param>
        /// <returns>
        /// A <see cref="ServicesResult{T}"/> indicating whether the operation was successful.
        /// </returns>
        public async Task<ServicesResult<bool>> UpdateInfo(string userId, string planId, UpdatePlan updatePlan)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(planId) || updatePlan == null)
                return ServicesResult<bool>.Failure("Invalid input parameters.");

            // Check if the plan exists in the project
            var planProject = (await _planInProjectServices.GetAllAsync()).FirstOrDefault(x => x.PlanId == planId);
            if (planProject == null)
                return ServicesResult<bool>.Failure("Plan not found in the project.");

            // Verify user existence and their association with the project
            if ((await _applicationUserServices.GetUserDetailByUserId(userId)) == null
                || !(await _roleApplicationUserInProjectServices.GetAllAsync())
                    .Any(x => x.ApplicationUserId == userId && x.ProjectId == planProject.ProjectId))
                return ServicesResult<bool>.Failure("User is not authorized to update this plan.");

            // Fetch the plan details
            var plan = await _planServices.GetValueByPrimaryKeyAsync(planId);
            if (plan == null)
                return ServicesResult<bool>.Failure("Plan not found.");

            // Update the plan information
            plan.PlanName = updatePlan.PlanName;

            // Save the updated plan
            if (await _planServices.UpdateAsync(planId, plan))
                return ServicesResult<bool>.Success(true);

            return ServicesResult<bool>.Failure("Failed to update the plan.");
        }

        #endregion
        #region Delete Plan

        /// <summary>
        /// Deletes a plan and its associated tasks if the user has the necessary permissions.
        /// </summary>
        /// <param name="userId">The ID of the user requesting the deletion.</param>
        /// <param name="planId">The ID of the plan to delete.</param>
        /// <returns>
        /// A <see cref="ServicesResult{T}"/> indicating whether the deletion was successful.
        /// </returns>
        public async Task<ServicesResult<bool>> Delete(string userId, string planId)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(planId))
                return ServicesResult<bool>.Failure("Invalid input parameters.");

            // Verify the plan is associated with a project
            var planProject = (await _planInProjectServices.GetAllAsync()).FirstOrDefault(x => x.PlanId == planId);
            if (planProject == null)
                return ServicesResult<bool>.Failure("Plan is not associated with any project.");

            // Check user existence and authorization
            if ((await _applicationUserServices.GetUserDetailByUserId(userId)) == null
                || !(await _roleApplicationUserInProjectServices.GetAllAsync())
                    .Any(x => x.ApplicationUserId == userId && x.ProjectId == planProject.ProjectId))
                return ServicesResult<bool>.Failure("User is not authorized to delete this plan.");

            // Fetch the plan details
            var plan = await _planServices.GetValueByPrimaryKeyAsync(planId);
            if (plan == null)
                return ServicesResult<bool>.Failure("Plan not found.");

            // Fetch associated tasks
            var taskPlans = (await _taskInPlanServices.GetAllAsync()).Where(x => x.PlanId == planId);
            if (taskPlans.Any())
            {
                // Delete all tasks associated with the plan
                foreach (var task in taskPlans)
                {
                    var taskDeletionResult = await _taskLogic.Delete(userId, task.TaskId);
                    if (!taskDeletionResult.Status)
                        return ServicesResult<bool>.Failure("Failed to delete associated tasks.");
                }
            }

            // Delete the association between the plan and project
            if (!await _planInProjectServices.DeleteAsync(planProject.Id))
                return ServicesResult<bool>.Failure("Failed to delete plan-project association.");

            // Delete the plan
            if (await _planServices.DeleteAsync(plan.Id))
                return ServicesResult<bool>.Success(true);

            return ServicesResult<bool>.Failure("Failed to delete the plan.");
        }

        #endregion

        #region Update Plan Completion Status

        /// <summary>
        /// Toggles the completion status of a plan and updates its status accordingly.
        /// </summary>
        /// <param name="userId">The ID of the user attempting to update the plan.</param>
        /// <param name="planId">The ID of the plan to update.</param>
        /// <returns>
        /// A <see cref="ServicesResult{T}"/> indicating whether the operation was successful.
        /// </returns>
        public async Task<ServicesResult<bool>> UpdateIsDone(string userId, string planId)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(planId))
                return ServicesResult<bool>.Failure("Invalid input parameters.");

            // Verify the plan exists in any project
            var planProject = (await _planInProjectServices.GetAllAsync()).FirstOrDefault(x => x.PlanId == planId);
            if (planProject == null)
                return ServicesResult<bool>.Failure("Plan not associated with any project.");

            // Check user existence and authorization
            if ((await _applicationUserServices.GetUserDetailByUserId(userId)) == null
                || !(await _roleApplicationUserInProjectServices.GetAllAsync())
                    .Any(x => x.ApplicationUserId == userId && x.ProjectId == planProject.ProjectId))
                return ServicesResult<bool>.Failure("User is not authorized to update this plan.");

            // Fetch the plan details
            var plan = await _planServices.GetValueByPrimaryKeyAsync(planId);
            if (plan == null)
                return ServicesResult<bool>.Failure("Plan not found.");

            // Toggle the completion status
            plan.IsDone = !plan.IsDone;

            // Update the plan's status based on completion and end date
            if (plan.IsDone)
            {
                if (plan.EndAt == DateTime.Now)
                    plan.StatusId = 5; // Completed on time
                else if (plan.EndAt < DateTime.Now)
                    plan.StatusId = 6; // Completed late
                else
                    plan.StatusId = 4; // Completed early
            }
            else
            {
                if (plan.EndAt < DateTime.Now)
                    plan.StatusId = 6; // Late but incomplete
                else
                    plan.StatusId = 3; // In progress
            }

            // Update the plan in the database
            if (await _planServices.UpdateAsync(planId, plan))
                return ServicesResult<bool>.Success(true);

            return ServicesResult<bool>.Failure("Failed to update the plan.");
        }

        #endregion
    }
}
