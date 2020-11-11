using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using AuthorisationRequest.Models;

namespace AuthorisationRequest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class Account : ControllerBase, IRequest
    {
        // POST
        [HttpPost]
       public HttpResponseMessage Submit(string postData, HttpRequestMessage request)
        {
            // create request
            //var body = Request.Body.;
            var accountRequest = JsonSerializer.Deserialize<AccountRequest>(postData);
            // put it on queue
            // <TODO>
            // reply success
            return new HttpResponseMessage(HttpStatusCode.Accepted);  
        }

        public HttpResponseMessage Confirm(HttpRequestMessage request, Guid RequestId)
        {
            throw new NotImplementedException();
        }

        // PUT
        public HttpResponseMessage Cancel(HttpRequestMessage request, Guid RequestId)
        {
            throw new NotImplementedException();
        }

        // PUT
        public HttpResponseMessage Approve(HttpRequestMessage request, Guid RequestId)
        {
            throw new NotImplementedException();
        }

        // PUT
        public HttpResponseMessage Disappvove(HttpRequestMessage request, Guid RequestId)
        {
            throw new NotImplementedException();
        }

        // PUT
        public HttpResponseMessage Conclude(HttpRequestMessage request, Guid RequestId)
        {
            throw new NotImplementedException();
        }

        public HttpResponseMessage Remove(HttpRequestMessage request, Guid RequestId)
        {
            throw new NotImplementedException();
        }

        // GET
        public HttpResponseMessage Status(HttpRequestMessage request, Guid RequestId)
        {
            throw new NotImplementedException();
        }
    }
}