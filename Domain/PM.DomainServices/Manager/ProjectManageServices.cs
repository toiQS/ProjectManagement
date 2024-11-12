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
    class ProjectManageServices : IProjectManageServices
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

        public ProjectManageServices(IProjectServices projectServices, IApplicationUserServices applicationUserServices, IRoleApplicationUserInProjectServices applicationUserInProjectServices, IRoleInProjectServices roleInProjectServices,
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
            // Declare a list to store the final result with project details
            var finalResult = new List<Dictionary<string, object>>();

            // Validate input data
            if (string.IsNullOrEmpty(userId))
            {
                finalResult.Add(new Dictionary<string, object>
                {
                    { "Message", "Invalid user ID" }
                });
                return finalResult;
            }

            // Check if user exists in the system
            var user = await _applicationUserServices.GetApplicationUserAsync(userId);
            if (user == null)
            {
                finalResult.Add(new Dictionary<string, object>
                {
                    { "Message", "User not found" }
                });
                return finalResult;
            }

            // Retrieve list of projects the user has joined
            var userProjects = await _roleApplicationUserInProjectServices.GetProjectsUserJoined(userId);
            if (userProjects == null || !userProjects.Any())
            {
                finalResult.Add(new Dictionary<string, object>
                {
                    { "Message", "No projects found for the user" }
                });
                return finalResult;
            }

            // Iterate through each project the user has joined
            foreach (var userProject in userProjects)
            {
                var project = await _projectServices.GetProjectAsync(userProject.ProjectId);
                if (project == null) continue; // Skip if project is not found

                // Determine project status based on its access status
                string status = project.IsAccessed ? "Active" : "Inactive";

                // Find the project owner
                string ownerImage ="";
                string ownerName = "";
                var projectMembers = await _roleApplicationUserInProjectServices.GetRoleApplicationUserInProjectsByProjectId(userProject.ProjectId);

                foreach (var member in projectMembers)
                {
                    var role = await _roleInProjectServices.GetNameRoleByRoleId(member.RoleInProjectId);
                    if (role == "Owner")
                    {
                        var ownerUser = await _applicationUserServices.GetApplicationUserAsync(member.ApplicationUserId);
                        if (ownerUser != null)
                        {
                            ownerName = ownerUser.UserName;
                            ownerImage = ownerUser.PathImage;
                            break;
                        }
                    }
                }

                // Add project details to final result
                finalResult.Add(new Dictionary<string, object>
                {
                    { "Project Name", project.ProjectName },
                    { "Owner", ownerName },
                    { "Image", ownerImage },
                    { "Message", "" }
                });
            }
            return finalResult;
        }

        public async Task<List<Dictionary<string, object>>> GetListAllListProjectByNameAndUserJoined(string userId, string projectName)
        {
            // Initialize the result list
            var finalResult = new List<Dictionary<string, object>>();

            // Validate input data
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectName))
            {
                finalResult.Add(new Dictionary<string, object>
                {
                    { "Message", "Invalid input data" }
                });
                return finalResult;
            }

            // Check if the user exists in the system
            var user = await _applicationUserServices.GetApplicationUserAsync(userId);
            if (user == null)
            {
                finalResult.Add(new Dictionary<string, object>
                {
                    { "Message", "User not found" }
                });
                return finalResult;
            }

            // Retrieve projects containing the search keyword in the project name
            var projects = await _projectServices.GetProjectsByProjectName(projectName);
            if (projects == null || !projects.Any())
            {
                finalResult.Add(new Dictionary<string, object>
                {
                    { "Message", "No projects found with the specified name" }
                });
                return finalResult;
            }

            // Iterate over the list of projects
            foreach (var project in projects)
            {
                // Check if the user has a role in the current project
                var userRole = await _roleApplicationUserInProjectServices.GetRoleApplicationUserInProjectByUserIdAndProjectId(userId, project.Id);
                if (userRole == null) continue; // Skip if the user is not part of the project

                // Get roles and check for project owner
                string ownerName = "";
                var projectRoles = await _roleApplicationUserInProjectServices.GetRoleApplicationUserInProjectsByProjectId(project.Id);

                foreach (var role in projectRoles)
                {
                    var roleInfo = await _roleInProjectServices.GetRoleInProjectByRoleId(role.RoleInProjectId);
                    if (roleInfo?.RoleName == "Owner")
                    {
                        ownerName = roleInfo.RoleName;
                        break; // Exit the loop once the owner is found
                    }
                }

                // Determine project status
                string status = project.IsAccessed ? "Active" : "Inactive";

                // Add project information to the final result
                finalResult.Add(new Dictionary<string, object>
                {
                    { "Project Name", project.ProjectName },
                    { "Owner", ownerName },
                    { "Status", status },
                    { "Message", "" }
                });
            }

            // Return the final list of project details
            return finalResult;
        }

        public async Task<Dictionary<string, string>> AddProject(string userId, string projectName, string projectDescription, string projectVersion, string projectStatus)
        {
            // Declare a dictionary to store the final result
            var finalResult = new Dictionary<string, string>();

            // Check if input data is valid
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectName) || string.IsNullOrEmpty(projectDescription) || string.IsNullOrEmpty(projectVersion) || string.IsNullOrEmpty(projectStatus))
            {
                finalResult.Add("Message", "Invalid input data");
                return finalResult;
            }

            // Check if the user exists
            var user = await _applicationUserServices.GetApplicationUserAsync(userId);
            if (user == null)
            {
                finalResult.Add("Message", "User not found");
                return finalResult;
            }

            // Retrieve projects the user owns
            var ownedProjects = new List<RoleApplicationUserInProjectDTO>();
            var userProjects = await _roleApplicationUserInProjectServices.GetProjectsUserJoined(userId);

            if (userProjects != null)
            {
                foreach (var projectRole in userProjects)
                {
                    // Check if the user is the owner of any projects
                    if (await _roleInProjectServices.GetNameRoleByRoleId(projectRole.RoleInProjectId) == "Owner")
                    {
                        ownedProjects.Add(projectRole);
                    }
                }
            }

            // Check if the user already owns projects with the same name
            foreach (var ownedProject in ownedProjects)
            {
                var existingProject = await _projectServices.GetProjectAsync(ownedProject.ProjectId);
                if (existingProject?.ProjectName == projectName)
                {
                    finalResult.Add("Message", "A project with this name already exists under your ownership");
                    return finalResult;
                }
            }
           

            // Generate a unique ID for the new project role
            var header = $"{new Random().Next(1, 100)}-{new Random().Next(1, 100)}-{new Random().Next(1, 100)}";
            var now = DateTime.Now;

             //Initialize a project entity
            var project = new ProjectDTO
            {
                 Id = $"1001-{header}-{now}",
            };
            //Initialize a role of application user in this project
            var projectRoleDTO = new RoleApplicationUserInProjectDTO
            {
                Id = $"1003-{header}-{now}",
                ProjectId = project.Id,
                RoleInProjectId = "1002-1",  
                ApplicationUserId = userId
            };

            // Add the user as the owner of the new project
            var roleAdded = await _roleApplicationUserInProjectServices.AddAsync(projectRoleDTO);
            if (!roleAdded)
            {
                finalResult.Add("Message", "Failed to assign user as project owner");
                return finalResult;
            }

            // Attempt to create the project
            var projectCreated = await _projectServices.AddAsync(project);
            if (!projectCreated)
            {
                finalResult.Add("Message", "Error occurred while creating the project");
                return finalResult;
            }

            // Successful creation
            finalResult.Add("Message", "Project created successfully");
            return finalResult;
        }

        public async Task<Dictionary<string, string>> TemporaryDeleteProject(string userId, string projectId)
        {
            // Initialize a dictionary to store the result message
            var finalResult = new Dictionary<string, string>();

            // Validate input data
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectId))
            {
                finalResult.Add("Message", "Invalid input data");
                return finalResult;
            }

            // Check if the user exists
            var user = await _applicationUserServices.GetApplicationUserAsync(userId);
            if (user == null)
            {
                finalResult.Add("Message", "User not found");
                return finalResult;
            }

            // Verify the user's role in the project
            var userRoleInProject = await _roleApplicationUserInProjectServices.GetRoleApplicationUserInProjectByUserIdAndProjectId(userId, projectId);
            if (userRoleInProject == null)
            {
                finalResult.Add("Message", "User is not associated with this project");
                return finalResult;
            }

            // Check if the user is the project owner
            var roleName = await _roleInProjectServices.GetNameRoleByRoleId(userRoleInProject.RoleInProjectId);
            if (roleName != "Owner")
            {
                finalResult.Add("Message", "User does not have permission to delete this project");
                return finalResult;
            }

            // Retrieve the project information
            var project = await _projectServices.GetProjectAsync(projectId);
            if (project == null)
            {
                finalResult.Add("Message", "Project not found");
                return finalResult;
            }

            // Mark the project as deleted
            project.IsDeleted = true;
            var updateResult = await _projectServices.UpdateAsync(projectId, project);

            // Check if the update was successful
            if (updateResult)
            {
                finalResult.Add("Message", "Project has been temporarily deleted");
            }
            else
            {
                finalResult.Add("Message", "Failed to update the project status in the database");
            }

            return finalResult;
        }

        public async Task<Dictionary<string, string>> PermanentDeleteProject(string userId, string projectId)
        {
            ///kiểm tra tính hợp lệ của dữ liệu đầu vào
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
            //is check data input null
            if (userId == null || projectId == null)
            {
                finalResult.Add("Message:", "Data input is invalid");
                return finalResult;
            }
            //authenticate user
            var findUser = await _applicationUserServices.GetApplicationUserAsync(userId);
            if (findUser != null)
            {
                finalResult.Add("Message:", "Can't find User");
                return finalResult;
            }
            // check role of user in project
            var getRoleIdOfUserInProject = await _roleApplicationUserInProjectServices.GetRoleApplicationUserInProjectByUserIdAndProjectId(userId, projectId);
            if (getRoleIdOfUserInProject == null)
            {
                finalResult.Add("Message:", "Can't get role id of user in project");
                return finalResult;
            }
            var getRoleName = await _roleInProjectServices.GetNameRoleByRoleId(getRoleIdOfUserInProject.RoleInProjectId);
            if (getRoleName == null)
            {
                finalResult.Add("Message:", "Can't get role name");
                return finalResult;
            }
            if (getRoleName == "Owner")
            {
                var getProject = await _projectServices.GetProjectAsync(projectId);
                if (getProject == null)
                {
                    finalResult.Add("Message:", "Can't get project");
                    return finalResult;
                }
                //todo all data have reference with project data

                var findAllMemberInProject = await _roleApplicationUserInProjectServices.GetRoleApplicationUserInProjectsByProjectId(projectId);
                var findAllPlanInProject = await _planInProjectServices.GetPlanInProjectsByProjectId(projectId);
                var findAllPositionInProject = await _positionInProjectServices.GetAllPositionInProjectByProjectId(projectId);
                if (findAllMemberInProject == null || findAllPlanInProject == null || findAllPositionInProject == null)
                {
                    finalResult.Add("Message:", "There are some issue when get data. This issue can occured while get data form member in project or plan in project or position in project");
                    return finalResult;
                }
                //delete data application user in project and position in project
                foreach (var member in findAllMemberInProject)
                {
                    foreach (var position in findAllPositionInProject)
                    {
                        // find and delele all position work of member in project
                        var findPositionWorkOfMemberInProject = await _positionWorkOfMemberServices.GetPositionWorkOfMemberByRoleApplicationUserIdAndPositionInProjectId(member.Id, position.Id);
                        if (findPositionWorkOfMemberInProject != null)
                        {
                            //find and delete member in task of project
                            var findMemberInTask = await _memberInTaskServices.GetAllMemberInTaskByPositionWorkOfMemberId(findPositionWorkOfMemberInProject.Id);
                            if (findMemberInTask == null)
                            {
                                finalResult.Add("Message:", "Can't get member in project ");
                                return finalResult;
                            }
                            foreach (var memberInTask in findMemberInTask)
                            {
                                var todoDelete = await _memberInTaskServices.RemoveAsync(memberInTask.Id);
                                if (todoDelete == false)
                                {
                                    finalResult.Add("Message:", "Can't delete member in task " + memberInTask.Id + "");
                                    return finalResult;
                                }
                                if (todoDelete == true) continue;
                            }
                            var deletePositionWorkOfMemberInProject = await _positionWorkOfMemberServices.RemoveAsync(findPositionWorkOfMemberInProject.Id);
                            if (deletePositionWorkOfMemberInProject == true) continue;
                            else
                            {
                                finalResult.Add("Message:", "Can't delete position work of member in project" + findPositionWorkOfMemberInProject.Id + "");
                                return finalResult;
                            }

                        }
                        //todo delete position work of memeber in project
                        var deletePositionInProject = await _positionWorkOfMemberServices.RemoveAsync(position.Id);
                        if (deletePositionInProject == true) continue;
                        else
                        {
                            finalResult.Add("Message:", "Can't delete position in project" + position.Id + "");
                            return finalResult;
                        }
                    }
                    // todo delete application user in project
                    var deleteMemberInProject = await _roleApplicationUserInProjectServices.RemoveAsync(member.Id);
                    if (deleteMemberInProject == true) continue;
                    else
                    {
                        finalResult.Add("Message:", "Can't delete member in project");
                        return finalResult;
                    }

                }

                //find and delete all plans in project
                foreach (var item in findAllPlanInProject)
                {
                    var getPlan = await _planInProjectServices.GetByIdAsync(item.PlanId);
                    if (getPlan == null)
                    {
                        finalResult.Add("Message:", "Can't get plan in this project" + item.ProjectId + ".");
                        return finalResult;
                    }
                    else
                    {
                        // find task in plan of project
                        var findtaskInPlan = await _taskInPlanServices.GetAllTaskInPlanByPlanId(getPlan.Id);
                        if (findtaskInPlan != null)
                        {
                            //find task of this project
                            foreach (var task in findtaskInPlan)
                            {
                                // todo delete task in plan of this project
                                var deleteTaskInPlan = await _taskInPlanServices.RemoveAsync(task.Id);
                                if (deleteTaskInPlan == false)
                                {
                                    finalResult.Add("Message:", "Can't delete task in plan of this project");
                                    return finalResult;
                                }

                                var findTask = await _taskServices.GetTaskById(task.TaskId);
                                if (findTask == null)
                                {
                                    finalResult.Add("Message:", "Can't get task in this project" + projectId + ".");
                                    return finalResult;
                                }
                                else
                                {
                                    //todo delete task of this project
                                    var deleteTask = await _taskServices.RemoveAsync(findTask.Id);
                                    if (deleteTask == false)
                                    {
                                        finalResult.Add("Message:", "Can't delete task in this project" + projectId + ".");
                                        return finalResult;
                                    }
                                    else continue;
                                }
                            }
                            var deletePlan = await _planServices.RemoveAsync(getPlan.Id);
                            if (deletePlan == false)
                            {
                                finalResult.Add("Message:", "There are some issue when trying delete a plan of this project");
                                return finalResult;
                            }
                        }
                        else
                        {
                            finalResult.Add("Message:", "Can't get task in plan of this project" + projectId + ".");
                            return finalResult;
                        }

                    }
                    //todo delete plan of project
                    var deletePlanInProject = await _planInProjectServices.RemoveAsync(item.Id);
                    if (deletePlanInProject == false)
                    {
                        finalResult.Add("Message:", "Can't delete plan in project");
                        return finalResult;
                    }
                    else continue;
                }
                //todo delete project
                var todo = await _projectServices.RemoveAsync(projectId);
                if (todo == true)
                {
                    finalResult.Add("Message:", "Project was deleted");
                    return finalResult;
                }
                else
                {
                    finalResult.Add("Message:", "Can't update databbase");
                    return finalResult;
                }

            }
            return finalResult;
        }
        public async Task<Dictionary<string, string>> EditInformationProject(string userId, string projectId, string projectName, string projectDescription, string projectVersion, string projectStatus)
        {
            // Initialize result dictionary to store response messages
            var finalResult = new Dictionary<string, string>();

            // Validate input data
            if ( string.IsNullOrEmpty(projectId) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(projectName) || string.IsNullOrEmpty(projectDescription) || string.IsNullOrEmpty(projectVersion) || string.IsNullOrEmpty(projectStatus))
            {
                finalResult.Add("Message", "Invalid input data");
                return finalResult;
            }

            // Verify user existence
            var user = await _applicationUserServices.GetApplicationUserAsync(userId);
            if (user == null)
            {
                finalResult.Add("Message", "User not found");
                return finalResult;
            }

            // Verify user role in the project
            var userRoleInProject = await _roleApplicationUserInProjectServices.GetRoleApplicationUserInProjectByUserIdAndProjectId(userId, projectId);
            if (userRoleInProject == null)
            {
                finalResult.Add("Message", "User does not have a role in this project");
                return finalResult;
            }

            var roleName = await _roleInProjectServices.GetNameRoleByRoleId(userRoleInProject.RoleInProjectId);
            if (roleName == null || roleName != "Owner")
            {
                finalResult.Add("Message", "User is not authorized to edit this project");
                return finalResult;
            }

            // Retrieve projects the user owns
            var ownedProjects = new List<RoleApplicationUserInProjectDTO>();
            var userProjects = await _roleApplicationUserInProjectServices.GetProjectsUserJoined(userId);

            if (userProjects != null)
            {
                foreach (var projectRole in userProjects)
                {
                    // Check if the user is the owner of any projects
                    if (await _roleInProjectServices.GetNameRoleByRoleId(projectRole.RoleInProjectId) == "Owner")
                    {
                        ownedProjects.Add(projectRole);
                    }
                }
            }

            // Check if the user already owns projects with the same name
            foreach (var ownedProject in ownedProjects)
            {
                var item = await _projectServices.GetProjectAsync(ownedProject.ProjectId);
                if (item?.ProjectName == projectName)
                {
                    finalResult.Add("Message", "A project with this name already exists under your ownership");
                    return finalResult;
                }
            }

            // Retrieve the project and validate its existence
            var existingProject = await _projectServices.GetProjectAsync(projectId);
            if (existingProject == null)
            {
                finalResult.Add("Message", "Project not found");
                return finalResult;
            }

            // Update project attributes with new values
            existingProject.ProjectName = projectName;
            existingProject.ProjectDescription = projectDescription;
            existingProject.ProjectVersion = projectVersion;
            existingProject.Projectstatus = projectStatus;

            // Attempt to update the project
            var updateSuccess = await _projectServices.UpdateAsync(projectId, existingProject);
            if (updateSuccess)
            {
                finalResult.Add("Message", "Project successfully updated");
            }
            else
            {
                finalResult.Add("Message", "Failed to update project in database");
            }

            return finalResult;
        }
    }
}
