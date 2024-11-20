using CM.ApplicationService.AuthModule.Abstracts;
using CM.ApplicationService.AuthModule.Implements;
using CM.ApplicationService.Movie.Abstracts;
using CM.Dtos.Movie;
using CM.Dtos.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CM_API.Controllers
{
    [Route("api/[controller]")]


    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;
        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddComment([FromForm] AddCommentDto request)
        {
            try
            {
                // Lấy UserId từ JWT token
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new { Message = "User ID not found in token." });
                }
                int userId = int.Parse(userIdClaim);

                // Thêm bình luận vào cơ sở dữ liệu
                await _commentService.AddCommentAsync(request, userId);

                return Ok(new { Message = "Comment added successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}