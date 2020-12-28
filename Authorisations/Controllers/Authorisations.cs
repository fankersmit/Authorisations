using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
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
    [Produces(MediaTypeNames.Application.Json)] //"application/json"
    public class AuthorisationsController : ControllerBase
    {
        private readonly ILogger<AuthorisationsController> _logger;
        private readonly RabbitMqClient _client;
        private readonly QueryBuilder _queryBuilder;

        public AuthorisationsController(ILogger<AuthorisationsController> logger,
            RabbitMqClient rabbitMqClient)
        {
            _logger = logger;
            _client = rabbitMqClient;
            _queryBuilder = new QueryBuilder();
        }

        [HttpPost]
        [Route("request/submit/account")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public object SubmitRequest([FromBody] RequestModel request)
        {
            request.Command = Commands.Submit;
            var content = request.SerializeToJson();
            _client.Post(content);
            _logger.LogInformation("Submitted posted request to RMQ client");
            return Accepted();
        }

        /*
        [Route("request/submit/product")]
        [Route("request/submit/organisation")]
         */
        [HttpGet]
        [Route("requests/under-consideration/count")]
        [Route("requests/under-consideration/{requestType}/count")]
        [ProducesResponseType(typeof(Dictionary<string, int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Dictionary<string, int>), StatusCodes.Status400BadRequest)]
        public object RequestsUnderConsideration(string requestType)
        {
            string underConsideration;
            if (requestType != null)
            {
                if (!RequestTypeChecker.IsKnownRequestType(requestType))
                {
                    return BadRequest();
                }

                underConsideration = requestType;
            }
            else
            {
                // get requests total  
                underConsideration = "All";
            }

            var query = _queryBuilder.BuildQueryFor(Queries.UnderConsideration, underConsideration);
            // put request on queue
            var messageBytes = query.AsUTF8Bytes;
            var result = _client.Call(messageBytes);
            int count = Convert.ToInt32(result);
            var response = new Dictionary<string, int> {{underConsideration, count}};
            return response;
        }

        [HttpGet]
        [Route("ping")]
        [ProducesResponseType(typeof(Dictionary<string, string>), StatusCodes.Status200OK)]
        public object Ping()
        {
            var query = _queryBuilder.BuildQueryFor(Queries.Ping, "");
            query.Add("Webserver", "Up");
            var result = _client.Call(query.AsUTF8Bytes);
            var responseObject = JsonSerializer.Deserialize<Dictionary<string, string>>(result);
            query.Arguments.Concat(responseObject);
            _logger.LogInformation( $"Status pinged at: {DateTime.UtcNow}");
            return query;
        }

        [HttpGet]
        [Route("request/{requestId}/status")]
        [ProducesResponseType(typeof(Dictionary<string, string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Dictionary<string, string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public object GetStatusForRequest(string requestId)
        {
            // check if it is a Guid
            if (!Guid.TryParse(requestId, out var guidRequestID))
            {
                return BadRequest(); // 400
            }

            var query = _queryBuilder.BuildQueryFor(Queries.CurrentStatus, guidRequestID);
            // put request on queue
            var result = _client.Call(query.AsUTF8Bytes);

            var responseObject = JsonSerializer.Deserialize<Dictionary<string, string>>(result);
            if (responseObject.ContainsKey("Failure"))
            {
                return NotFound(responseObject); // 404
            }

            _logger.LogInformation($"Status requested for : {requestId}");
            return Ok(responseObject); // 200
        }
    }
}
