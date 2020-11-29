using System;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Authorisations.Infrastructure;
using Authorisations.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Requests.Domain;

namespace Authorisations.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces( MediaTypeNames.Application.Json)] //"application/json"
    public class AuthorisationsController : ControllerBase
    {
        private readonly ILogger<AuthorisationsController> _logger;
        private readonly RabbitMqClient _client; 
        
        public AuthorisationsController(ILogger<AuthorisationsController> logger,
             RabbitMqClient rabbitMqClient)
        {
            _logger = logger;
            _client = rabbitMqClient;
        }

        [HttpPost]
        [Route("request/submit/account")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public object SubmitRequest(string requestType, [FromBody] AccountRequest request)
        {
            var requestBytes = request.ToJson();
            var content =  System.Text.Encoding.UTF8.GetString(requestBytes);
            _client.Post(content);
             return Accepted();
        }
        
        /*
        [Route("request/submit/product")]
        [Route("request/submit/organisation")]
         */

        [HttpGet]
        [Route("requests/under-consideration")]
        [Route("requests/under-consideration/{requestType}")]
        [ProducesResponseType(typeof(Dictionary<string,string>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Dictionary<string,string>),StatusCodes.Status404NotFound)]
        public object RequestsUnderConsideration(string requestType)
        {
            string underConsideration;
            if (requestType != null)
            {
                if (!RequestChecker.IsKnownRequestType(requestType))
                {
                    return NotFound();
                }
                underConsideration  = $"{requestType}-UnderConsideration";
            }
            else
            {
                underConsideration = "UnderConsideration";    
            }
            // put request on queue
            var result = _client.Call(underConsideration);
            var split  = result.Split(':');
            var response  = new Dictionary<string,string> {{ split[0], split[1]}};
            return response;
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