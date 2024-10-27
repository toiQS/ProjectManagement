using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
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
        public async Task<List<Dictionary<string, object>>> GetListProjecUserJoined(string userId)
        {
            ///kiểm tra id người dùng có tồn tại trên hệ thống hay không,
            ///lấy nhanh sách dự án người dùng đã tham gia 
            ///trả về cuối cùng tên dự án, tên người sỡ hữu, trình trạng dự án
            ///các thông tin trả về sẽ dùng dictionary phù hợp cho nhiều loại thông tin và dữ liệu trả về linh hoạt hơn
            ///


            // khai báo kiểu dữ liệu trả về
            var finalResult = new List<Dictionary<string, object>>();
            // check user is existed on system
            var findUser = await _applicationUserServices.GetApplicationUserAsync(userId);
            if(findUser == null)
            {
                // return new Dictionary<string, object>()
                // {
                //     {"Message","Can't find User"}
                // };
                finalResult.Add(new Dictionary<string, object>
                {
                   {"Message","Can't find User"}
                });
                return finalResult;
            }
            // get list project user joined
            var listRoleApplicationUserInProject = await _roleApplicationUserInProjectServices.GetProjectsUserJoined(userId);
            if(listRoleApplicationUserInProject == null)
            {
                // return new Dictionary<string, object>()
                // {
                //     {"Message","Can't find any project user joined"}
                // };
                finalResult.Add(new Dictionary<string, object>
                {
                    {"Message","Can't find any project user joined"}
                });
                return finalResult;
            }
            
            foreach(var item in listRoleApplicationUserInProject)
            {
                var project = await _projectServices.GetProjectAsync(item.ProjectId);
                string status = "";
                if(project != null)
                {
                    
                    finalResult.Add(new Dictionary<string, object>
                    {
                        
                        {"Project Name:", project.ProjectName},
                        {"Owner:",""},
                        {"Status:",""}
                    });
                }
                else
                {
                    finalResult.Add(new Dictionary<string, object>()
                    {
                        {"Message","Can't find project or can't get owner name."}
                    });
                }
            }
            return finalResult;
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
