using Shared.project;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PM.Persistence.IServices;
using PM.DomainServices.ILogic;
using System.Diagnostics;
using PM.Domain;
using Microsoft.VisualBasic;
using Shared.task;
using System.Diagnostics.Eventing.Reader;
using System.ComponentModel.DataAnnotations;

namespace PM.DomainServices.Logic
{
    internal class ProjectLogic : IProjecttLogic
    {
        private readonly IApplicationUserServices _applicationUserServices;
        private readonly IRoleApplicationUserInProjectServices _roleApplicationUserServices;
        private readonly IProjectServices _projectServices;
        private readonly IRoleInProjectServices _roleInProjectServices;
        private readonly IStatusServices _statusServices;
        private readonly IMemberLogic _memberLogic;
        private readonly IPlanLogic _planLogic;
        public ProjectLogic(IApplicationUserServices applicationUserServices, IRoleApplicationUserInProjectServices roleApplicationUserServices, IProjectServices projectServices, IRoleInProjectServices roleInProjectServices, IStatusServices statusServices, IMemberLogic memberLogic, IPlanLogic planLogic)
        {
            _applicationUserServices = applicationUserServices;
            _roleApplicationUserServices = roleApplicationUserServices;
            _projectServices = projectServices;
            _roleInProjectServices = roleInProjectServices;
            _statusServices = statusServices;
            _memberLogic = memberLogic;
            _planLogic = planLogic;
        }

        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetProductListUserHasJoined(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return ServicesResult<IEnumerable<IndexProject>>.Failure("");
            if((await _applicationUserServices.GetUser(userId)) == null) return ServicesResult<IEnumerable<IndexProject>>.Failure("");
            var projectUser = (await _roleApplicationUserServices.GetAllAsync()).Where(x => x.ApplicationUserId == userId);
            if (!projectUser.Any()) return ServicesResult<IEnumerable<IndexProject>>.Failure("");
            var getRoleOwner = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Owner").Id;
            if (getRoleOwner == null) return ServicesResult<IEnumerable<IndexProject>>.Failure("");
            var data = new List<IndexProject>();
            foreach (var project in projectUser)
            {
                if ((await _projectServices.GetValueByPrimaryKeyAsync(project.Id)).IsDeleted == true) continue;
                else
                {
                    var owner = (await _roleApplicationUserServices.GetAllAsync()).FirstOrDefault(x => x.RoleInProjectId == getRoleOwner && x.ProjectId == project.ProjectId);
                    if (owner == null) return ServicesResult<IEnumerable<IndexProject>>.Failure("");
                    var user = await _applicationUserServices.GetUser(owner.ApplicationUserId);
                    if (user == null) return ServicesResult<IEnumerable<IndexProject>>.Failure("");
                    var projectItem = await _projectServices.GetValueByPrimaryKeyAsync(project.ProjectId);
                    var value = new IndexProject()
                    {
                        ProjectId = project.ProjectId,
                        OwnerAvata = user.PathImage,
                        ProjectName = projectItem.ProjectName,
                        OwnerName = user.UserName
                    };

                    data.Add(value);
                }
            }
            return ServicesResult<IEnumerable<IndexProject>>.Success(data);
        }
        public async Task<ServicesResult<IEnumerable<IndexProject>>> GetProductListUserHasOwner(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return ServicesResult<IEnumerable<IndexProject>>.Failure("");
            if ((await _applicationUserServices.GetUser(userId)) == null) return ServicesResult<IEnumerable<IndexProject>>.Failure("");
           
            var getRoleOwner = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Owner").Id;
            var data = new List<IndexProject>();
            var projectUser = (await _roleApplicationUserServices.GetAllAsync()).Where(x => x.ApplicationUserId == userId && x.RoleInProjectId == getRoleOwner);
            if (!projectUser.Any()) return ServicesResult<IEnumerable<IndexProject>>.Failure("");
            var user = await _applicationUserServices.GetUser(userId);
            if (user == null) return ServicesResult<IEnumerable<IndexProject>>.Failure("");
            foreach (var item in projectUser)
            {
                if ((await _projectServices.GetValueByPrimaryKeyAsync(item.Id)).IsDeleted == true) continue;
                else
                {
                    var project = await _projectServices.GetValueByPrimaryKeyAsync(item.ProjectId);
                    if (project == null) return ServicesResult<IEnumerable<IndexProject>>.Failure("");
                    var value = new IndexProject()
                    {
                        ProjectId = project.Id,
                        OwnerName = user.UserName,
                        ProjectName = project.ProjectName,
                        OwnerAvata = user.PathImage
                    };
                    data.Add(value);
                }

            }
            return ServicesResult<IEnumerable<IndexProject>>.Success(data);
        }
        public async Task<ServicesResult<DetailProject>> GetProductDetailProjectHasJoined(string userId, string projectId)
        {
            if (userId == null || projectId == null) return ServicesResult<DetailProject>.Failure("");
            var getRoleOwner = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Owner").Id;
            var projectUser = (await _roleApplicationUserServices.GetAllAsync()).Where(x => x.ApplicationUserId == userId && x.ProjectId == projectId);
            if (!projectUser.Any()) return ServicesResult<DetailProject>.Failure("");
            var project = await _projectServices.GetValueByPrimaryKeyAsync(projectId);
            if (project.IsAccessed != true && !(await _roleApplicationUserServices.GetAllAsync()).Any(x => x.ApplicationUserId == userId && x.RoleInProjectId == getRoleOwner))
                return ServicesResult<DetailProject>.Failure("");
            var roleUser = (await _roleApplicationUserServices.GetAllAsync()).FirstOrDefault(x => x.ProjectId == projectId && x.RoleInProjectId == getRoleOwner);
            if (roleUser == null) return ServicesResult<DetailProject>.Failure("");
            var user = await _applicationUserServices.GetUser(roleUser.ApplicationUserId);
            if (user == null) return ServicesResult<DetailProject>.Failure("");
            var data = new DetailProject()
            {
                ProjectId = projectId,
                OwnerName = user.UserName,
                CreateAt = project.CreateAt,
                ProjectName = project.ProjectName,
                EndAt = project.EndAt,
                IsAccessed = project.IsAccessed,
                IsDeleted = project.IsDeleted,
                IsDone = project.IsDone,
                OwnerAvata = user.PathImage,
                ProjectDescription = project.ProjectDescription,
                QuantityMember = ((await _roleApplicationUserServices.GetAllAsync()).Where(x => x.ProjectId == projectId)).Count(),
                StartAt = project.StartAt,
                Status = (await _statusServices.GetAllAsync()).FirstOrDefault(x => x.Id == project.StatusId).Value
            };
            var member = await _memberLogic.GetMembersInProject(userId, projectId);
            if (member.Data != null) return ServicesResult<DetailProject>.Failure("");
            data.Members = member.Data.Select(x => new Shared.member.IndexMember()
            {
                PositionWorkName = x.PositionWorkName,
                UserName = x.UserName,
                RoleUserInProjectId = userId,
                UserAvata = user.PathImage,
            }).ToList();
            return ServicesResult<DetailProject>.Success(data);
        }
        public async Task<ServicesResult<bool>> Add(string userId, AddProject addProject)
        {
            var getRoleOwner = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Owner").Id;
            if (userId == null || addProject == null) return ServicesResult<bool>.Failure("");
            var projectUser = (await _roleApplicationUserServices.GetAllAsync()).Where(x => x.ApplicationUserId == userId && x.RoleInProjectId == getRoleOwner);
            if (!projectUser.Any()) return await AddMethod(userId, addProject);
            foreach (var item in projectUser)
            {
                var project = await _projectServices.GetValueByPrimaryKeyAsync (item.ProjectId);
                if (project == null) return ServicesResult<bool>.Failure("");
                if(project.ProjectName == addProject.ProjectName) return ServicesResult<bool>.Failure("");
            }
            return await AddMethod(userId, addProject);
        }
        private async Task<ServicesResult<bool>> AddMethod(string userId, AddProject addProject)
        {
            var randon = new Random().Next(1000000, 9000000);

            var project = new Project()
            {
                Id = $"1001-{randon}-{DateTime.Now}",
                ProjectName = addProject.ProjectName,
                CreateAt = DateTime.Now,
                EndAt = addProject.EndAt,
                IsAccessed = true,
                IsDeleted = false,
                IsDone = false,
                ProjectDescription = addProject.ProjectDescription,
                StartAt = addProject.StartAt
            };
            if (DateTime.Now == addProject.StartAt) project.StatusId = 3;
            if (DateTime.Now < addProject.StartAt) project.StatusId = 2;

            if(! await _projectServices.AddAsync(project)) return ServicesResult<bool>.Failure("");
            var getRoleOwner = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Owner").Id;
            var roleProject = new RoleApplicationUserInProject()
            {
                Id = $"1002-{randon}-{DateTime.Now}",
                ProjectId = project.Id,
                ApplicationUserId = userId,
                RoleInProjectId = getRoleOwner
            };
            if (await _roleApplicationUserServices.AddAsync(roleProject)) return ServicesResult<bool>.Success(true);
            return ServicesResult<bool>.Failure("");
        }
        
