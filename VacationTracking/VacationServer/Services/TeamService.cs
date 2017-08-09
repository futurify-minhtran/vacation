using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Common.Core.Exceptions;
using VacationServer.Models;
using VacationServer.Resources;
using VacationServer.ServiceInterfaces;

namespace VacationServer.Services
{
    public class TeamService : ITeamService
    {
        private VacationDbContext _context;
        
        public TeamService(VacationDbContext context)
        {
            _context = context;
        }

        public async Task<Team> CreateAsync(Team team)
        {
            if(team == null)
            {
                throw new CustomException(Error.TEAM_IS_NULL, Error.TEAM_IS_NULL_MSG);
            }

            var now = DateTime.Now;
            team.CreatedAt = now;
            team.ModifiedAt = now;

            _context.Teams.Add(team);
            await _context.SaveChangesAsync();

            return team;
        }
    }
}
