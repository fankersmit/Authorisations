using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Authorisation.Models;

namespace Authorisation.Controllers
{
    public class Account : Controller, IRequest
    {
        // POST
        public HttpResponseMessage New(HttpRequestMessage request)
        {
            // create request
            
            // put it on queue
            // reply success
            throw new NotImplementedException();
        }

        // PUT
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