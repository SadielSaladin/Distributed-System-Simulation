using Distributed_System_Simulation.Services.Interfaces;
using System.Xml.Linq;
using static Distributed_System_Simulation.Types.Node;

namespace Distributed_System_Simulation.Services
{
 
    public class NodeServices : INodeServices
    {

        public string NodeId { get; private set; }
        private NodeState _state;
        private int _proposedState;
        private List<string> _messages;
        private List<INodeServices> _neighbors;
        private INetworkService _networkService;

        public NodeServices(INetworkService networkService)
        {
            NodeId = Guid.NewGuid().ToString();
            _state = NodeState.Follower;
            _proposedState = 0;
            _messages = new List<string>();
            _neighbors = new List<INodeServices>();
            _networkService = networkService;
        }

        /// <summary>
        /// Handles the proposal of a new state by a node. If the node is the leader, it checks if the proposed state is higher than the current one and broadcasts the new state proposal to all nodes in the network.
        /// </summary>
        /// <param name="state">the proposed state that the node is suggesting to the system.</param>
        public void ProposeState(int state)
        {
            if (_state == NodeState.Leader)
            {
                if (state > _proposedState)
                {
                    _proposedState = state;
                    _messages.Add($"State proposal: {state}");

                    _networkService.BroadcastLogEntry($"State proposal: {state}");

                    foreach (var neighbor in _neighbors)
                    {
                        neighbor.ReceiveStateProposal(state);
                    }
                }
            }
            else
            {
                _messages.Add($"Node {NodeId} cannot propose a state as it is not the leader.");
            }
        }

        /// <summary>
        /// Receives a message from another node. 
        /// This message could be any kind of communication, such as state proposals or other relevant messages in the system.
        /// </summary>
        /// <param name="message">The message received from another node, which can be logged or processed accordingly.</param>
        public void ReceiveMessage(string message)
        {
            _messages.Add(message);
        }
        /// <summary>
        /// Adds a neighboring node to the current node's list of neighbors, 
        /// allowing them to communicate with each other in the system.
        /// </summary>
        /// <param name="neighbor">the node that will be added as a neighbor. This should implement the INodeServices interface.</param>
        public void AddNeighbor(INodeServices neighbor)
        {
            _neighbors.Add(neighbor);
        }

        /// <summary>
        /// Retrieves all the messages that the current node has received and processed.
        /// This method returns a list of strings that represent the messages logged by the node
        /// </summary>
        /// <returns>A list of all messages received by the node.</returns>
        public List<string> GetMessages()
        {
            return _messages;
        }

        /// <summary>
        /// Retrieves the logs of state transitions or any other relevant actions performed by the node. 
        /// This can include state proposals, state changes, or other significant events.
        /// </summary>
        /// <returns>A list of log entries representing actions or events related to the node's state.</returns>
        public List<string> GetLogs()
        {
            return _messages; // Suponiendo que el log está contenido en los mensajes
        }

        public void BecomeLeader()
        {
            _state = NodeState.Leader;
        }
        /// <summary>
        /// Handles the reception of a state proposal from another node. 
        /// It processes the incoming state and decides if it should update the node's state based on the consensus rules.
        /// </summary>
        /// <param name="state">The state proposed by another node that is received by the current node.</param>
        public void ReceiveStateProposal(int state)
        {
            _messages.Add($"State proposal: {state}");
        }

        public void SimulatePartition(List<INodeServices> partitionedNodes)
        {
            _neighbors.RemoveAll(n => partitionedNodes.Contains(n));
        }
    }
}
