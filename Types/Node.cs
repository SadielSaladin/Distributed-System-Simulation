namespace Distributed_System_Simulation.Types
{
    /// <summary>
    /// Class with the different static data that a node can use.
    /// </summary>
    public class Node
    {
        /// <summary>
        /// enum with the different state that a node can have 
        /// </summary>
        public enum NodeState { Follower, Candidate, Leader }
    }
}
