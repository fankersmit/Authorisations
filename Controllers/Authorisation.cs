using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Authorisation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthorisationController : ControllerBase
    {
        private readonly ILogger<AuthorisationController> _logger;       
        
        public AuthorisationController(ILogger<AuthorisationController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("under-consideration")]
        public Dictionary<string,int> RequestsUnderConsideration()
        {
            return new Dictionary<string,int> {{"RequestsUnderConsideration", 0 }};
        }
        
        [HttpGet]
        public object Get()
        {
            var responseObject = new
            {
                Status = "Up"
            };
            _logger.LogInformation($"Status pinged: {responseObject.Status}");
            return responseObject;
        }
        
    }
}