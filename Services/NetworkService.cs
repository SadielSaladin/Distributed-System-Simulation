using Distributed_System_Simulation.Services.Interfaces;
using static Distributed_System_Simulation.Types.Node;

namespace Distributed_System_Simulation.Services
{
    /// <summary>
    /// Class that handles network communications between nodes
    /// </summary>
    public class NetworkService : INetworkService
    {
       

        private List<INodeServices> _nodes;

        public NetworkService()
        {
            _nodes = new List<INodeServices>();
        }

        /// <summary>
        /// Add a new node to the network
        /// </summary>
        /// <param name="node">Node to be register into the network</param>
        public void RegisterNode(INodeServices node)
        {
            _nodes.Add(node);
        }

        /// <summary>
        /// Handles the messages to all registered nodes in the system
        /// </summary>
        /// <param name="logEntry">Message to be broadcasted to all nodes</param>
        public void BroadcastLogEntry(string logEntry)
        {
            foreach (var node in _nodes)
            {
                node.ReceiveMessage(logEntry);
            }
        }
        public void BroadcastVote(string candidateId, int term)
        {
            foreach (var node in _nodes)
            {
                node.ReceiveVoteRequest(term, candidateId);
            }
        }

        public void SelectNewLeader()
        {
            // Solo se selecciona un líder si no hay uno actual
            var availableNodes = _nodes.Where(n => !(n as NodeServices)._isFailed).ToList();
            if (availableNodes.Count > 0)
            {
                var leaderCandidate = availableNodes.First();
                (leaderCandidate as NodeServices).BecomeLeader();
            }
            else
            {
                BroadcastLogEntry("No available nodes to become the leader.");
            }
        }

        public void HandleElectionTimeout()
        {
            // Si el líder no está disponible, se inicia una nueva elección
            var candidate = _nodes.FirstOrDefault(n => (n as NodeServices)._state == NodeState.Follower);
            if (candidate != null)
            {
                (candidate as NodeServices).StartElection();
            }
        }
    }
}
