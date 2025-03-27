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

        /// <summary>
        /// Registers a vote for a candidate in the Raft algorithm and determines if they have won the election.
        /// </summary>
        /// <param name="candidateId">The unique identifier of the candidate receiving the vote.</param>
        /// <param name="term">The current election term.</param>
        public void BroadcastVote(string candidateId, int term);

        /// <summary>
        /// Initiates a new leader election in the Raft algorithm.
        /// Selects the first available node to start an election.
        /// </summary>
        public void SelectNewLeader();
    }
}
