using CM.ApplicantService.Auth.Permission.Abstracts;
using CM.ApplicationService.AuthModule.Abstracts;
using CM.ApplicationService.Movie.Abstracts;
using CM.ApplicationService.Theater.Abstracts;
using CM.ApplicationService.UserModule.Abstracts;
using CM.Dtos.Movie;
using CM.Dtos.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Share.Constant.Permission;

namespace CM_API.Controllers
{
    [Route("api/[controller]")]
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly IPermissionService _permissionService;
        private readonly ILogger<CommentController> _logger;

        public CommentController(
            ICommentService commentService,
            IPermissionService permissionService,
            ILogger<CommentController> logger)
        {
            _commentService = commentService;
            _permissionService = permissionService;
            _logger = logger;
        }

        [HttpPost("AddComment")]
        [Authorize]
        public async Task<IActionResult> AddComment([FromForm] AddCommentDto request)
        {
            _logger.LogInformation("Start: Adding a new comment.");

            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    _logger.LogWarning("User ID not found in token.");
                    return Unauthorized(new { Message = "User ID not found in token." });
                }

                int userId = int.Parse(userIdClaim);

                // Kiểm tra quyền
                if (!_permissionService.CheckPermission(userId, PermissionKey.AddComment))
                {
                    _logger.LogWarning("User {UserId} does not have permission to add comments.", userId);
                    return Unauthorized("You do not have permission to add comments.");
                }

                await _commentService.AddCommentAsync(request, userId);

                _logger.LogInformation("Comment added successfully for user {UserId}.", userId);
                return Ok(new { Message = "Comment added successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding comment.");
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("GetCommentsMovie/{movieId}")]
        public async Task<IActionResult> GetCommentsByMovieId(int movieId)
        {
            _logger.LogInformation("Start: Retrieving comments for movie ID {MovieId}.", movieId);

            try
            {
                var comments = await _commentService.GetCommentsByMovieId(movieId);
                if (!comments.Any())
                {
                    _logger.LogWarning("No comments found for movie ID {MovieId}.", movieId);
                    return NotFound(new { Message = $"No comments found for movie ID {movieId}." });
                }

                _logger.LogInformation("Successfully retrieved {Count} comments for movie ID {MovieId}.", comments.Count(), movieId);
                return Ok(comments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving comments for movie ID {MovieId}.", movieId);
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpDelete("DeleteComment/{commentId}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            _logger.LogInformation("Start: Deleting comment ID {CommentId}.", commentId);

            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    _logger.LogWarning("User ID not found in token.");
                    return Unauthorized(new { Message = "User ID not found in token." });
                }

                int userId = int.Parse(userIdClaim);

                // Kiểm tra quyền
                if (!_permissionService.CheckPermission(userId, PermissionKey.DeleteComment))
                {
                    _logger.LogWarning("User {UserId} does not have permission to delete comments.", userId);
                    return Unauthorized("You do not have permission to delete comments.");
                }

                await _commentService.DeleteCommentAsync(commentId);

                _logger.LogInformation("Comment ID {CommentId} deleted successfully by user {UserId}.", commentId, userId);
                return Ok(new { Message = "Comment deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting comment ID {CommentId}.", commentId);
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}