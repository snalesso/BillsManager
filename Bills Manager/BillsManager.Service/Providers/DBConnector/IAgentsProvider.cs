using BillsManager.Models;
using System.Collections.Generic;

namespace BillsManager.Services.Providers
{
    public interface IAgentsProvider
    {
        string Path { get; }

        string DBName { get; }

        uint GetLastAgentID();

        IEnumerable<Agent> GetAllAgents();
        Agent GetAgentByID(uint agentID);

        bool Add(Agent agent);

        bool Edit(Agent agent);
        bool Edit(IEnumerable<Agent> agents);

        bool Delete(Agent agent);
        bool Delete(IEnumerable<Agent> agents);
    }
}