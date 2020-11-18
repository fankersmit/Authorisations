using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Authorisations.Infrastructure;
using Authorisations.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Authorisations.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces( MediaTypeNames.Application.Json)] //"application/json"
    public class AuthorisationsController : ControllerBase
    {
        private readonly ILogger<AuthorisationsController> _logger;
        private readonly RabbitMqRpcClient _rpcClient; 
        
        public AuthorisationsController(ILogger<AuthorisationsController> logger,
             RabbitMqRpcClient rabbitMqRpcClient)
        {
            _logger = logger;
            _rpcClient = rabbitMqRpcClient;
        }

        [HttpGet]
        [Route("requests/under-consideration")]
        [Route("requests/{requestType}/under-consideration")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult RequestsUnderConsideration(string requestType)
        {
            // abort if request is fawlty
            if (!RequestChecker.IsKnownRequestType(requestType))
            {
                return NotFound();
            }

            // create message call
            var sep = requestType == null ? string.Empty : "-";  
            var underConsideration = $"{requestType}{sep}UnderConsideration";
            // put request on queue
            var response = _rpcClient.Call(underConsideration);
            return Ok( new Dictionary<string,string> {{ $"{underConsideration}", response }});
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