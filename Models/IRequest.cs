namespace AuthorisationRequest.Models
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
         // Status   
         RequestStatus Status { get; }
    }
}