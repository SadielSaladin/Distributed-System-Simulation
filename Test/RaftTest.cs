﻿

using Distributed_System_Simulation.Services.Interfaces;
using Distributed_System_Simulation.Services;
using Xunit;

namespace Distributed_System_Simulation.Test
{
    public class RaftTest
    {
        private readonly INetworkService _networkService = new NetworkService();
        private readonly NodeServices _node1;
        private readonly NodeServices _node2;
        private readonly NodeServices _node3;

        public RaftTest()
        {
            _node1 = new NodeServices(_networkService);
            _node2 = new NodeServices(_networkService);
            _node3 = new NodeServices(_networkService);

            _networkService.RegisterNode(_node1);
            _networkService.RegisterNode(_node2);
            _networkService.RegisterNode(_node3);

            _node1.AddNeighbor(_node2);
            _node1.AddNeighbor(_node3);
            _node2.AddNeighbor(_node1);
            _node2.AddNeighbor(_node3);
            _node3.AddNeighbor(_node1);
            _node3.AddNeighbor(_node2);
        }

        [Fact]
        public void Test_Consensus_With_Multiple_Proposals()
        {

            _node1.BecomeLeader();


            _node1.ProposeState(1);

            _node2.ProposeState(2);
            _node3.ProposeState(3);


            _node3.SimulatePartition(new List<INodeServices> { _node1 });


            _node3.BecomeLeader();
            _node3.ProposeState(3);


            Assert.Matches($"Node {_node2.NodeId} cannot propose a state as it is not the leader", string.Join(" ", _node2.GetMessages()));
            Assert.Matches($"Node {_node3.NodeId} cannot propose a state as it is not the leader", string.Join(" ", _node3.GetMessages()));

            Assert.Contains("State proposal: 3", _node1.GetMessages());
            Assert.Contains("State proposal: 3", _node2.GetMessages());
            Assert.Contains("State proposal: 3", _node3.GetMessages());


            var LogCount = _node3.GetLogs().Count(log => log.Contains("State proposal: 3"));
            Assert.Equal(2, LogCount);  
        }
    }
}
