using Bockstation.EggRace.Common.Interfaces;
using Bockstation.EggRace.Data.Common.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

namespace Bockstation.EggRace.UI.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        #region Private fields
        private readonly IDataRepository<Team, Result> _dataRepository;
        #endregion Private fields

        #region Constructors
        public TeamsController(IDataRepository<Team, Result> dataRepository)
        {
            _dataRepository = dataRepository;
        }
        #endregion Constructors

        #region Methods
        #region Public
        [HttpGet]
        public IActionResult Get()
        {
            var result = _dataRepository.GetTeams();
            return Ok(result);
        }

        [HttpPost("file")]
        public IActionResult UploadFile()
        {
            try
            {
                var file = Request.Form.Files[0];
                Console.WriteLine(file);

                using (var stream = new MemoryStream())
                {
                    file.CopyTo(stream);
                    _dataRepository.UploadTeamsFile(stream);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }

            return Ok();
        }
        #endregion Public
        #endregion Methods
    }
}
