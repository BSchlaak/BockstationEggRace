using Bockstation.EggRace.Common.Interfaces;
using Bockstation.EggRace.Data.Common.Models;
using Bockstation.EggRace.UI.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;

namespace Bockstation.EggRace.UI.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultsController : ControllerBase
    {
        #region Private fields
        private readonly IDataRepository<Team, Result> _dataRepository;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ResultsService _resultsService;
        #endregion Private fields

        #region Constructors
        public ResultsController(IDataRepository<Team, Result> dataRepository, IServiceScopeFactory serviceScopeFactory)
        {
            _dataRepository = dataRepository;
            _serviceScopeFactory = serviceScopeFactory;

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedService = scope.ServiceProvider;
                var allHostedServices = scopedService.GetService<IEnumerable<IHostedService>>();
                var resultsServices = allHostedServices.OfType<ResultsService>();
                _resultsService = resultsServices.SingleOrDefault();
            }
        }
        #endregion Constructors

        #region Methods
        #region Public
        [HttpGet]
        public IActionResult Get()
        {
            var results = _resultsService?.GetResults();
            return Ok(results);
        }

        [HttpPost("start")]
        public IActionResult Start(Result result)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                _resultsService?.Start(result);
            }

            return Ok();
        }

        [HttpGet("file")]
        public IActionResult DownloadFile()
        {
            var file = _dataRepository?.DownloadResultsFile();
            return Ok(file);
        }
        #endregion Public
        #endregion Methods
    }
}
