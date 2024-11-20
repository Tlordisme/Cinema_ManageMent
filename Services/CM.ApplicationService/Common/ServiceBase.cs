using CM.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.ApplicationService.Common
{
    public abstract class ServiceBase
    {
        protected readonly ILogger _logger;
        protected readonly CMDbContext _dbContext;

        protected ServiceBase(ILogger logger, CMDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
    }
}