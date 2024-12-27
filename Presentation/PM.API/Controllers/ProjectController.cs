using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.project;
using System.Net.Http.Json;

namespace PM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        public ProjectController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:5000");
        }
        [HttpGet("joined-projects")]
        public async Task<IActionResult> GetProjectListUserHasJoined(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return BadRequest("");
            var response = await _httpClient.GetFromJsonAsync<ServicesResult<IEnumerable<IndexProject>>>($"/project/joined-projects/{token}");
            if(response == null) return BadRequest(response.Message);
            return Ok(response.Data);
        }
        [HttpGet("owned-projects")]
        public async Task<IActionResult> GetProjectListUserHasOwner(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return BadRequest("");
            var response = await _httpClient.GetFromJsonAsync<ServicesResult<IEnumerable<IndexProject>>>($"/project/owned-projects/{token}");
            if (response == null) return BadRequest(response.Message);
            return Ok(response.Data);
        }
        [HttpGet("joined-project-details")]
        public async Task<IActionResult> GetProjectDetailProjectHasJoined(string token, string projectId)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrEmpty(projectId)) return BadRequest();
            var response = await _httpClient.GetFromJsonAsync<ServicesResult<IEnumerable<IndexProject>>>($"/project/joined-project-details?token={token}&projectId={projectId}");
            if (response == null) return BadRequest(response.Message);
            return Ok(response.Data);
        }

        [HttpPost("add-project")]
        public async Task<IActionResult> AddProject(string token, string projectName, DateTime startAt, DateTime endAt, string projectDescription)
        {
            if(string.IsNullOrWhiteSpace(token) || string.IsNullOrEmpty(projectName) || string.IsNullOrEmpty(startAt.ToString()) || string.IsNullOrEmpty(endAt.ToString()) || projectDescription == null) return BadRequest();
            var model = new AddProject()
            {
                ProjectName = projectName,
                EndAt = endAt,
                ProjectDescription = projectDescription
            };
            var response = await _httpClient.PostAsJsonAsync($"/project/add-project?token={token}", model);
            if (response.IsSuccessStatusCode)
            {
                return Ok("Add success");
            }
            return BadRequest(response.Content.ReadAsStringAsync().Result);
        }
        [HttpPut("update-project")]
        public async Task<IActionResult> Update(string token, string projectId, string projectName, string projectDescription)
        {
            if(string.IsNullOrEmpty(token) || string.IsNullOrEmpty(projectName) || string.IsNullOrEmpty (projectDescription)) return BadRequest();
            var model = new UpdateProject()
            {
                ProjectDescription = projectDescription,
                ProjectName = projectName,
            };
            var response = await _httpClient.PutAsJsonAsync($"/project/update-project?token={token}", model);
            if (response.IsSuccessStatusCode)
            {
                return Ok("Update success");
            }
            return BadRequest(response.Content.ReadAsStringAsync().Result);
        }
        [HttpDelete("delete-project")]
        public async Task<IActionResult> Delete(string token, string projectId)
        {
            if(token == null || string.IsNullOrEmpty(projectId)) return BadRequest();
            var response = await _httpClient.DeleteAsync($"/project/delete-project?token={token}&projectId={projectId}");
            if (response.IsSuccessStatusCode)
            {
                return Ok("Delete success");
            }
            return BadRequest(response.Content.ReadAsStringAsync().Result);
        }
        [HttpPatch("update-is-delete")]
        public async Task<IActionResult> UpdateIsDelete(string token, string projectId)
        {
            if (token == null || string.IsNullOrEmpty(projectId)) return BadRequest();
            var response = await _httpClient.PatchAsync($"/project/update-is-delete?token={token}&projectId={projectId}",null);
            if (response.IsSuccessStatusCode)
            {
                return Ok("update status success");
            }
            return BadRequest(response.Content.ReadAsStringAsync().Result);
        }
        [HttpPatch("update-is-done")]
        public async Task<IActionResult> UpdateIsDone(string token, string projectId)
        {
            if (token == null || string.IsNullOrEmpty(projectId)) return BadRequest();
            var response = await _httpClient.PatchAsync($"/project/update-is-done?token={token}&projectId={projectId}", null);
            if (response.IsSuccessStatusCode)
            {
                return Ok("update status success");
            }
            return BadRequest(response.Content.ReadAsStringAsync().Result);
        }
        [HttpPatch("update-status")]
        public async Task<IActionResult> UpdateStatus(string token, string projectId)
        {
            if (token == null || string.IsNullOrEmpty(projectId)) return BadRequest();
            var response = await _httpClient.PatchAsync($"/project/update-status?token={token}&projectId={projectId}", null);
            if (response.IsSuccessStatusCode)
            {
                return Ok("update status success");
            }
            return BadRequest(response.Content.ReadAsStringAsync().Result);
        }
    }
}
