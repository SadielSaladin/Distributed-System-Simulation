namespace Distributed_System_Simulation.Services.Interfaces
{
    /// <summary>
    /// Interface for node handling services
    /// </summary>
    public interface INodeServices
    {
        /// <summary>
        /// Node identifier
        /// </summary>
        string NodeId { get; }
        /// <summary>
        /// Handles the proposal of a new state by a node. 
        /// If the node is the leader, it checks if the proposed state is higher than the current one and broadcasts the new state proposal to all nodes in the network.
        /// </summary>
        /// <param name="state">the proposed state that the node is suggesting to the system.</param>
        void ProposeState(int state);
        /// <summary>
        /// Handles the reception of a state proposal from another node. 
        /// It processes the incoming state and decides if it should update the node's state based on the consensus rules.
        /// </summary>
        /// <param name="state">The state proposed by another node that is received by the current node.</param>
        void ReceiveStateProposal(int state);
        /// <summary>
        /// Receives a message from another node. 
        /// This message could be any kind of communication, such as state proposals or other relevant messages in the system.
        /// </summary>
        /// <param name="message">The message received from another node, which can be logged or processed accordingly.</param>
        void ReceiveMessage(string message);
        /// <summary>
        /// Adds a neighboring node to the current node's list of neighbors, 
        /// allowing them to communicate with each other in the system.
        /// </summary>
        /// <param name="neighbor">the node that will be added as a neighbor. This should implement the INodeServices interface.</param>
        void AddNeighbor(INodeServices neighbor);
        /// <summary>
        /// Retrieves all the messages that the current node has received and processed.
        /// This method returns a list of strings that represent the messages logged by the node
        /// </summary>
        /// <returns>A list of all messages received by the node.</returns>
        List<string> GetMessages();
        /// <summary>
        /// Retrieves the logs of state transitions or any other relevant actions performed by the node. 
        /// This can include state proposals, state changes, or other significant events.
        /// </summary>
        /// <returns>A list of log entries representing actions or events related to the node's state.</returns>
        List<string> GetLogs();
        /// <summary>
        /// Simulates a network partition by disconnecting the node from a list of other nodes, 
        /// preventing communication between them. This can be used to test how the system behaves when certain nodes are isolated from the network.
        /// </summary>
        /// <param name="partitionedNodes">A list of INodeServices nodes that will be isolated from the current node, simulating a network partition.</param>
        public void SimulatePartition(List<INodeServices> partitionedNodes);
        /// <summary>
        /// Sets the current node as the leader of the system. 
        /// This method updates the node's state to "Leader" and allows it to propose new states and coordinate the system's consensus process.
        /// </summary>
        public void BecomeLeader();
    }
}
