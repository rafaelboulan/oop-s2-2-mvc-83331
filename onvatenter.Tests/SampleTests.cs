using Xunit;

namespace onvatenter.Tests
{
    public class SampleTests
    {
        [Fact]
        public void Sample_Test_Passes()
        {
            // Arrange & Act
            var result = 1 + 1;

            // Assert
            Assert.Equal(2, result);
        }
    }
}
