using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationServer.Models;

namespace VacationServer.ServiceInterfaces
{
    public interface ITeamService
    {
        Task<Team> CreateAsync(Team team);
    }
}
