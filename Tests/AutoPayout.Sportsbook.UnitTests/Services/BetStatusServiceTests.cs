using AutoPayout.Sportsbook.Services;
using Xunit;

namespace AutoPayout.Sportsbook.UnitTests.Services
{
    public class BetStatusServiceTests
    {
        [Theory]
        [InlineData("", "FL:0-0;")]
        [InlineData("AHAH", "FL:2-1;")]
        [InlineData("AAAA", "FL:2-2;")]
        [InlineData("HAAA", "FL:1-2;")]
        [InlineData("HHHH", "FL:1-1;")]
        public void GenerateBetStatus_WithGoalRecord_ReturnsRightExpectedBetStatus(string goalRecord, string expectedBetStatus)
        {
            // Arrange
            var service = new BetStatusServiceImp();

            // Act
            var actual = service.GenerateBetStatus("FL", goalRecord);

            // Assert
            Assert.Equal(expectedBetStatus, actual);
        }
    }
}