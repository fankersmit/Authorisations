using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AuthorisationRequest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces( MediaTypeNames.Application.Json)] //"application/json"
    public class AuthorisationRequestController : ControllerBase
    {
        private readonly ILogger<AuthorisationRequestController> _logger;       
        public AuthorisationRequestController(ILogger<AuthorisationRequestController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("requests/under-consideration")]
        [ProducesResponseType(typeof(Dictionary<string,int>), StatusCodes.Status200OK)]
        public Dictionary<string,int> RequestsUnderConsideration()
        {
            // get from requeastapp behind rabbitmq
            return new Dictionary<string,int> {{"RequestsUnderConsideration", 0 }};
        }
        
        [HttpGet]
        [ProducesResponseType(typeof(Dictionary<string,string>), StatusCodes.Status200OK)]
        public object Get()
        {
            var responseObject = new Dictionary<string,string> {{"Status", "Up"}};
            _logger.LogInformation($"Status pinged: {responseObject.ElementAt(0).Value}");
            return responseObject;
        }
        
    }
}