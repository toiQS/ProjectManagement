using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using PM.Domain.DTOs;
using PM.Persistence.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PM.DomainServices
{
    class ProjectManager
    {
        private readonly IProjectServices _projectServices;
        private readonly IApplicationUserServices _applicationUserServices;
        private readonly IRoleApplicationUserInProjectServices _roleApplicationUserInProjectServices;
        private readonly IRoleInProjectServices _roleInProjectServices;
        
        public ProjectManager(IProjectServices projectServices, IApplicationUserServices applicationUserServices, IRoleApplicationUserInProjectServices applicationUserInProjectServices, IRoleInProjectServices roleInProjectServices)
        {
            _projectServices = projectServices;
            _applicationUserServices = applicationUserServices;
            _roleInProjectServices = roleInProjectServices;
            _roleApplicationUserInProjectServices = applicationUserInProjectServices;
        }
        // public async Task<IEnumerable<ProjectDTO>> GetListProjecUserJoined(string userId)
        public async Task<Dictionary<string, object>> GetListProjecUserJoined(string userId)
        {
            ///kiểm tra id người dùng có tồn tại trên hệ thống hay không, nếu không trả về 0
            ///lấy nhanh sách dự án người dùng đã tham gia 
            ///trả về tên dự án, tên người sỡ hữu, trình trạng dự án
            /// 
            // check user is existed on system
            return new Dictionary<string, object>
            {
                { "", userId },
            };
        }
        public async Task<Dictionary<string, string>> GetProjectsByName(string text)
        {
            return new Dictionary<string, string>()
                {
                    {"a", "1"},
                    {"b", "2"},
                    {"c", "3"},
                };
        }
        public async Task<int> AddProject(string userId, ProjectDTO project)
        {
            ///kiểm tra thông tin người dùng ai đang tạo, có tồn tại trên hệ thống hay chưa, nếu không trả về 0
            ///kiểm tra tên dự án có trùng lặp trong quyền sở hữu của người dùng hay không, nếu tồn đã tồn tại trả về 2
            ///kiểm tra thời gian bắt đầu dự án còn đang triển khai dự án nào khác mà người sỡ hữu có tham gia hay không, nếu có trả về 3
            ///tạo thành công trả kết quả về là 1, nếu tạo không thành công sẽ trả về 4
            ///

            //authenticate user
            var findUser = await _applicationUserServices.GetApplicationUserAsync(userId);
            if (findUser != null) return 0;
            // get list project user joined
            var arrayProjectUserHasOwned = new List<RoleApplicationUserInProjectDTO>(); //declaration a new array contains project user has owned
            var getListProjectuserJoined = await _roleApplicationUserInProjectServices.GetProjectsUserJoined(userId);
            foreach (var item in getListProjectuserJoined)
            {
                if (await _roleInProjectServices.GetNameRoleByRoleId(item.RoleInProjectId) == "Owner")
                {
                    arrayProjectUserHasOwned.Add(item);
                }
            }
            // check if project name exists
            foreach (var item in arrayProjectUserHasOwned)
            {
                var getProject = await _projectServices.GetProjectAsync(item.ProjectId);
                if (getProject.ProjectName == project.ProjectName)
                {
                    return 2;
                }
            }
            var isCreate = await _projectServices.AddAsync(project);
            if (isCreate == false) return 4;
            return 1;
        }
        public async Task<int> TemporaryDeleteProject(string userId, string projectId)
        {
            return 1;
        }
        public async Task<int> PermanentDeleteProject(string userId, string projectId)
        {
            return 1;
        }
        public async Task<int> EditInformantionProject(string userId, string projectId, ProjectDTO project)
        {
            return 1;
        }

    }
}
