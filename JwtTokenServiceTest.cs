namespace NethuesChatboxWebApp.Test
{
    using Xunit;
    using Moq;
    using Microsoft.Extensions.Options;
    using Nethues_ChatboxWebApp;
    using Nethues_ChatboxWebApp.Services.Implementation;
    using Nethues_ChatboxWebApp.Models;

    public class JwtTokenServiceTests
    {
        private JwtToken GetTestOptions() => new JwtToken
        {
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            Key = "ThisIsASecretKeyThatIsAtLeast64BytesLongForTestingPurposes1234567890",
            ExpiresMinutes = 30,
            RefreshExpiresDays = 7
        };

        [Fact]
        public void CreateToken_ShouldReturnValidJwt()
        {
            // Arrange
            var optionsMock = new Mock<IOptions<JwtToken>>();
            optionsMock.Setup(o => o.Value).Returns(GetTestOptions());

            var service = new JwtTokenService(optionsMock.Object);
            var user = new User { Id = 1, Username = "testuser", FirstName = "Test", LastName ="Test123", PasswordHash="123334" };

            // Act
            var token = service.CreateToken(user);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(token));
            Assert.Contains(".", token); // JWT should contain at least two dots
        }

        [Fact]
        public void CreateRefreshToken_ShouldReturnValidRefreshToken()
        {
            // Arrange
            var optionsMock = new Mock<IOptions<JwtToken>>();
            optionsMock.Setup(o => o.Value).Returns(GetTestOptions());

            var service = new JwtTokenService(optionsMock.Object);

            // Act
            var refreshToken = service.CreateRefreshToken(42);

            // Assert
            Assert.Equal(42, refreshToken.UserId);
            Assert.False(string.IsNullOrEmpty(refreshToken.Token));
            Assert.True(refreshToken.ExpiresUtc > DateTime.UtcNow);
            Assert.False(refreshToken.IsRevoked);
        }

        [Theory]
        [InlineData(0, "zero")]
        [InlineData(5, "five")]
        [InlineData(19, "nineteen")]
        [InlineData(42, "forty-two")]
        [InlineData(100, "one hundred")]
        [InlineData(123, "one hundred twenty-three")]
        [InlineData(1001, "one thousand one")]
        [InlineData(1000000, "one million")]
        [InlineData(-7, "minus seven")]
        public void NumberToWords_ShouldReturnExpectedString(int number, string expected)
        {
            // Arrange
            var optionsMock = new Mock<IOptions<JwtToken>>();
            optionsMock.Setup(o => o.Value).Returns(GetTestOptions());

            var service = new JwtTokenService(optionsMock.Object);

            // Act
            var result = service.NumberToWords(number);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
