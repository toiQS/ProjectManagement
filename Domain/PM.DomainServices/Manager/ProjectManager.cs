using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.IdentityModel.Tokens;
using PM.Domain.DTOs;
using PM.DomainServices.IManager;
using PM.Persistence.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;


namespace PM.DomainServices.Manager
{
    class ProjectManager : IProjectManager
    {
        private readonly IProjectServices _projectServices;
        private readonly IApplicationUserServices _applicationUserServices;
        private readonly IRoleApplicationUserInProjectServices _roleApplicationUserInProjectServices;
        private readonly IRoleInProjectServices _roleInProjectServices;
        private readonly IPlanServices _planServices;
        private readonly IPlanInProjectServices _planInProjectServices;
        private readonly IPositionInProjectServices _positionInProjectServices;
        private readonly IPositionWorkOfMemberServices _positionWorkOfMemberServices;
        private readonly ITaskInPlanServices _taskInPlanServices;
        private readonly IMemberInTaskServices _memberInTaskServices;
        private readonly ITaskServices _taskServices;
        
        public ProjectManager(IProjectServices projectServices, IApplicationUserServices applicationUserServices, IRoleApplicationUserInProjectServices applicationUserInProjectServices, IRoleInProjectServices roleInProjectServices,
            IPlanServices planServices, IPlanInProjectServices planInProjectServices, IPositionInProjectServices positionInProjectServices, IPositionWorkOfMemberServices positionWorkOfMemberServices,
            ITaskInPlanServices taskInPlanServices, IMemberInTaskServices memberInTaskServices, ITaskServices taskServices)
        {
            _projectServices = projectServices;
            _applicationUserServices = applicationUserServices;
            _roleInProjectServices = roleInProjectServices;
            _roleApplicationUserInProjectServices = applicationUserInProjectServices; 
            _planServices = planServices;
            _planInProjectServices = planInProjectServices;
            _positionInProjectServices = positionInProjectServices;
            _positionWorkOfMemberServices = positionWorkOfMemberServices;
            _taskServices = taskServices;
            _taskInPlanServices = taskInPlanServices;
            _memberInTaskServices = memberInTaskServices;
            _taskInPlanServices = taskInPlanServices;
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
        public async Task<Dictionary<string,string>> TemporaryDeleteProject(string userId, string projectId)
        {
            ///kiểm tra người dùng có trên hệ thống hay không
            ///kiểm tra người dùng hiện tại đang thực hiện lệnh phải chủ sở hữu phải không
            ///lấy thông tin cơ bản dự bán kiểm tra một lần nữa
            ///thực hiện lệnh kiểm tra lệnh có hoạt động hay không
            ///thực hiện việc trả thông báo một cách trực quan
            ///
            //declare value return 
            var finalResult = new Dictionary<string, string>();
            //authenticate user
            var findUser = await _applicationUserServices.GetApplicationUserAsync(userId);
            if (findUser != null)
            {
                finalResult.Add("Message:","Can't find User");
                return finalResult;
            }
            // check role of user in project
            var getRoleIdOfUserInProject = await _roleApplicationUserInProjectServices.GetRoleApplicationUserInProjectByUserIdAndProjectId(userId,projectId);
            if (getRoleIdOfUserInProject == null)
            {
                finalResult.Add("Message:","Can't get role id of user in project");
                return finalResult;
            }
            var getRoleName =await _roleInProjectServices.GetNameRoleByRoleId(getRoleIdOfUserInProject.RoleInProjectId);
            if (getRoleName == null)
            {
                finalResult.Add("Message:","Can't get role name");
                return finalResult;
            }
            if(getRoleName == "Owner")
            {
                var getProject = await _projectServices.GetProjectAsync(projectId);
                if (getProject == null)
                {
                    finalResult.Add("Message:","Can't get project");
                    return finalResult;
                }
                //todo update data in the database IsDeleted = true
                getProject.IsDeleted = true;
                var todo = await _projectServices.UpdateAsync(projectId,getProject);
                if(todo == true)
                {
                    finalResult.Add("Message:","This status project is no access");
                    return finalResult;
                }
                else
                {
                    finalResult.Add("Message:","Can't update databbase");
                    return finalResult;
                }

            }
            return finalResult;
        }
        public async Task<Dictionary<string,string>> PermanentDeleteProject(string userId, string projectId)
        {
            ///kiểm tra người dùng có trên hệ thống hay không
            ///kiểm tra người dùng hiện tại đang thực hiện lệnh phải chủ sở hữu phải không
            ///lấy thông tin cơ bản dự bán kiểm tra một lần nữa
            ///thực hiện lệnh kiểm tra lệnh có hoạt động hay không
            ///xóa viễn viễn xóa trực tiếp toàn bộ dữ liệu trong các bảng liên quan bao gồm: 
            ///vai trò người dùng trong dự án, nhiệm vụ, kế hoạch, vị trí công việc của thành viên trong dự án, vị trí công việc trong dự án, người thực hiện nhiệm vụ trong dự án
            ///thực hiện việc trả thông báo một cách trực quan
            ///
            //declare value return 
            var finalResult = new Dictionary<string, string>();
            //authenticate user
            var findUser = await _applicationUserServices.GetApplicationUserAsync(userId);
            if (findUser != null)
            {
                finalResult.Add("Message:","Can't find User");
                return finalResult;
            }
            // check role of user in project
            var getRoleIdOfUserInProject = await _roleApplicationUserInProjectServices.GetRoleApplicationUserInProjectByUserIdAndProjectId(userId,projectId);
            if (getRoleIdOfUserInProject == null)
            {
                finalResult.Add("Message:","Can't get role id of user in project");
                return finalResult;
            }
            var getRoleName =await _roleInProjectServices.GetNameRoleByRoleId(getRoleIdOfUserInProject.RoleInProjectId);
            if (getRoleName == null)
            {
                finalResult.Add("Message:","Can't get role name");
                return finalResult;
            }
            if(getRoleName == "Owner")
            {
                var getProject = await _projectServices.GetProjectAsync(projectId);
                if (getProject == null)
                {
                    finalResult.Add("Message:","Can't get project");
                    return finalResult;
                }
                //todo all data have reference with project data
                
                var findAllMemberInProject = await _roleApplicationUserInProjectServices.GetRoleApplicationUserInProjectsByProjectId(projectId);
                var findAllPlanInProject = await _planInProjectServices.GetPlanInProjectsByProjectId(projectId);
                var findAllPositionInProject = await _positionInProjectServices.GetAllPositionInProjectByProjectId(projectId);
                if (findAllMemberInProject == null || findAllPlanInProject == null || findAllPositionInProject == null)
                {
                    finalResult.Add("Message:","There are some issue when get data. This issue can occured while get data form member in project or plan in project or position in project");
                    return finalResult;
                }
                //delete data application user in project and position in project
                foreach(var member in findAllMemberInProject)
                {
                    foreach(var position in findAllPositionInProject)
                    {
                        // find and delele all position work of member in project
                        var findPositionWorkOfMemberInProject = await _positionWorkOfMemberServices.GetPositionWorkOfMemberByRoleApplicationUserIdAndPositionInProjectId(member.Id,position.Id);
                        if(findPositionWorkOfMemberInProject != null) 
                        {
                            //find and delete member in task of project
                            var findMemberInTask = await _memberInTaskServices.GetAllMemberInTaskByPositionWorkOfMemberId(findPositionWorkOfMemberInProject.Id);
                            if (findMemberInTask == null)
                            {
                                finalResult.Add("Message:","Can't get member in project ");
                                return finalResult;
                            }
                            foreach(var memberInTask in findMemberInTask)
                            {
                                var todoDelete = await _memberInTaskServices.RemoveAsync(memberInTask.Id);
                                if(todoDelete == false)
                                {
                                    finalResult.Add("Message:","Can't delete member in task " + memberInTask.Id + "");
                                    return finalResult;
                                }
                                if(todoDelete == true) continue;
                            }
                            var deletePositionWorkOfMemberInProject = await _positionWorkOfMemberServices.RemoveAsync(findPositionWorkOfMemberInProject.Id);
                            if(deletePositionWorkOfMemberInProject == true ) continue;
                            else
                            {
                                finalResult.Add("Message:","Can't delete position work of member in project" + findPositionWorkOfMemberInProject.Id + "");
                                return finalResult;
                            }
                            
                        }
                        //todo delete position work of memeber in project
                        var deletePositionInProject = await _positionWorkOfMemberServices.RemoveAsync(position.Id);
                        if(deletePositionInProject == true ) continue;
                        else
                        {
                            finalResult.Add("Message:","Can't delete position in project"+position.Id+"");
                            return finalResult;
                        }
                    }
                    // todo delete application user in project
                    var deleteMemberInProject = await _roleApplicationUserInProjectServices.RemoveAsync(member.Id);
                    if(deleteMemberInProject == true) continue;
                    else
                    {
                        finalResult.Add("Message:","Can't delete member in project");
                        return finalResult;
                    }
                    
                }
                
                //find and delete all plans in project
                foreach(var item in findAllPlanInProject)
                {
                    var getPlan = await _planInProjectServices.GetByIdAsync(item.PlanId);
                    if(getPlan == null)
                    {
                        finalResult.Add("Message:","Can't get plan in this project"+item.ProjectId+".");
                        return finalResult;
                    }
                    else
                    {
                        // find task in plan of project
                        var findtaskInPlan = await _taskInPlanServices.GetAllTaskInPlanByPlanId(getPlan.Id);
                        if(findtaskInPlan != null)
                        {
                            //find task of this project
                            foreach(var task in findtaskInPlan)
                            {
                                // todo delete task in plan of this project
                                var deleteTaskInPlan = await _taskInPlanServices.RemoveAsync(task.Id);
                                if (deleteTaskInPlan == false)
                                {
                                    finalResult.Add("Message:","Can't delete task in plan of this project");
                                    return finalResult;
                                }
                                
                                var findTask = await _taskServices.GetTaskById(task.TaskId);
                                if(findTask == null)
                                {
                                    finalResult.Add("Message:","Can't get task in this project"+ projectId+".");
                                    return finalResult;
                                }
                                else
                                {
                                    //todo delete task of this project
                                    var deleteTask = await _taskServices.RemoveAsync(findTask.Id);
                                    if(deleteTask == false)
                                    {
                                        finalResult.Add("Message:","Can't delete task in this project"+projectId+".");
                                        return finalResult;
                                    }
                                    else continue;
                                }
                            }
                        }
                        else
                        {
                            finalResult.Add("Message:","Can't get task in plan of this project"+ projectId+".");
                            return finalResult;
                        }

                    }
                    //todo delete plan of project
                    var deletePlan = await _planInProjectServices.RemoveAsync(item.Id);
                    if (deletePlan == false)
                    {
                        finalResult.Add("Message:","Can't delete plan in project");
                        return finalResult;
                    }
                    else continue;
                }
                //todo delete project
                var todo = await _projectServices.RemoveAsync(projectId);
                if(todo == true)
                {
                    finalResult.Add("Message:","Project was deleted");
                    return finalResult;
                }
                else
                {
                    finalResult.Add("Message:","Can't update databbase");
                    return finalResult;
                }

            }
            return finalResult;
        }
        public async Task<Dictionary<string,string>> EditInformantionProject(string userId, string projectId, ProjectDTO project)
        {
            ///kiểm tra người dùng có trên hệ thống hay không
            ///kiểm tra người dùng hiện tại đang thực hiện lệnh phải chủ sở hữu phải không
            ///lấy thông tin cơ bản dự bán kiểm tra một lần nữa
            ///thực hiện lệnh kiểm tra lệnh có hoạt động hay không
            ///thực hiện việc trả thông báo một cách trực quan
            ///
            //declare value return 
            var finalResult = new Dictionary<string, string>();
            //authenticate user
            var findUser = await _applicationUserServices.GetApplicationUserAsync(userId);
            if (findUser != null)
            {
                finalResult.Add("Message:","Can't find User");
                return finalResult;
            }
            // check role of user in project
            var getRoleIdOfUserInProject = await _roleApplicationUserInProjectServices.GetRoleApplicationUserInProjectByUserIdAndProjectId(userId,projectId);
            if (getRoleIdOfUserInProject == null)
            {
                finalResult.Add("Message:","Can't get role id of user in project");
                return finalResult;
            }
            var getRoleName =await _roleInProjectServices.GetNameRoleByRoleId(getRoleIdOfUserInProject.RoleInProjectId);
            if (getRoleName == null)
            {
                finalResult.Add("Message:","Can't get role name");
                return finalResult;
            }
            if(getRoleName == "Owner")
            {
                var getProject = await _projectServices.GetProjectAsync(projectId);
                if (getProject == null)
                {
                    finalResult.Add("Message:","Can't get project");
                    return finalResult;
                }
                // set up new value for project attribute
                getProject.ProjectName = project.ProjectName;
                getProject.ProjectDescription = project.ProjectDescription;
                getProject.ProjectVersion = project.ProjectVersion;
                getProject.Projectstatus = project.Projectstatus;
                var todo = await _projectServices.UpdateAsync(projectId,getProject);
                if(todo == true)
                {
                    finalResult.Add("Message:","Project was just update");
                    return finalResult;
                }
                else
                {
                    finalResult.Add("Message:","Can't update databbase");
                    return finalResult;
                }

            }
            return finalResult;
        }

    }
}
