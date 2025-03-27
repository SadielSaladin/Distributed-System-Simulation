using Distributed_System_Simulation.Services.Interfaces;
using static Distributed_System_Simulation.Types.Node;

namespace Distributed_System_Simulation.Services
{
    /// <summary>
    /// Class that handles network communications between nodes
    /// </summary>
    public class NetworkService : INetworkService
    {

        /// <summary>
        /// nodes in the network
        /// </summary>
        private List<INodeServices> _nodes;
        /// <summary>
        /// votes for the candidates in the network
        /// </summary>
        private Dictionary<string, int> _votes;

        public NetworkService()
        {
            _nodes = new List<INodeServices>();
            _votes = new Dictionary<string, int>();
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

        /// <summary>
        /// Registers a vote for a candidate in the Raft algorithm and determines if they have won the election.
        /// </summary>
        /// <param name="candidateId">The unique identifier of the candidate receiving the vote.</param>
        /// <param name="term">The current election term.</param>
        public void BroadcastVote(string candidateId, int term)
        {
            if (!_votes.ContainsKey(candidateId))
            {
                _votes[candidateId] = 0;
            }
            _votes[candidateId]++;

            int majority = _nodes.Count / 2 + 1;
            if (_votes[candidateId] >= majority)
            {
                var leaderNode = _nodes.FirstOrDefault(n => (n as NodeServices).NodeId == candidateId);
                if (leaderNode != null)
                {
                    (leaderNode as NodeServices).BecomeLeader();
                }
            }
        }

        /// <summary>
        /// Initiates a new leader election in the Raft algorithm.
        /// Selects the first available node to start an election.
        /// </summary>
        public void SelectNewLeader()
        {
            var availableNodes = _nodes.Where(n => !(n as NodeServices)._isFailed).ToList();
            if (availableNodes.Any())
            {
                availableNodes.First().StartElection();
            }
            else
            {
                BroadcastLogEntry("No available nodes to become the leader.");
            }
        }

    }
}
