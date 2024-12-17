using CM.ApplicantService.Auth.Permission.Abstracts;
using CM.ApplicationService.Movie.Abstracts;
using CM.Domain.Auth;
using CM.Dtos.Movie;
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

        public CommentController(
            ICommentService commentService,
            IPermissionService permissionService)
        {
            _commentService = commentService;
            _permissionService = permissionService;
        }

        [HttpPost("AddComment")]
        [Authorize]
        public async Task<IActionResult> AddComment([FromForm] AddCommentDto request)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new { Message = "User ID not found in token." });
                }

                int userId = int.Parse(userIdClaim);

                // Kiểm tra quyền
                if (!_permissionService.CheckPermission(userId, PermissionKey.AddComment))
                {
                    return Unauthorized("You do not have permission to add comments.");
                }

                await _commentService.AddCommentAsync(request, userId);
                return Ok(new { Message = "Comment added successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("GetCommentsMovie/{movieId}")]
        public async Task<IActionResult> GetCommentsByMovieId(int movieId)
        {
            try
            {
                var comments = await _commentService.GetCommentsByMovieId(movieId);
                if (!comments.Any())
                {
                    return NotFound(new { Message = $"No comments found for movie ID {movieId}." });
                }

                return Ok(comments);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpDelete("DeleteComment/{commentId}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new { Message = "User ID not found in token." });
                }

                int userId = int.Parse(userIdClaim);

                // Kiểm tra quyền
                if (!_permissionService.CheckPermission(userId, PermissionKey.DeleteComment))
                {
                    return Unauthorized("You do not have permission to delete comments.");
                }

                await _commentService.DeleteCommentAsync(commentId);
                return Ok(new { Message = "Comment deleted successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
