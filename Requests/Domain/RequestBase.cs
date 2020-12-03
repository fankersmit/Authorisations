using System;
using System.Text.Json;

namespace Requests.Domain
{
    public abstract class RequestBase : IRequest 
    {
        // properties
        public Guid Id { get; protected set; }
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
            Id = Guid.NewGuid();
            DateCreated = DateLastUpdated = DateTime.UtcNow;
            Status = RequestStatus.New;
            Remarks = string.Empty;
        }

        // methods
        public virtual byte[] ToJson()
        {
            var options = new JsonSerializerOptions {WriteIndented = true};
            return JsonSerializer.SerializeToUtf8Bytes(this, options);   
        }

        public virtual void Submit()
        {
            if( !IsInValidState(ValidTransitions.Submit)) return; 
            UpdateStatus(RequestStatus.Submitted);
        }
        
        public virtual void Confirm()
        {
            if( !IsInValidState(ValidTransitions.Confirm)) return; 
            UpdateStatus(RequestStatus.Confirmed);             
        }

        public virtual void Cancel()
        {
            if( !IsInValidState(ValidTransitions.Cancel)) return;             
            UpdateStatus(RequestStatus.Cancelled);        
        }

        public virtual void Approve()
        {
            if( !IsInValidState(ValidTransitions.Approve)) return; 
            UpdateStatus(RequestStatus.Approved);
        }

        public virtual void Disapprove()
        {
            if( !IsInValidState(ValidTransitions.Disapprove)) return; 
            UpdateStatus(RequestStatus.Disapproved);
        }

        public virtual void Conclude()
        {
            if( !IsInValidState(ValidTransitions.Conclude)) return; 
            UpdateStatus(RequestStatus.Concluded);
        }

        public virtual void Remove()
        {
            if( !IsInValidState(ValidTransitions.Remove)) return; 
            UpdateStatus(RequestStatus.Removed);
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