using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;

namespace Authorisation.Controllers
{
    public interface IRequest
    {
        // New, POST            Nieuw
        HttpResponseMessage Submit(string postData, HttpRequestMessage request);
        // Confirm, PUT        Bevestigen
        HttpResponseMessage Confirm( HttpRequestMessage request, Guid RequestId);
        // Cancel, PUT         Annuleren
        HttpResponseMessage Cancel( HttpRequestMessage request, Guid RequestId);
        // Approve, PUT        Goedkeuren
        HttpResponseMessage Approve( HttpRequestMessage request, Guid RequestId);
        // Disapprove, PUT     Afkeuren
        HttpResponseMessage Disappvove( HttpRequestMessage request, Guid RequestId);
        // Conclude, PUT       Afhandelen
        HttpResponseMessage Conclude( HttpRequestMessage request, Guid RequestId);
        HttpResponseMessage Remove( HttpRequestMessage request, Guid RequestId);
        // Status, GET         Status   
        HttpResponseMessage Status( HttpRequestMessage request, Guid RequestId);
        
    }
}