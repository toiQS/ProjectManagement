using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.AspNetCore.Identity;
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
                

                var listMember = await _roleApplicationUserInProjectServices.GetRoleApplicationUserInProjectsByProjectId(item.ProjectId);
                if(listMember == null) 
                {
                    finalResult.Add(new Dictionary<string, object>
                    {
                        {"Message",$"Can't find memeber in project {item.ProjectId}"}
                    });
                    return finalResult;
                }
                string OwnerName = "";
                foreach(var member in listMember)
                {
                    var findOwn = await _roleInProjectServices.GetNameRoleByRoleId(member.RoleInProjectId);
                    if( findOwn == "Owner")
                    {
                        var user = await _applicationUserServices.GetApplicationUserAsync(member.ApplicationUserId);
                        if(user != null) 
                        {
                            finalResult.Add(new Dictionary<string, object>
                            {
                                {"Message",$"Can't find User in Project {item.ProjectId}"}
                            });
                            return finalResult;
                        }
                        OwnerName = user.UserName;
                    }
                    else
                    {
                        finalResult.Add(new Dictionary<string, object>
                        {
                            {"Message",$"Can't find role user in project {item.ProjectId}"}
                        });
                        return finalResult;
                    }
                }
                if(project != null)
                {
                    if(project.IsAccessed == true ) status = "Đang hoạt động";
                    else status = "Không hoạt động";
                    finalResult.Add(new Dictionary<string, object>
                    {
                        
                        {"Project Name:", project.ProjectName},
                        {"Owner:",""},
                        {"Status:",status}
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
        public async Task<List<Dictionary<string,object>>> GetListAllListProjectByNameAndUserJoined(string userId, string projectName)
        {
            ///kiểm tra người dùng có tồn tại trên hệ thống hay không
            ///lấy thông tin toàn bộ thông tin dự án mà người dùng đã tham gia
            ///kiểm tra những dự án nào có chứa từ khóa đang tìm kiếm
            ///trả về danh sanh sách dữ liệu
            ///

            //declare a value return 
            var finalResult = new List<Dictionary<string, object>>();
            //check user is existed in system
             var findUser = await _applicationUserServices.GetApplicationUserAsync(userId);
            if(findUser == null)
            {
                finalResult.Add(new Dictionary<string, object>
                {
                   {"Message","Can't find User"}
                });
                return finalResult;
            }
            //get list project contain project name value
            var getProjectListContainProjectName = await _projectServices.GetProjectsByProjectName(projectName);
            if(getProjectListContainProjectName == null)
            {
                finalResult.Add(new Dictionary<string, object>
                {
                    {"Message","Can't get data project"}
                });
                return finalResult;
            }
            string roleName="";
            //Get list project name user joined
            foreach(var item in getProjectListContainProjectName)
            {
                var isCheck = await _roleApplicationUserInProjectServices.GetRoleApplicationUserInProjectByUserIdAndProjectId(userId, item.Id);
                if(isCheck != null) 
                {
                    var findOwn = await _roleApplicationUserInProjectServices.GetRoleApplicationUserInProjectsByProjectId(item.Id);
                    if (findOwn == null)
                    {
                        finalResult.Add(new Dictionary<string,object>()
                        {
                            {"Message","Cant't get list member in this project " + item.ProjectName}
                        });
                        return finalResult;
                    }
                    
                    foreach (var itemRole in findOwn)
                    {
                        var getRoleNameInProject = await _roleInProjectServices.GetRoleInProjectByRoleId(itemRole.RoleInProjectId);
                        if (getRoleNameInProject == null)
                        {
                            finalResult.Add(new Dictionary<string, object>()
                            {
                                {"Message","Can't get list role information"}
                            });
                            return finalResult;
                        }
                        if(getRoleNameInProject.RoleName == "Owner")
                        {
                            roleName = getRoleNameInProject.RoleName;
                        }
                    }
                    string status;
                    if(item.IsAccessed == true ) status = "Đang hoạt động";
                    else status = "Không hoạt động";
                    if(string.IsNullOrEmpty(roleName))
                    {
                        finalResult.Add(new Dictionary<string, object>()
                        {
                            {"Message","Can't assign data"}
                        });
                    }
                    finalResult.Add(new Dictionary<string,object>()
                    {
                        {"Project Name:",item.ProjectName},
                        {"Owner:",roleName},
                        {"Status:",status}
                    });
                }
                finalResult.Add(new Dictionary<string, object>
                {
                    {"Message","Can't get project user joined by project id and user id"}
                });
                return finalResult;
            }
            return finalResult;
        }
        public async Task<Dictionary<string,string>> AddProject(string userId, ProjectDTO project)
        {
            ///kiểm tra thông tin người dùng ai đang tạo, có tồn tại trên hệ thống hay chưa, nếu không trả về 0
            ///kiểm tra tên dự án có trùng lặp trong quyền sở hữu của người dùng hay không, nếu tồn đã tồn tại trả về 2
            ///kiểm tra thời gian bắt đầu dự án còn đang triển khai dự án nào khác mà người sỡ hữu có tham gia hay không, nếu có trả về 3
            ///tạo thành công trả kết quả về là 1, nếu tạo không thành công sẽ trả về 4
            ///
            
            //declare a value return
            var finalResult = new Dictionary<string, string>();
            //authenticate user
            var findUser = await _applicationUserServices.GetApplicationUserAsync(userId);
            if (findUser != null)
            {
                finalResult.Add("Message:","Can't find User");
                return finalResult;
            }
            // get list project user joined
            var arrayProjectUserHasOwned = new List<RoleApplicationUserInProjectDTO>(); //declaration a new array contains project user has owned
            var getListProjectuserJoined = await _roleApplicationUserInProjectServices.GetProjectsUserJoined(userId);
            if(getListProjectuserJoined == null)
            {
                finalResult.Add("Message:","Can't get list project user joined");
            }
            foreach (var item in getListProjectuserJoined)
            {
                if (await _roleInProjectServices.GetNameRoleByRoleId(item.RoleInProjectId) == "Owner")
                {
                    arrayProjectUserHasOwned.Add(item);
                }
            }
            if (arrayProjectUserHasOwned.Count() == 0)
            {
                finalResult.Add("Message:","Can't find project user has owner");
                return finalResult;
            }
            // check if project name exists
            foreach (var item in arrayProjectUserHasOwned)
            {
                var getProject = await _projectServices.GetProjectAsync(item.ProjectId);
                if (getProject.ProjectName == project.ProjectName)
                {
                    finalResult.Add("Message:","There is a project, has name is same with new project");
                    return finalResult;
                }
            }
            var isCreate = await _projectServices.AddAsync(project);
            if (isCreate == false)
            {
                finalResult.Add("Message:","there are some error when creating new project");
                return finalResult;
            }
            finalResult.Add("Message:","creating is success");
            return finalResult;
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
