using Shared.appUser;

namespace PM.Infrastructure.Jwts
{
    /// <summary>
    /// Interface defining JWT helper methods for token generation and parsing.
    /// </summary>
    public interface IJwtHelper
    {
        #region Token Generation

        /// <summary>
        /// Generates a JWT token for a user based on their email and role.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <param name="role">The role of the user.</param>
        /// <returns>A string representing the generated JWT token.</returns>
        string GenerateToken(string email, string role);

        #endregion

        #region Token Parsing

        /// <summary>
        /// Parses a JWT token and extracts the user information.
        /// </summary>
        /// <param name="token">The JWT token to be parsed.</param>
        /// <returns>A <see cref="UserResult"/> containing the extracted user information.</returns>
        UserResult ParseToken(string token);

        #endregion
    }
}
