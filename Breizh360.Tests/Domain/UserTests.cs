using Xunit;
using Breizh360.Domaine.Auth.Users;
using Breizh360.Domaine.Common;

namespace Breizh360.Tests.Domain
{
    public class UserTests
    {
        [Fact]
        public void SetLogin_WithTooShortValue_ThrowsDomainException()
        {
            // Arrange
            var user = new User("validlogin", "test@example.com", new string('a', 20));

            // Act & Assert
            Assert.Throws<DomainException>(() => user.SetLogin("a"));
        }

        [Fact]
        public void SetEmail_WithInvalidFormat_ThrowsDomainException()
        {
            var user = new User("validlogin", "test@example.com", new string('b', 20));
            Assert.Throws<DomainException>(() => user.SetEmail("not_an_email"));
        }
    }
}