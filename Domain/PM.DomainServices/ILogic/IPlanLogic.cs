using PM.DomainServices.Models;
using PM.DomainServices.Models.plans;

namespace PM.DomainServices.ILogic
{
    public interface IPlanLogic
    {
        #region Plan Retrieval

        /// <summary>
        /// Retrieves a list of plans associated with a project based on the user and project IDs.
        /// </summary>
        /// <param name="userId">The ID of the user requesting the plans.</param>
        /// <param name="projectId">The ID of the project to retrieve plans for.</param>
        /// <returns>A list of plans within the specified project.</returns>
        Task<ServicesResult<IEnumerable<IndexPlan>>> GetPlansInProjectId(string userId, string projectId);

        /// <summary>
        /// Retrieves detailed information for a specific plan based on the user and plan IDs.
        /// </summary>
        /// <param name="userId">The ID of the user requesting the plan details.</param>
        /// <param name="planId">The ID of the plan to retrieve.</param>
        /// <returns>The details of the specified plan.</returns>
        Task<ServicesResult<DetailPlan>> GetDetailPlanById(string userId, string planId);

        #endregion

        #region Plan Management

        /// <summary>
        /// Adds a new plan to the project based on the provided user ID, project ID, and plan details.
        /// </summary>
        /// <param name="userId">The ID of the user adding the plan.</param>
        /// <param name="projectId">The ID of the project where the plan will be added.</param>
        /// <param name="addPlan">The details of the plan to be added.</param>
        /// <returns>A result indicating whether the plan was successfully added.</returns>
        Task<ServicesResult<bool>> Add(string userId, string projectId, AddPlan addPlan);

        /// <summary>
        /// Updates an existing plan's details based on the user ID, plan ID, and updated plan information.
        /// </summary>
        /// <param name="userId">The ID of the user updating the plan.</param>
        /// <param name="planId">The ID of the plan to update.</param>
        /// <param name="updatePlan">The updated plan details.</param>
        /// <returns>A result indicating whether the plan was successfully updated.</returns>
        Task<ServicesResult<bool>> UpdateInfo(string userId, string planId, UpdatePlan updatePlan);

        /// <summary>
        /// Deletes a specific plan based on the user and plan IDs.
        /// </summary>
        /// <param name="userId">The ID of the user deleting the plan.</param>
        /// <param name="planId">The ID of the plan to delete.</param>
        /// <returns>A result indicating whether the plan was successfully deleted.</returns>
        Task<ServicesResult<bool>> Delete(string userId, string planId);
        /// <summary>
        /// Update an exitsting plan is done
        /// </summary>
        /// <param name="userId">The Id of the user updating the plan</param>
        /// <param name="planId">The ID of the plan to delete</param>
        /// <returns></returns>
        Task<ServicesResult<bool>> UpdateIsDone(string userId, string planId);

        #endregion
    }
}
