namespace Requests.Domain
{
    public interface IRequest
    {
        // Submit()
        void Submit();
        // Confirm
        void Confirm();
        // Cancel
        void Cancel();
        // Approve
        void  Approve();
        // Disapprove
        void Disapprove();
        // Conclude, 
        void Conclude();
        // Remove
        void Remove();
         // Status   
         RequestStatus Status { get; }
    }
}