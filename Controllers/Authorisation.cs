using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AuthorisationRequest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthorisationRequestController : ControllerBase
    {
        private readonly ILogger<AuthorisationRequestController> _logger;       
        public AuthorisationRequestController(ILogger<AuthorisationRequestController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("requests/under-consideration")]
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