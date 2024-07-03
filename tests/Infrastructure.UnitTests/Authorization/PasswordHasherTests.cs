using static Infrastructure.Authorization.PasswordHasher;

namespace Infrastructure.UnitTests.Authorization;

internal sealed class PasswordHasherTests
{
    private const string Password = "password";
    private const string Salt = "salt";

    [TestCase("")]
    [TestCase(" ")]
    public void ShouldThrowExceptionWhenPasswordIsEmpty(string password)
    {
        var hash = () => Hash(password, Salt);

        hash.Should().Throw<ArgumentException>();
    }

    [TestCase("")]
    [TestCase(" ")]
    public void ShouldThrowExceptionWhenSaltIsEmpty(string salt)
    {
        var hash = () => Hash(Password, salt);

        hash.Should().Throw<ArgumentException>();
    }

    [Test]
    public void HashShouldNotBeSameAsOriginalPassword()
    {
        var hash = Hash(Password, Salt);

        hash.Should().NotBeSameAs(Password);
    }

    [Test]
    public void HashShouldBeTheSameWhenHashMultiplyTimes()
    {
        var hash1 = Hash(Password, Salt);
        var hash2 = Hash(Password, Salt);

        hash1.Should().Be(hash2);
    }

    [Test]
    public void HashShouldBeDifferentIfPasswordAreDifferent()
    {
        const string password1 = "Password";
        const string password2 = "Passw0rd";

        var hash1 = Hash(password1, Salt);
        var hash2 = Hash(password2, Salt);

        hash1.Should().NotBeSameAs(hash2);
    }

    [Test]
    public void HashShouldBeDifferentIfSaltAreDifferent()
    {
        const string salt1 = "salt";
        const string salt2 = "sa1t";

        var hash1 = Hash(Password, salt1);
        var hash2 = Hash(Password, salt2);

        hash1.Should().NotBeSameAs(hash2);
    }
}