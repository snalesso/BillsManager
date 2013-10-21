using BillsManager.Model;
using System.Collections.Generic;

namespace BillsManager.Service.Providers
{
    public interface IAgentsProvider
    {
        uint GetLastID();

        IEnumerable<Agent> GetAll();
        Agent GetByID(uint agentID);

        bool Add(Agent agent);

        bool Edit(Agent agent);
        bool Edit(IEnumerable<Agent> agents);

        bool Delete(Agent agent);
        bool Delete(IEnumerable<Agent> agents);
    }
}