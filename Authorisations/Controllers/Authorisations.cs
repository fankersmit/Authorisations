using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Mime;
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
        private readonly RabbitMqDefaultClient _defaultClient;
        private readonly RabbitMqRpcClient _rpcClient;
        private readonly QueryBuilder _queryBuilder;

        public AuthorisationsController(ILogger<AuthorisationsController> logger,
            RabbitMqDefaultClient rabbitMqDefaultClient, RabbitMqRpcClient rabbitMqRpcClient)
        {
            _logger = logger;
            _defaultClient = rabbitMqDefaultClient;
            _rpcClient = rabbitMqRpcClient;
            _queryBuilder = new QueryBuilder();
        }

        [HttpPost]
        [Route("request/submit/account")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public object SubmitRequest([FromBody] RequestModel request)
        {
            request.Command = Commands.Submit;
            var content = request.SerializeToJson();
            _defaultClient.Post(content);
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
                    // TODO: add failure message
                    return BadRequest();
                }
                underConsideration = ToTitleCase(requestType);
            }
            else
            {
                // get requests total  
                underConsideration = "All";
            }

            var query = _queryBuilder.BuildQueryFor(Queries.UnderConsideration, underConsideration);
            var result = _rpcClient.Call(query.AsUTF8Bytes);
            var responseObject = JsonSerializer.Deserialize<Dictionary<string, string>>(result);
            _logger.LogInformation( $"Query {underConsideration} executed at: {DateTime.UtcNow}");
            return Ok(responseObject);
        }

        [HttpGet]
        [Route("ping")]
        [ProducesResponseType(typeof(Dictionary<string, string>), StatusCodes.Status200OK)]
        public object Ping()
        {
            var query = _queryBuilder.BuildQueryFor(Queries.Ping, "");
            var result = _rpcClient.Call(query.AsUTF8Bytes);
            var responseObject = JsonSerializer.Deserialize<Dictionary<string, string>>(result);
            responseObject.Add("Webserver", "Up");
            _logger.LogInformation( $"Status pinged at: {DateTime.UtcNow}");
            return Ok(responseObject); //200
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
                var response = new Dictionary<string, string>() {
                    { "Query", "CurrentStatus"},
                    { "ID", requestId  },
                    { "Failure", $"the provided request Id: {requestId} is not a valid Guid."}
                };
                return BadRequest(response); // 400
            }

            var query = _queryBuilder.BuildQueryFor(Queries.CurrentStatus, guidRequestID);
            // put request on queue
            var result = _rpcClient.Call(query.AsUTF8Bytes);
            var responseObject = JsonSerializer.Deserialize<Dictionary<string, string>>(result);
            if (responseObject.ContainsKey("Failure"))
            {
                return NotFound(responseObject); // 404
            }

            _logger.LogInformation($"Status requested for : {requestId}");
            return Ok(responseObject); // 200
        }

        [HttpGet]
        [Route("request/{requestId}")]
        [ProducesResponseType(typeof(Dictionary<string, string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Dictionary<string, string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public object GetLatestRequestInfo(string requestId)
        {
            // check if it is a Guid
            if (!Guid.TryParse(requestId, out var guidRequestID))
            {
                var response = new Dictionary<string, string>() {
                    { "Query", "CurrentStatus"},
                    { "ID", requestId  },
                    { "Failure", $"the provided request Id: {requestId} is not a valid Guid."}
                };
                return BadRequest(response); // 400
            }

            var query = _queryBuilder.BuildQueryFor(Queries.Request, guidRequestID);
            // put request on queue
            byte[] result = _rpcClient.Call(query.AsUTF8Bytes);
            //var responseObject = JsonSerializer.Deserialize<JsonDocument>(result);
            var responseObject = JsonDocument.Parse(result);

            if (responseObject.RootElement.TryGetProperty("Failure", out  var failure))
            {
                return NotFound(responseObject); // 404
            }

            _logger.LogInformation($"Status requested for : {requestId}");
            return Ok(responseObject); // 200
        }

        // private helper mthods
        string ToTitleCase(string toConvert)
        {
            var upr = toConvert.ToUpper()[0];
            return  $"{upr}{toConvert.Substring(1)}";
        }
    }
}
