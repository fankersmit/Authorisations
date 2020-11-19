using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Requests.Domain;
using Xunit;

namespace Tests.Requests
{
    delegate void RequestTransition();

    public class RequestBaseTests
    {
        [Fact]
        public void RequestIsCreatedWithStatusNew()
        {
            // arrange
            var expected = RequestStatus.New;
            // act
            var request = CreateTestRequest();
            // assert
            Assert.Equal(expected, request.Status);
        }

        [Fact]
        public void ChangingToSameStateChangesNothing()
        {
            // arrange
            var request = CreateTestRequest();
            request.Submit();
            var status = request.Status;
            var updated = request.DateLastUpdated;
            // act
            request.Submit();
            // assert
            Assert.Equal(status, request.Status);
            Assert.Equal(updated, request.DateLastUpdated);

        }

        [Theory]
        [InlineData(0, true)] // submit
        [InlineData(1, false)] // confirm
        [InlineData(2, false)] // cancel      
        [InlineData(3, false)] // approve
        [InlineData(4, false)] // disapprove 
        [InlineData(5, false)] // conclude
        [InlineData(6, true)] // remove
        public void New_Can_Only_Change_ToNext_ValidStates(int commandIndex, bool hasNewState)
        {
            // arrange 
            var request = CreateTestRequestWthStatus(RequestStatus.New );
            var transitions = GetRequestCommands(request);
            var originalState = request.Status;
            // act, execute each command
            transitions[commandIndex]();
            // assert
            Assert.Equal(hasNewState, request.Status != originalState);
        }

        [Theory]
        [InlineData(0, false)] // submit
        [InlineData(1, true)] // confirm
        [InlineData(2, true)] // cancel      
        [InlineData(3, false)] // approve
        [InlineData(4, false)] // disapprove 
        [InlineData(5, false)] // conclude
        [InlineData(6, false)] // remove
        public void Submitted_Can_Only_Change_ToNext_ValidStates(int commandIndex, bool hasNewState)
        {
            // arrange 
            var request = CreateTestRequestWthStatus(RequestStatus.Submitted);
           var transitions = GetRequestCommands(request);
           var originalState = request.Status;
           // act, execute each command
           transitions[commandIndex]();
           // assert
           Assert.Equal(hasNewState, (originalState != request.Status));
        }

        //  helper methods
        private RequestTransition[] GetRequestCommands(TestRequest request)
        {
            var commands = new RequestTransition[7];
            commands[0] = new RequestTransition(request.Submit);
            commands[1] = new RequestTransition(request.Confirm);
            commands[2] = new RequestTransition(request.Cancel);
            commands[3] = new RequestTransition(request.Approve);
            commands[4] = new RequestTransition(request.Disapprove);
            commands[5] = new RequestTransition(request.Conclude);
            commands[6] = new RequestTransition(request.Remove);
            return commands;
        }

        private TestRequest CreateTestRequestWthStatus(RequestStatus desiredStatus)
        {
            var request = new TestRequest();
            if (desiredStatus == RequestStatus.New) return request;

            request.Submit();
            switch (desiredStatus)
            {
                case RequestStatus.Submitted:
                    break;
                case RequestStatus.Confirmed:
                    request.Confirm();
                    break;
                case RequestStatus.Cancelled:
                    request.Cancel();
                    break;
                case RequestStatus.Approved:
                    request.Confirm();
                    request.Approve();
                    break;
                case RequestStatus.Disapproved:
                    request.Confirm();
                    request.Disapprove();
                    break;
                case RequestStatus.Concluded:
                    request.Confirm();
                    request.Disapprove();
                    request.Conclude();
                    break;
                case RequestStatus.Removed:
                    request.Confirm();
                    request.Disapprove();
                    request.Conclude();
                    request.Remove();
                    break;
                default:
                    // New throws exception
                    throw new ArgumentOutOfRangeException(nameof(desiredStatus), desiredStatus, null);
            }

            return request;
        }

        private TestRequest CreateTestRequest( )
        {
            return new TestRequest();
        }
    }



    //  helper classes
    internal class TestRequest : RequestBase
    {
        // does nothing extra
        // just enables testing of request flow  
    }
}