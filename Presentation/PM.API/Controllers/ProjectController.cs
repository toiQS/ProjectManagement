using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared;
using Shared.project;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace PM.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public ProjectController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:5000");
        }

        #region Project Retrieval

        /// <summary>
        /// Retrieves a list of projects the user has joined.
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <returns>List of joined projects</returns>
        [HttpGet("joined-projects")]
        public async Task<IActionResult> GetProjectListUserHasJoined(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return BadRequest("Token is required.");

            var response = await _httpClient.GetAsync($"/project/joined-projects?token={token}");
            if (response == null) return BadRequest("No response from the server.");

            return Ok(await response.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Retrieves a list of projects owned by the user.
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <returns>List of owned projects</returns>
        [HttpGet("owned-projects")]
        public async Task<IActionResult> GetProjectListUserHasOwner(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return BadRequest("Token is required.");

            var response = await _httpClient.GetAsync($"/project/owned-projects?token={token}");
            if (response == null) return BadRequest("No response from the server.");

            return Ok(await response.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Retrieves details of a joined project.
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="projectId">Project ID</param>
        /// <returns>Project details</returns>
        [HttpGet("joined-project-details")]
        public async Task<IActionResult> GetProjectDetailProjectHasJoined(string token, string projectId)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrEmpty(projectId))
                return BadRequest("Token and Project ID are required.");

            var response = await _httpClient.GetAsync($"/project/joined-project-details?token={token}&projectId={projectId}");
            if (response == null) return BadRequest("No response from the server.");

            return Ok(await response.Content.ReadAsStringAsync());
        }

        #endregion

        #region Project Management

        /// <summary>
        /// Adds a new project.
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="projectName">Name of the project</param>
        /// <param name="startAt">Start date</param>
        /// <param name="endAt">End date</param>
        /// <param name="projectDescription">Description of the project</param>
        /// <returns>Operation result</returns>
        [HttpPost("add-project")]
        public async Task<IActionResult> AddProject(string token, string projectName, DateTime startAt, DateTime endAt, string projectDescription)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrEmpty(projectName) || startAt == default || endAt == default || startAt >= endAt || string.IsNullOrWhiteSpace(projectDescription))
                return BadRequest("Invalid input parameters.");

            var model = new AddProject()
            {
                ProjectName = projectName,
                StartAt = startAt,
                EndAt = endAt,
                ProjectDescription = projectDescription
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync($"/project/add-project?token={token}", model);
                if (response.IsSuccessStatusCode)
                    return Ok("Project added successfully.");

                var errorContent = await response.Content.ReadAsStringAsync();
                return BadRequest($"Error adding project: {(string.IsNullOrEmpty(errorContent) ? "No details provided by the API." : errorContent)}");
            }
            catch (HttpRequestException httpEx)
            {
                return StatusCode(500, $"HTTP error: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates a project.
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="projectId">Project ID</param>
        /// <param name="projectName">Name of the project</param>
        /// <param name="projectDescription">Description of the project</param>
        /// <returns>Operation result</returns>
        [HttpPut("update-project")]
        public async Task<IActionResult> Update(string token, string projectId, string projectName, string projectDescription)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(projectName) || string.IsNullOrEmpty(projectDescription))
                return BadRequest("Invalid input parameters.");

            var model = new UpdateProject()
            {
                ProjectDescription = projectDescription,
                ProjectName = projectName,
            };

            var response = await _httpClient.PutAsJsonAsync($"/project/update-project?token={token}", model);
            if (response.IsSuccessStatusCode)
                return Ok("Update success");

            return BadRequest(await response.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Deletes a project.
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="projectId">Project ID</param>
        /// <returns>Operation result</returns>
        [HttpDelete("delete-project")]
        public async Task<IActionResult> Delete(string token, string projectId)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(projectId))
                return BadRequest("Token and Project ID are required.");

            var response = await _httpClient.DeleteAsync($"/project/delete-project?token={token}&projectId={projectId}");
            if (response.IsSuccessStatusCode)
                return Ok("Delete success");

            return BadRequest(await response.Content.ReadAsStringAsync());
        }

        #endregion

        #region Project Status Updates

        /// <summary>
        /// Updates the deletion status of a project.
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="projectId">Project ID</param>
        /// <returns>Operation result</returns>
        [HttpPatch("update-is-delete")]
        public async Task<IActionResult> UpdateIsDelete(string token, string projectId)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(projectId))
                return BadRequest("Token and Project ID are required.");

            var response = await _httpClient.PatchAsync($"/project/update-is-delete?token={token}&projectId={projectId}", null);
            if (response.IsSuccessStatusCode)
                return Ok("Update status success");

            return BadRequest(await response.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Updates the completion status of a project.
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="projectId">Project ID</param>
        /// <returns>Operation result</returns>
        [HttpPatch("update-is-done")]
        public async Task<IActionResult> UpdateIsDone(string token, string projectId)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(projectId))
                return BadRequest("Token and Project ID are required.");

            var response = await _httpClient.PatchAsync($"/project/update-is-done?token={token}&projectId={projectId}", null);
            if (response.IsSuccessStatusCode)
                return Ok("Update status success");

            return BadRequest(await response.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Updates the general status of a project.
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="projectId">Project ID</param>
        /// <returns>Operation result</returns>
        [HttpPatch("update-status")]
        public async Task<IActionResult> UpdateStatus(string token, string projectId)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(projectId))
                return BadRequest("Token and Project ID are required.");

            var response = await _httpClient.PatchAsync($"/project/update-status?token={token}&projectId={projectId}", null);
            if (response.IsSuccessStatusCode)
                return Ok("Update status success");

            return BadRequest(await response.Content.ReadAsStringAsync());
        }

        #endregion
    }
}