        public async Task<ServicesResult<bool>> UpdateInfo(string userId, string projectId, UpdateProject updateProject)
        {
            var getRoleOwner = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Owner").Id;
            if (userId == null || updateProject == null || projectId == null) return ServicesResult<bool>.Failure("");
            var projectUser = (await _roleApplicationUserServices.GetAllAsync()).Where(x => x.ApplicationUserId == userId && x.RoleInProjectId == getRoleOwner && x.ProjectId == projectId);
            if (!projectUser.Any()) return ServicesResult<bool>.Failure("");
            var project = await _projectServices.GetValueByPrimaryKeyAsync(projectId);
            if (project == null) return ServicesResult<bool>.Failure("");
            if (project.ProjectName == updateProject.ProjectName) return ServicesResult<bool>.Failure("");
            project.ProjectName = updateProject.ProjectName;
            project.ProjectDescription = updateProject.ProjectDescription;
            if(await _projectServices.UpdateAsync(projectId, project)) return ServicesResult<bool>.Success(true);
            return ServicesResult<bool>.Failure("");
        }

        public async Task<ServicesResult<bool>> UpdateIsDelete(string userId, string projectId)
        {
            var getRoleOwner = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Owner").Id;
            if (userId == null || projectId == null) return ServicesResult<bool>.Failure("");
            var projectUser = (await _roleApplicationUserServices.GetAllAsync()).Where(x => x.ApplicationUserId == userId && x.RoleInProjectId == getRoleOwner && x.ProjectId == projectId);
            if (!projectUser.Any()) return ServicesResult<bool>.Failure("");
            var project = await _projectServices.GetValueByPrimaryKeyAsync(projectId);
            if (project == null) return ServicesResult<bool>.Failure("");
            project.IsDeleted = !project.IsDeleted;
            if (await _projectServices.UpdateAsync(projectId, project)) return ServicesResult<bool>.Success(true);
            return ServicesResult<bool>.Failure("");
        }
        public async Task<ServicesResult<bool>> UpdateIsAccessed(string userId, string projectId)
        {
            var getRoleOwner = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Owner").Id;
            if (userId == null || projectId == null) return ServicesResult<bool>.Failure("");
            var projectUser = (await _roleApplicationUserServices.GetAllAsync()).Where(x => x.ApplicationUserId == userId && x.RoleInProjectId == getRoleOwner && x.ProjectId == projectId);
            if (!projectUser.Any()) return ServicesResult<bool>.Failure("");
            var project = await _projectServices.GetValueByPrimaryKeyAsync(projectId);
            if (project == null) return ServicesResult<bool>.Failure("");
            project.IsAccessed = !project.IsAccessed;
            if (await _projectServices.UpdateAsync(projectId, project)) return ServicesResult<bool>.Success(true);
            return ServicesResult<bool>.Failure("");
        }
        public async Task<ServicesResult<bool>> UpdateIsDone(string userId, string projectId)
        {
            var getRoleOwner = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Owner").Id;
            if (userId == null || projectId == null) return ServicesResult<bool>.Failure("");
            var projectUser = (await _roleApplicationUserServices.GetAllAsync()).Where(x => x.ApplicationUserId == userId && x.RoleInProjectId == getRoleOwner && x.ProjectId == projectId);
            if (!projectUser.Any()) return ServicesResult<bool>.Failure("");
            var project = await _projectServices.GetValueByPrimaryKeyAsync(projectId);
            if (project == null) return ServicesResult<bool>.Failure("");
            project.IsAccessed = !project.IsAccessed;
            if (!await _projectServices.UpdateAsync(projectId, project))
            return ServicesResult<bool>.Failure("");
            return await UpdateStatus(userId, projectId);
        }
        public async Task<ServicesResult<bool>> UpdateStatus(string userId, string projectId)
        {
            var getRoleOwner = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Owner").Id;
            if (userId == null || projectId == null) return ServicesResult<bool>.Failure("");
            var projectUser = (await _roleApplicationUserServices.GetAllAsync()).Where(x => x.ApplicationUserId == userId && x.RoleInProjectId == getRoleOwner);
            if (!projectUser.Any()) return ServicesResult<bool>.Failure("");
            var project = await _projectServices.GetValueByPrimaryKeyAsync(projectId);
            if (project == null) return ServicesResult<bool>.Failure("");

            if (project.IsDone = false && project.EndAt < DateTime.Now) project.StatusId = 6;
            if (project.IsDone = false && project.EndAt == DateTime.Now) project.StatusId = 5;
            if (project.IsDone = false && project.EndAt > DateTime.Now) project.StatusId = 3;
            if (project.IsDone = true && project.EndAt < DateTime.Now) project.StatusId = 7;
            if (project.IsDone = true && project.EndAt == DateTime.Now) project.StatusId = 5;
            if (project.IsDone = true && project.EndAt > DateTime.Now) project.StatusId = 4;


            if (await _projectServices.UpdateAsync(projectId, project)) return ServicesResult<bool>.Success(true);
            return ServicesResult<bool>.Failure("");
        }
        public async Task<ServicesResult<bool>> Delete(string userId, string projectId)
        {
            var getRoleOwner = (await _roleInProjectServices.GetAllAsync()).FirstOrDefault(x => x.RoleName == "Owner").Id;
            if (userId == null || projectId == null) return ServicesResult<bool>.Failure("");
            var projectUser = (await _roleApplicationUserServices.GetAllAsync()).Where(x => x.ApplicationUserId == userId && x.RoleInProjectId == getRoleOwner);
            if (!projectUser.Any()) return ServicesResult<bool>.Failure("");
            var project = await _projectServices.GetValueByPrimaryKeyAsync(projectId);
            if (project == null) return ServicesResult<bool>.Failure("");
            if (!(await _planLogic.Delete(userId, projectId)).Status) return ServicesResult<bool>.Failure("");
            if (!(await _memberLogic.Delete(userId, projectId)).Status) return ServicesResult<bool>.Failure("");
            if(await _projectServices.DeleteAsync(projectId)) return ServicesResult<bool>.Success(true);
            return ServicesResult<bool>.Failure("");
        }
    }
}
