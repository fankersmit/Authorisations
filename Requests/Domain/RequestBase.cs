using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Requests.Shared.Domain;

namespace Requests.Domain
{
    public abstract class RequestBase : IRequest
    {
        // properties
        public Guid ID { get; protected set; }
        public DateTime DateCreated { get; protected set; }
        public DateTime DateLastUpdated { get; protected set; }
        public RequestStatus Status { get; protected set; }
        public string Remarks { get; protected set; }

        // requester properties
        public Person Applicant { get; protected set; }
        
        // contract
        public Contract Contract { get; protected set; }
        
        // constructors
        protected RequestBase()
        {
            ID = Guid.NewGuid();
            DateCreated = DateLastUpdated = DateTime.UtcNow;
            Status = RequestStatus.New;
            Remarks = string.Empty;
        }

        // methods
        public virtual byte[] ToJson()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            return JsonSerializer.SerializeToUtf8Bytes(this, options);   
        }

        public virtual bool Submit()
        {
            if( !IsInValidState(ValidTransitions.Submit)) return false; 
            UpdateStatus(RequestStatus.Submitted);
            return true;
        }
        
        public virtual bool Confirm()
        {
            if( !IsInValidState(ValidTransitions.Confirm)) return false; 
            UpdateStatus(RequestStatus.Confirmed);
            return true;
        }

        public virtual bool Cancel()
        {
            if( !IsInValidState(ValidTransitions.Cancel)) return false;             
            UpdateStatus(RequestStatus.Cancelled);
            return true;
        }

        public virtual bool Approve()
        {
            if( !IsInValidState(ValidTransitions.Approve)) return false; 
            UpdateStatus(RequestStatus.Approved);
            return true;
        }

        public virtual bool Disapprove()
        {
            if( !IsInValidState(ValidTransitions.Disapprove)) return false; 
            UpdateStatus(RequestStatus.Disapproved);
            return true;
        }

        public virtual bool Conclude()
        {
            if( !IsInValidState(ValidTransitions.Conclude)) return false; 
            UpdateStatus(RequestStatus.Concluded);
            return true;
        }

        public virtual bool Remove()
        {
            if( !IsInValidState(ValidTransitions.Remove)) return false; 
            UpdateStatus(RequestStatus.Removed);
            return true;
        }
        
        // private Helper methods
        void UpdateStatus(RequestStatus newStatus)
        {
            if (newStatus == Status) return;
            Status = newStatus;
            DateLastUpdated = DateTime.UtcNow;
        }

        private bool IsInValidState( ValidTransitions transition )
        {
            var currentStatus = (uint)Status;
            var desired = (uint) transition;
            return (currentStatus & desired) != 0; 
        }
    }
}