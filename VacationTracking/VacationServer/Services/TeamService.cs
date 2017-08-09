using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Common.Core.Exceptions;
using Microsoft.EntityFrameworkCore;
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

        public async Task<Team> UpdateAsync(Team team)
        {
            if(team == null)
            {
                throw new CustomException(Error.TEAM_IS_NULL, Error.TEAM_IS_NULL_MSG);
            }

            var existingTeam = await this.GetByIdAsync(team.Id);
            if (existingTeam == null)
            {
                throw new CustomException(Error.TEAM_NOT_FOUND, Error.TEAM_NOT_FOUND_MSG);
            }

            existingTeam.Name = team.Name;
            existingTeam.LeaderId = team.LeaderId;

            existingTeam.ModifiedAt = DateTime.Now;

            return existingTeam;
        }

        public async Task DeleteAsync(int id)
        {
            var existingTeam = await this.GetByIdAsync(id);
            if(existingTeam == null)
            {
                throw new CustomException(Error.TEAM_NOT_FOUND, Error.TEAM_NOT_FOUND_MSG);
            }

            _context.Teams.Remove(existingTeam);
            await _context.SaveChangesAsync();
        }

        public async Task<Team> GetByIdAsync(int id)
        {
            return await _context.Teams.Include(t=>t.TeamMembers).FirstOrDefaultAsync(t => t.Id == id);
        }
    }
    
}
