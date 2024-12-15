using Microsoft.AspNetCore.Mvc;
using PM.DomainServices.ILogic;
using PM.Infrastructure.Jwts;
using PM.Infrastructure.Loggers;
using Shared.member;

namespace MemberAPIServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        #region Fields
        private readonly IAuthLogic _authLogic;
        private readonly IJwtHelper _jwtHelper;
        private readonly ILoggerHelper<MemberController> _loggerHelper;
        private readonly IMemberLogic _memberLogic;
        #endregion

        #region Constructor
        public MemberController(IAuthLogic authLogic, IJwtHelper jwtHelper, ILoggerHelper<MemberController> loggerHelper, IMemberLogic memberLogic)
        {
            _authLogic = authLogic;
            _jwtHelper = jwtHelper;
            _loggerHelper = loggerHelper;
            _memberLogic = memberLogic;
        }
        #endregion

        #region Get Methods
        /// <summary>
        /// Retrieves the members in a specific project.
        /// </summary>
        /// <param name="token">JWT token</param>
        /// <param name="projectId">Project ID</param>
        /// <returns>List of members in the project</returns>
        [HttpGet("members-in-project")]
        public async Task<IActionResult> GetMemberInProject(string token, string projectId)
        {
            try
            {
                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(projectId))
                {
                    _loggerHelper.LogWarning("Token or Project ID is null or empty.");
                    return BadRequest("Token and Project ID are required.");
                }

                var result = _jwtHelper.ParseToken(token);
                if (result == null)
                {
                    _loggerHelper.LogWarning("Failed to parse token.");
                    return BadRequest("Invalid token.");
                }

                if (result.Role == "Admin")
                {
                    _loggerHelper.LogWarning("Admin role not allowed.");
                    return BadRequest("Access denied.");
                }

                var user = await _authLogic.GetUserDetailByMail(result.Email);
                if (!user.Status)
                {
                    _loggerHelper.LogDebug(user.Message);
                    return BadRequest(user);
                }

                var members = await _memberLogic.GetMembersInProject(user.Data.UserId, projectId);
                if (!members.Status)
                {
                    _loggerHelper.LogDebug(members.Message);
                    return BadRequest(members);
                }

                return Ok(members);
            }
            catch (Exception ex)
            {
                _loggerHelper.LogError(ex, "Error fetching members in project.");
                throw;
            }
        }

        /// <summary>
        /// Retrieves a member's details by their ID.
        /// </summary>
        /// <param name="token">JWT token</param>
        /// <param name="memberId">Member ID</param>
        /// <returns>Member details</returns>
        [HttpGet("member-details")]
        public async Task<IActionResult> GetMemberByMemberId(string token, string memberId)
        {
            try
            {
                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(memberId))
                {
                    _loggerHelper.LogWarning("Token or Member ID is null or empty.");
                    return BadRequest("Token and Member ID are required.");
                }

                var result = _jwtHelper.ParseToken(token);
                if (result == null)
                {
                    _loggerHelper.LogWarning("Failed to parse token.");
                    return BadRequest("Invalid token.");
                }

                if (result.Role == "Admin")
                {
                    _loggerHelper.LogWarning("Admin role not allowed.");
                    return BadRequest("Access denied.");
                }

                var user = await _authLogic.GetUserDetailByMail(result.Email);
                if (!user.Status)
                {
                    _loggerHelper.LogDebug(user.Message);
                    return BadRequest(user);
                }

                var member = await _memberLogic.GetMemberByMemberId(user.Data.UserId, memberId);
                if (!member.Status)
                {
                    _loggerHelper.LogDebug(member.Message);
                    return BadRequest(member);
                }

                return Ok(member);
            }
            catch (Exception ex)
            {
                _loggerHelper.LogError(ex, "Error fetching member details.");
                throw;
            }
        }
        #endregion

        #region Create Methods
        /// <summary>
        /// Adds a new member to a project.
        /// </summary>
        /// <param name="token">JWT token</param>
        /// <param name="projectId">Project ID</param>
        /// <param name="addMember">New member details</param>
        /// <returns>Operation result</returns>
        [HttpPost("add-member")]
        public async Task<IActionResult> AddNewMember(string token, string projectId, AddMember addMember)
        {
            try
            {
                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(projectId) || addMember == null || !ModelState.IsValid)
                {
                    _loggerHelper.LogWarning("Invalid input.");
                    return BadRequest("Invalid input.");
                }

                var result = _jwtHelper.ParseToken(token);
                if (result == null)
                {
                    _loggerHelper.LogWarning("Failed to parse token.");
                    return BadRequest("Invalid token.");
                }

                if (result.Role == "Admin")
                {
                    _loggerHelper.LogWarning("Admin role not allowed.");
                    return BadRequest("Access denied.");
                }

                var user = await _authLogic.GetUserDetailByMail(result.Email);
                if (!user.Status)
                {
                    _loggerHelper.LogDebug(user.Message);
                    return BadRequest(user);
                }

                var resultAdd = await _memberLogic.Add(user.Data.UserId, addMember, projectId);
                if (!resultAdd.Status)
                {
                    _loggerHelper.LogDebug(resultAdd.Message);
                    return BadRequest(resultAdd);
                }

                return Ok(resultAdd);
            }
            catch (Exception ex)
            {
                _loggerHelper.LogError(ex, "Error adding member.");
                throw;
            }
        }
        #endregion

        #region Update Methods
        /// <summary>
        /// Updates information of a specific member.
        /// </summary>
        /// <param name="token">JWT token</param>
        /// <param name="memberId">Member ID</param>
        /// <param name="updateMember">Updated member details</param>
        /// <returns>Operation result</returns>
        [HttpPut("update-member")]
        public async Task<IActionResult> UpdateInfo(string token, string memberId, UpdateMember updateMember)
        {
            try
            {
                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(memberId) || updateMember == null || !ModelState.IsValid)
                {
                    _loggerHelper.LogWarning("Invalid input.");
                    return BadRequest("Invalid input.");
                }

                var result = _jwtHelper.ParseToken(token);
                if (result == null)
                {
                    _loggerHelper.LogWarning("Failed to parse token.");
                    return BadRequest("Invalid token.");
                }

                if (result.Role == "Admin")
                {
                    _loggerHelper.LogWarning("Admin role not allowed.");
                    return BadRequest("Access denied.");
                }

                var user = await _authLogic.GetUserDetailByMail(result.Email);
                if (!user.Status)
                {
                    _loggerHelper.LogDebug(user.Message);
                    return BadRequest(user);
                }

                var resultUpdate = await _memberLogic.UpdateInfo(user.Data.UserId, memberId, updateMember);
                if (!resultUpdate.Status)
                {
                    _loggerHelper.LogDebug(resultUpdate.Message);
                    return BadRequest(resultUpdate);
                }

                return Ok(resultUpdate);
            }
            catch (Exception ex)
            {
                _loggerHelper.LogError(ex, "Error updating member information.");
                throw;
            }
        }
        #endregion

        #region Delete Methods
        /// <summary>
        /// Deletes a member from a project.
        /// </summary>
        /// <param name="token">JWT token</param>
        /// <param name="memberId">Member ID</param>
        /// <returns>Operation result</returns>
        [HttpDelete("delete-member")]
        public async Task<IActionResult> Delete(string token, string memberId)
        {
            try
            {
                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(memberId))
                {
                    _loggerHelper.LogWarning("Invalid input.");
                    return BadRequest("Token and Member ID are required.");
                }

                var result = _jwtHelper.ParseToken(token);
                if (result == null)
                {
                    _loggerHelper.LogWarning("Failed to parse token.");
                    return BadRequest("Invalid token.");
                }

                if (result.Role == "Admin")
                {
                    _loggerHelper.LogWarning("Admin role not allowed.");
                    return BadRequest("Access denied.");
                }

                var user = await _authLogic.GetUserDetailByMail(result.Email);
                if (!user.Status)
                {
                    _loggerHelper.LogDebug(user.Message);
                    return BadRequest(user);
                }

                var resultDelete = await _memberLogic.Delete(user.Data.UserId, memberId);
                if (!resultDelete.Status)
                {
                    _loggerHelper.LogDebug(resultDelete.Message);
                    return BadRequest(resultDelete);
                }

                return Ok(resultDelete);
            }
            catch (Exception ex)
            {
                _loggerHelper.LogError(ex, "Error deleting member.");
                throw;
            }
        }
        #endregion
    }
}
