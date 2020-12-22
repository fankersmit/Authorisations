using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Authorisations.Infrastructure;
using Authorisations.Models;
using Requests.Shared.Domain;

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
        public object SubmitRequest( [FromBody] RequestModel request )
        {
            request.Command = Commands.Submit;
            var content = request.SerializeToJson();
            _client.Post( content);
            _logger.LogInformation( "Submitted posted request to RMQ client");
            return Accepted();
        }
        
        /*
        [Route("request/submit/product")]
        [Route("request/submit/organisation")]
         */
#nullable enable 
        [HttpGet]
        [Route("requests/under-consideration")]
        [Route("requests/under-consideration/{requestType}")]
        [ProducesResponseType(typeof(Dictionary<string,int>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Dictionary<string,int>),StatusCodes.Status400BadRequest)]
        public object RequestsUnderConsideration(string? requestType)
        {
            string underConsideration;
            if (requestType != null)
            {
                if (!RequestTypeChecker.IsKnownRequestType(requestType))
                {
                    return BadRequest();
                }
                underConsideration  = $"UnderConsideration-{requestType}";
            }
            else
            {
                // get requests total  
                underConsideration = "UnderConsideration-all";    
            }
            // put request on queue
            var messageBytes = Encoding.UTF8.GetBytes(underConsideration);
            var result = _client.Call(messageBytes);
            int count = Convert.ToInt32(result);
            var response  = new Dictionary<string,int> {{ underConsideration, count}};
            return response;
        }
 #nullable disable    
        
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