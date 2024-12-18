using CM.ApplicationService.Cloudinary.Abstracts;
using CM.ApplicationService.Common;
using CM.ApplicationService.Movie.Abstracts;
using CM.Domain.Movie;
using CM.Dtos.Movie;
using CM.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CM.ApplicationService.Movie.Implements
{
    public class CommentService : ServiceBase, ICommentService
    {
        private readonly ICloudService _cloudService;

        public CommentService(
            ICloudService cloudService,
            CMDbContext dbContext,
            ILogger<CommentService> logger
        )
            : base(logger, dbContext)
        {
            _cloudService = cloudService;
        }

        public async Task AddCommentAsync(AddCommentDto dto, int userId)
        {
            var movie = await _dbContext.Movies.FirstOrDefaultAsync(m => m.Id == dto.MovieId);
            if (movie == null)
            {
                _logger.LogWarning($"Movie with ID {dto.MovieId} not found.");
                throw new Exception("Movie not found.");
            }

            string? imgUrl = null;
            if (dto.Image != null && dto.Image.Length > 0)
            {
                imgUrl = await _cloudService.UploadImageAsync(dto.Image, "Comment Images");
            }
            else
            {
                imgUrl = "";
            }

            var newComment = new MoComment
            {
                MovieId = dto.MovieId,
                Content = dto.Content,
                Rating = dto.Rating,
                ImageUrl = imgUrl,
                CreateAt = DateTime.Now,
                UserId = userId,
            };

            await _dbContext.Comments.AddAsync(newComment);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation($"Comment added for movie ID {dto.MovieId} by user {userId}");
        }

        public async Task DeleteCommentAsync(int commentId)
        {
            var comment = await _dbContext.Set<CommentDto>()
                                         .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null)
            {
                throw new Exception("Comment not found.");
            }

            _dbContext.Set<CommentDto>().Remove(comment);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation($"Comment ID {commentId} deleted.");
        }

        public async Task<IEnumerable<CommentDto>> GetCommentsByMovieId(int movieId)
        {
            try
            {
                var comments = await _dbContext
                    .Comments.Where(c => c.MovieId == movieId)
                    .OrderByDescending(c => c.CreateAt)
                    .Select(c => new CommentDto
                    {
                        Id = c.Id,
                        MovieId = c.MovieId,
                        UserId = c.UserId,
                        Content = c.Content,
                        Rating = c.Rating,
                        ImageUrl = c.ImageUrl,
                        CreateAt = c.CreateAt,
                    })
                    .ToListAsync();

                _logger.LogInformation(
                    $"Retrieved {comments.Count} comments for movie ID {movieId}"
                );
                return comments;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving comments for movie ID {movieId}", ex);
                throw;
            }
        }
    }
}