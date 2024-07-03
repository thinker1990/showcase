using Infrastructure.ImageProcessing;
using static Abstractions.PixelFormat;
using static Infrastructure.ImageProcessing.ImageEncoderExtensions;
using static OpenCvSharp.MatType;

namespace Infrastructure.UnitTests.ImageProcessing;

internal sealed class ImageUtilityTests
{
    [TestCase(Mono8)]
    [TestCase(BayerRG8)]
    public void Mono8AndBayerRg8ShouldHas8BitsPerPixelAnd1Channel(PixelFormat pixelFormat)
    {
        var matType = pixelFormat.CorrespondingMatType();

        matType.Should().Be(CV_8UC1);
    }

    [TestCase(Mono12)]
    [TestCase(BayerRG12)]
    public void Mono12AndBayerRg12ShouldHas16BitsPerPixelAnd1Channel(PixelFormat pixelFormat)
    {
        var matType = pixelFormat.CorrespondingMatType();

        matType.Should().Be(CV_16UC1);
    }

    [TestCase(Mono8)]
    [TestCase(BayerRG8)]
    public void PixelFormatShouldBeIntactWhenMatTypeImply8BitsPerPixel(PixelFormat origin)
    {
        var matType = CV_8UC1;

        var afterCorrection = matType.PixelFormatCorrection(origin);

        afterCorrection.Should().Be(origin);
    }

    [TestCase(Mono12)]
    [TestCase(BayerRG12)]
    public void PixelFormatShouldBeIntactWhenMatTypeImply16BitsPerPixel(PixelFormat origin)
    {
        var matType = CV_16UC1;

        var afterCorrection = matType.PixelFormatCorrection(origin);

        afterCorrection.Should().Be(origin);
    }

    [TestCase(Mono12)]
    [TestCase(BayerRG12)]
    public void PixelFormatShouldBeMono8WhenMatTypeImply8BitsPerPixel(PixelFormat origin)
    {
        var matType = CV_8UC1;

        var afterCorrection = matType.PixelFormatCorrection(origin);

        afterCorrection.Should().Be(Mono8);
    }

    [TestCase(Mono8)]
    [TestCase(BayerRG8)]
    public void PixelFormatShouldBeMono12WhenMatTypeImply16BitsPerPixel(PixelFormat origin)
    {
        var matType = CV_16UC1;

        var afterCorrection = matType.PixelFormatCorrection(origin);

        afterCorrection.Should().Be(Mono12);
    }

    [Test]
    public async Task ShouldConvertToImageCorrectly()
    {
        using var source = await SomeImageMatrix();

        const PixelFormat pixelFormat = Mono8;
        var image = source.ToImage(pixelFormat);

        image.Width.Should().Be(source.Width);
        image.Height.Should().Be(source.Height);
        image.PixelFormat.Should().Be(pixelFormat);
        image.PixelData.Should().HaveCount((int)source.Total() * source.ElemSize());
    }

    [Test]
    public async Task PixelFormatShouldBeAutoCorrectToMono8WhenGivenWrongHint()
    {
        var mono8Image = await ReadImage("Mono8.tiff");

        var decoded = Decode(mono8Image, Mono12);

        decoded.PixelFormat.Should().Be(Mono8);
    }

    [Test]
    public async Task PixelFormatShouldBeAutoCorrectToMono12WhenGivenWrongHint()
    {
        var mono12Image = await ReadImage("Mono12.tiff");

        var decoded = Decode(mono12Image, Mono8);

        decoded.PixelFormat.Should().Be(Mono12);
    }

    [Category(LongRunning)]
    [TestCase("Mono8.tiff", Mono8)]
    [TestCase("Mono12.tiff", Mono12)]
    [TestCase("BayerRG8.tiff", BayerRG8)]
    [TestCase("BayerRG12.tiff", BayerRG12)]
    public async Task ShouldConvertToViewableTiffCorrectly(string imageName, PixelFormat pixelFormat)
    {
        var raw = await ReadImage(imageName);
        var image = Decode(raw, pixelFormat);

        var viewableTiff = image.ToViewableTiff();

        var display = () => Display(viewableTiff, nameof(ShouldConvertToViewableTiffCorrectly));
        display.Should().NotThrow();
    }
}