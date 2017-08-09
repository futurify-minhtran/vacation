using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Common.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using VacationServer.Adapters;
using VacationServer.Models;
using VacationServer.Models.BindingModels;
using VacationServer.Resources;
using VacationServer.ServiceInterfaces;

namespace VacationServer.Controllers
{
    [Route("api/team")]
    public class TeamController : Controller
    {
        ITeamService _teamService;

        public TeamController(ITeamService teamService) {
            _teamService = teamService;
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody]TeamBindingModel teamBindingModel)
        {
            try
            {
                var teamModel = teamBindingModel.ToModel();

                if (teamModel == null || !ModelState.IsValid)
                {
                    throw new CustomException(Error.INVALID_TEAM, Error.INVALID_TEAM_MSG);
                }

                teamModel = await _teamService.CreateAsync(teamModel);

                var teamViewModel = teamModel.ToViewModel();

                return Json(new { Team = teamViewModel });
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message });
            }
        }

        [HttpGet, Route("{id:int}")]
        public async Task<ActionResult> GetById(int id)
        {
            var team = await _teamService.GetByIdAsync(id);
            return Json(new { Team = team });
        }
    }
}
