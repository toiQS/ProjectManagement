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
        private async Task<ServicesResult<IEnumerable<Plan>>> GetAllPlans()
        {
            var plans = await _planServices.GetAllAsync();
            if (plans.Data == null) return ServicesResult<IEnumerable<Plan>>.Success(new List<Plan>(), "no plan in dabase");
            if (plans.Status == false) return ServicesResult<IEnumerable<Plan>>.Failure(plans.Message);
            _planList = plans.Data.ToList();
            return ServicesResult<IEnumerable<Plan>>.Success(plans.Data, string.Empty);
        }
        private async Task<ServicesResult<IEnumerable<PlanInProject>>> GetAllPlanInProjects()
        {
            var plans = await _planInProjectServices.GetAllAsync();
            if (plans.Data == null) return ServicesResult<IEnumerable<PlanInProject>>.Success(new List<PlanInProject>(), "no plan in dabase");
            if (plans.Status == false) return ServicesResult<IEnumerable<PlanInProject>>.Failure(plans.Message);
            _planInProjects = plans.Data.ToList();
            return ServicesResult<IEnumerable<PlanInProject>>.Success(plans.Data, string.Empty);
        }
        private async Task<ServicesResult<string>> GetAllStatus()
        {
            var statuses = await _statusServices.GetAllAsync();
            if (statuses.Data == null) return ServicesResult<string>.Success(string.Empty, "no status in database");
            if (statuses.Status == false) return ServicesResult<string>.Failure(statuses.Message);
            _statuses = statuses.Data.ToList();
            return ServicesResult<string>.Success("Success", string.Empty);
        }
        private async Task<ServicesResult<string>> GetStatusInfo(int statusId)
        {
            if (statusId == 0) return ServicesResult<string>.Failure("");


            var getInfo = _statuses.Where(x => x.Id == statusId).FirstOrDefault();
            if (getInfo == null) return ServicesResult<string>.Failure($"can't get any this status {statusId}");
            return ServicesResult<string>.Success(getInfo.Value, string.Empty);
        }
        #endregion
        #region suport method
        #endregion
        #region primary method

        public async Task<ServicesResult<IEnumerable<IndexPlan>>> GetPlansInProject(string projectId)
        {
            var result = new List<IndexPlan>();
            if (string.IsNullOrEmpty(projectId)) return ServicesResult<IEnumerable<IndexPlan>>.Failure("");
            var projectPlan = _planInProjects.Where(x => x.ProjectId == projectId);
            if (!projectPlan.Any()) return ServicesResult<IEnumerable<IndexPlan>>.Success(new List<IndexPlan>(), "no plan in project");
            foreach (var plan in projectPlan)
            {
                var info = await _planServices.GetValueByPrimaryKeyAsync(plan.PlanId);
                if (info.Data == null || info.Status == false) return ServicesResult<IEnumerable<IndexPlan>>.Failure(info.Message);
                var index = new IndexPlan()
                {
                    PlanName = info.Data.PlanName,
                    PlanId = plan.PlanId,
                };
                var status = await GetStatusInfo(info.Data.StatusId);
                index.Status = status.Data;
                result.Add(index);
            }
            return ServicesResult<IEnumerable<IndexPlan>>.Success(result, string.Empty);
        }
        #endregion
    }
}
