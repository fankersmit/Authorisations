using System;
using System.Linq;
using Requests.Domain;
using Xunit;

namespace Tests.Requests
{
    public class PersonTests
    {
        [Theory]
        [InlineData("Mr.", "Jan", "Dekker", 2 ) ]
        [InlineData("Mr.", "", "Dekker", 1 ) ]
        [InlineData("Mr.", "Jan", "", 1 ) ]
        [InlineData("Mr.", "", "", 0 ) ]
        [InlineData("Mr.", "Jan", "van Dekker", 3 ) ]
        [InlineData("Mr.", "Jan", "van de Dekker", 4 ) ]
        [InlineData("", "", "", 0 ) ]
        [InlineData("De heer", "Jan ", "Dekker", 3 ) ]
        [InlineData("De heer ", "Jan ", "Dekker", 3 ) ]
        public void FullNameIsCorrectlyFormatted(string salutation, string firstName, string lastName, int expected)
        {
            // arrange, act
            var p = new Person(firstName, lastName, salutation);
            // assert
            Assert.Equal(expected, p.FullName().Count(i => i == ' '));
        }
        
    }
}