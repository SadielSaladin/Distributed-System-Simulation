using Distributed_System_Simulation.Services.Interfaces;
using System.Xml.Linq;
using static Distributed_System_Simulation.Types.Node;

namespace Distributed_System_Simulation.Services
{
 
    public class NodeServices : INodeServices
    {

        public string NodeId { get; private set; }
        public bool _isFailed { get; private set; }

        public NodeState _state;
        private int _proposedState;
        private int _currentTerm;
        private int _votesReceived;
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
            _isFailed = false;
            _votesReceived = 0;
            _proposedState = 0;
        }

        /// <summary>
        /// Handles the proposal of a new state by a node. If the node is the leader, it checks if the proposed state is higher than the current one and broadcasts the new state proposal to all nodes in the network.
        /// </summary>
        /// <param name="state">the proposed state that the node is suggesting to the system.</param>
        public void ProposeState(int state)
        {
            if (_isFailed)
            {
                _messages.Add($"Node {NodeId} cannot propose a state because it has failed.");
                return;
            }

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
            if(!_isFailed) _messages.Add(message);
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

        /// <summary>
        /// Sets the current node as the leader of the system. 
        /// This method updates the node's state to "Leader" and allows it to propose new states and coordinate the system's consensus process.
        /// </summary>
        public void BecomeLeader()
        {
            if (_isFailed)
            {
                _messages.Add($"Node {NodeId} cannot become leader because it has failed.");
                return;
            }

            _state = NodeState.Leader;
            _votesReceived = 0;  // Reset vote count on becoming leader
            _currentTerm++;
            _messages.Add($"Node {NodeId} has become the leader.");
            _networkService.BroadcastLogEntry($"Node {NodeId} has become the leader.");
            _messages.Add($"Node {NodeId} has become the leader.");
            _networkService.BroadcastLogEntry($"Node {NodeId} has become the leader for term {_currentTerm}.");
        }

        /// <summary>
        /// Handles the reception of a state proposal from another node. 
        /// It processes the incoming state and decides if it should update the node's state based on the consensus rules.
        /// </summary>
        /// <param name="state">The state proposed by another node that is received by the current node.</param>
        public void ReceiveStateProposal(int state)
        {
            if (_isFailed)
            {
                _messages.Add($"Node {NodeId} cannot receive state proposals because it has failed.");
                return;
            }

            _messages.Add($"State proposal: {state}");
        }

        /// <summary>
        /// Start the election process to select a new leader.
        /// </summary>
        public void StartElection()
        {
            if(_isFailed) return;
            _state = NodeState.Candidate;
            _currentTerm++;
            _votesReceived = 1; // Vote for itself
            _messages.Add($"Node {NodeId} is starting an election for term {_currentTerm}.");

            // Request votes from all neighbors
            foreach (var neighbor in _neighbors)
            {
                neighbor.ReceiveVoteRequest(_currentTerm, NodeId);
            }
        }

        /// <summary>
        /// Handles a vote request from a candidate in the Raft algorithm.
        /// </summary>
        /// <param name="term">The term of the candidate requesting the vote.</param>
        /// <param name="candidateId">The unique identifier of the candidate requesting the vote.</param>
        public void ReceiveVoteRequest(int term, string candidateId)
        {
            if(_isFailed) return;

            if (term > _currentTerm)
            {
                _currentTerm = term;
                _state = NodeState.Follower; // Convert to follower if the candidate's term is greater
                _votesReceived = 0;
                _messages.Add($"Node {NodeId} is now a follower (term {_currentTerm}) due to vote request from {candidateId}.");
                // Vote for the candidate
                SendVote(candidateId);
            }
            else if (term == _currentTerm && _state == NodeState.Follower)
            {
                SendVote(candidateId);
            }
        }

        /// <summary>
        /// Sends a vote response to a candidate in the Raft algorithm.
        /// </summary>
        /// <param name="candidateId">he unique identifier of the candidate requesting the vote.</param>
        public void SendVote(string candidateId)
        {
            _messages.Add($"Node {NodeId} has voted for {candidateId} in term {_currentTerm}.");
            _networkService.BroadcastVote(candidateId, _currentTerm);
        }

        /// <summary>
        /// Simulates a network partition by disconnecting the node from a list of other nodes, 
        /// preventing communication between them. This can be used to test how the system behaves when certain nodes are isolated from the network.
        /// </summary>
        /// <param name="partitionedNodes">A list of INodeServices nodes that will be isolated from the current node, simulating a network partition.</param>
        public void SimulatePartition(List<INodeServices> partitionedNodes)
        {
            _neighbors.RemoveAll(n => partitionedNodes.Contains(n));
        }

        /// <summary>
        /// Simulates the failiure of the current node.
        /// </summary>
        public void SimulateFail()
        {
            _isFailed = true;
            _messages.Add($"Node {NodeId} has failed.");
            _state = NodeState.Follower;
        }
    }
}
