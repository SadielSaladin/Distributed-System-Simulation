using Distributed_System_Simulation.Services.Interfaces;

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
    }
}
