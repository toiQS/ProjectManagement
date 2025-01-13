using PM.Domain;
using PM.DomainServices.Models.plans;
using PM.DomainServices.Models;
using PM.Persistence.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PM.DomainServices.ILogic;

namespace PM.DomainServices.Logic
{
    public class PlanLogic : IPlanLogic
    {
        //intialize services
        private readonly IPlanInProjectServices _planInProjectServices;
        private readonly IPlanServices _planServices;
        private readonly IStatusServices _statusServices;
        //intialize logic
        //intialize primary value
        private List<Plan> _planList;
        private List<PlanInProject> _planInProjects;
        private List<Status> _statuses;

        #region private method
        /// <summary>
        /// Retrieves all plans from the database.
        /// </summary>
        /// <returns>A service result containing a list of plans, or an error message.</returns>
        private async Task<ServicesResult<IEnumerable<Plan>>> GetAllPlans()
        {
            // Fetch plans asynchronously
            var plans = await _planServices.GetAllAsync();

            // If no plans are found, return an empty list with a message
            if (plans.Data == null)
                return ServicesResult<IEnumerable<Plan>>.Success(new List<Plan>(), "No plans in database");

            // If there's an issue with the status, return a failure result
            if (plans.Status == false)
                return ServicesResult<IEnumerable<Plan>>.Failure(plans.Message);

            // Store plans in the local list
            _planList = plans.Data.ToList();

            // Return the retrieved plans
            return ServicesResult<IEnumerable<Plan>>.Success(plans.Data, string.Empty);
        }

        /// <summary>
        /// Retrieves all plans associated with projects from the database.
        /// </summary>
        /// <returns>A service result containing a list of plans in projects, or an error message.</returns>
        private async Task<ServicesResult<IEnumerable<PlanInProject>>> GetAllPlanInProjects()
        {
            // Fetch plans in projects asynchronously
            var plans = await _planInProjectServices.GetAllAsync();

            // If no plans in projects are found, return an empty list with a message
            if (plans.Data == null)
                return ServicesResult<IEnumerable<PlanInProject>>.Success(new List<PlanInProject>(), "No plans in database");

            // If there's an issue with the status, return a failure result
            if (plans.Status == false)
                return ServicesResult<IEnumerable<PlanInProject>>.Failure(plans.Message);

            // Store plans in the local list
            _planInProjects = plans.Data.ToList();

            // Return the retrieved plans in projects
            return ServicesResult<IEnumerable<PlanInProject>>.Success(plans.Data, string.Empty);
        }

        /// <summary>
        /// Retrieves all available statuses from the database.
        /// </summary>
        /// <returns>A service result containing a success message or an error message.</returns>
        private async Task<ServicesResult<string>> GetAllStatus()
        {
            // Fetch statuses asynchronously
            var statuses = await _statusServices.GetAllAsync();

            // If no statuses are found, return an empty string with a message
            if (statuses.Data == null)
                return ServicesResult<string>.Success(string.Empty, "No statuses in database");

            // If there's an issue with the status, return a failure result
            if (statuses.Status == false)
                return ServicesResult<string>.Failure(statuses.Message);

            // Store statuses in the local list
            _statuses = statuses.Data.ToList();

            // Return a success message
            return ServicesResult<string>.Success("Success", string.Empty);
        }

        /// <summary>
        /// Retrieves the status information based on the given status ID.
        /// </summary>
        /// <param name="statusId">The ID of the status to retrieve information for.</param>
        /// <returns>A service result containing the status value or an error message.</returns>
        private async Task<ServicesResult<string>> GetStatusInfo(int statusId)
        {
            // If statusId is 0, return a failure result
            if (statusId == 0)
                return ServicesResult<string>.Failure("Invalid status ID");

            // Find the status by its ID
            var getInfo = _statuses.Where(x => x.Id == statusId).FirstOrDefault();

            // If the status is not found, return a failure result
            if (getInfo == null)
                return ServicesResult<string>.Failure($"Can't find status with ID {statusId}");

            // Return the status value
            return ServicesResult<string>.Success(getInfo.Value, string.Empty);
        }

        #endregion
        #region suport method
        #endregion
        #region primary method

        /// <summary>
        /// Retrieves all plans associated with a specific project.
        /// </summary>
        /// <param name="projectId">The ID of the project to get the plans for.</param>
        /// <returns>A service result containing a list of plans in the project or an error message.</returns>
        public async Task<ServicesResult<IEnumerable<IndexPlan>>> GetPlansInProject(string projectId)
        {
            // Initialize the result list
            var result = new List<IndexPlan>();

            // If the project ID is null or empty, return a failure result
            if (string.IsNullOrEmpty(projectId))
                return ServicesResult<IEnumerable<IndexPlan>>.Failure("Project ID is required.");

            // Fetch all plans associated with the given project ID
            var projectPlan = _planInProjects.Where(x => x.ProjectId == projectId);

            // If no plans are found for the project, return an empty list with a message
            if (!projectPlan.Any())
                return ServicesResult<IEnumerable<IndexPlan>>.Success(new List<IndexPlan>(), "No plans found for this project.");

            // Iterate over each plan in the project and fetch detailed information
            foreach (var plan in projectPlan)
            {
                // Fetch the plan details using the plan's ID
                var info = await _planServices.GetValueByPrimaryKeyAsync(plan.PlanId);

                // If plan details are not found or an error occurred, return a failure result
                if (info.Data == null || info.Status == false)
                    return ServicesResult<IEnumerable<IndexPlan>>.Failure(info.Message);

                // Create a new IndexPlan object to store the plan details
                var index = new IndexPlan()
                {
                    PlanName = info.Data.PlanName,
                    PlanId = plan.PlanId,
                };

                // Retrieve the status of the plan using its status ID
                var status = await GetStatusInfo(info.Data.StatusId);

                // Set the status of the plan
                index.Status = status.Data;

                // Add the plan to the result list
                result.Add(index);
            }

            // Return the list of plans associated with the project
            return ServicesResult<IEnumerable<IndexPlan>>.Success(result, string.Empty);
        }

        #endregion
    }
}
