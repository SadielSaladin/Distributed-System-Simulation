namespace Distributed_System_Simulation.Services.Interfaces
{
    /// <summary>
    /// Interface for network comunications services
    /// </summary>
    public interface INetworkService
    {
        /// <summary>
        /// Add a new node to the network
        /// </summary>
        /// <param name="node">Node to be register into the network</param>
        void RegisterNode(INodeServices node);

        /// <summary>
        /// Handles the messages to all registered nodes in the system
        /// </summary>
        /// <param name="logEntry">Message to be broadcasted to all nodes</param>
        void BroadcastLogEntry(string logEntry);

        public void BroadcastVote(string candidateId, int term);

        public void SelectNewLeader();
    }
}
