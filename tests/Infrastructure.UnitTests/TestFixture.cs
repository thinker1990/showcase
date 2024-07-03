using Abstractions.Extensions;
using Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Infrastructure.UnitTests;

internal static class TestFixture
{
    internal static string PublicKey => "RsaPublicKey";

    internal static IEncryptionProvider NoEncryption()
    {
        var encryptor = new Mock<IEncryptionProvider>();
        encryptor.Setup(it => it.Encrypt(It.IsAny<string>()))
            .Returns((string argument) => argument);
        encryptor.Setup(it => it.Decrypt(It.IsAny<string>()))
            .Returns((string argument) => argument);

        return encryptor.Object;
    }

    internal static FileInfo ProofFile(string name) =>
        FileRelativeToBase("Activation", "proof", name);

    internal static Task<byte[]> ReadImage(string name)
    {
        var image = FileRelativeToBase("SampleImages", name);
        return image.AllBytes();
    }

    internal static void Display(byte[] image, string identifier)
    {
        using var mat = ConvertToMat(image);
        Cv2.ImShow(identifier, mat);
        Cv2.WaitKey(1500);
        Cv2.DestroyWindow(identifier);
    }

    internal static IDbContextFactory<AuthorizationContext> AuthorizationContextFactory() =>
        DbContextFactory<AuthorizationContext>("authorization");

    internal static IDbContextFactory<ConfigurationContext> ConfigurationContextFactory() =>
        DbContextFactory<ConfigurationContext>("configuration");

    private static IDbContextFactory<T> DbContextFactory<T>(string database)
        where T : DbContext
    {
        var serviceProvider = new ServiceCollection()
            .AddDbContextFactory<T>(options => options.UseSqlite($"DataSource={database}.db"))
            .BuildServiceProvider();

        return serviceProvider.GetRequiredService<IDbContextFactory<T>>();
    }
}