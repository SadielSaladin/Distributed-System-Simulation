namespace Distributed_System_Simulation.Models
{
    /// <summary>
    /// Model use to request vote from other nodes
    /// </summary>
    public class RequestVoteModel
    {
        public int CandidateId { get; set; }
        public int Term { get; set; }
    }
}
