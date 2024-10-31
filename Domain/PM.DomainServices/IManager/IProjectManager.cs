using PM.Domain.DTOs;
namespace PM.DomainServices.IManager
{
    public interface IProjectManager
    {
        public Task<List<Dictionary<string, object>>> GetListProjecUserJoined(string userId);
        public Task<List<Dictionary<string,object>>> GetListAllListProjectByNameAndUserJoined(string userId, string projectName);
        public Task<Dictionary<string,string>> AddProject(string userId, ProjectDTO project);
        public Task<Dictionary<string,string>> TemporaryDeleteProject(string userId, string projectId);
        public Task<Dictionary<string,string>> PermanentDeleteProject(string userId, string projectId);
        public Task<Dictionary<string, string>> EditInformationProject(string userId, string projectId, ProjectDTO project);
    }
}