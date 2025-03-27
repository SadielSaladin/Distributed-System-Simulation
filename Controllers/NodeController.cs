using Distributed_System_Simulation.Models;
using Distributed_System_Simulation.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Distributed_System_Simulation.Controllers
{
    [Route("api/nodes")]
    [ApiController]
    public class NodeController : ControllerBase
    {
        private readonly INodeServices _nodeServices;

        public NodeController(INodeServices nodeServices)
        {
            _nodeServices = nodeServices;
        }
    }
}
