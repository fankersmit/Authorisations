using Requests.Shared.Domain;

namespace Requests.Domain
{
    public interface IRequest
    {
        // Submit()
        bool Submit();
        // Confirm
        bool Confirm();
        // Cancel
        bool Cancel();
        // Approve
        bool  Approve();
        // Disapprove
        bool Disapprove();
        // Conclude, 
        bool Conclude();
        // Remove
        bool Remove();
         // Status   
         RequestStatus Status { get; }
    }
}