using CM.ApplicationService.Common;
using CM.ApplicationService.Theater.Abstracts;
using CM.Domain.Theater;
using CM.Dtos.Theater;
using CM.Infrastructure;
using Microsoft.Extensions.Logging;

public class TheaterChainService : ServiceBase, ITheaterChainService
{
    private readonly ILogger<TheaterChainService> _logger;

    public TheaterChainService(CMDbContext dbContext, ILogger<TheaterChainService> logger)
        : base(logger, dbContext)
    {
        _logger = logger;
    }

    public string CreateTheaterChain(TheaterChainDto dto)
    {
        _logger.LogInformation("Creating a new theater chain with ID: {TheaterChainId}, Name: {Name}.", dto.Id, dto.Name);

        var theaterChain = new CMTheaterChain
        {
            Id = dto.Id,
            Name = dto.Name,
        };

        _dbContext.TheaterChains.Add(theaterChain);
        _dbContext.SaveChanges();

        _logger.LogInformation("Theater chain created successfully with ID {TheaterChainId}.", theaterChain.Id);
        return theaterChain.Id;
    }

    public List<CMTheaterChain> GetAllTheaterChains()
    {
        _logger.LogInformation("Retrieving all theater chains.");

        var theaterChains = _dbContext.TheaterChains.ToList();

        _logger.LogInformation("Retrieved {TheaterChainCount} theater chains.", theaterChains.Count);
        return theaterChains;
    }

    public void DeleteTheaterChain(string theaterChainId)
    {
        _logger.LogInformation("Attempting to delete theater chain with ID: {TheaterChainId}.", theaterChainId);

        var theaterChain = _dbContext.TheaterChains.Find(theaterChainId);

        if (theaterChain == null)
        {
            _logger.LogWarning("Theater chain with ID {TheaterChainId} does not exist.", theaterChainId);
            throw new Exception("Theater chain không tồn tại.");
        }

        _dbContext.TheaterChains.Remove(theaterChain);
        _dbContext.SaveChanges();

        _logger.LogInformation("Theater chain with ID {TheaterChainId} deleted successfully.", theaterChainId);
    }

    public string UpdateTheaterChain(TheaterChainDto dto)
    {
        _logger.LogInformation("Attempting to update theater chain with ID: {TheaterChainId}.", dto.Id);

        var theaterChain = _dbContext.TheaterChains.Find(dto.Id);

        if (theaterChain == null)
        {
            _logger.LogWarning("Theater chain with ID {TheaterChainId} does not exist.", dto.Id);
            throw new Exception("Theater chain không tồn tại.");
        }

        theaterChain.Name = dto.Name;
        _dbContext.TheaterChains.Update(theaterChain);
        _dbContext.SaveChanges();

        _logger.LogInformation("Theater chain with ID {TheaterChainId} updated successfully.", dto.Id);
        return theaterChain.Id;
    }
}