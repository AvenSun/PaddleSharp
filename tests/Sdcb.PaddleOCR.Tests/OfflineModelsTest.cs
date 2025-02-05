using OpenCvSharp;
using Sdcb.PaddleOCR.Models;
using Sdcb.PaddleOCR.Models.LocalV3;
using Xunit.Abstractions;

namespace Sdcb.PaddleOCR.Tests;

public class OfflineModelsTest
{
    private readonly ITestOutputHelper _console;

    public OfflineModelsTest(ITestOutputHelper console)
    {
        _console = console;
    }

    [Fact]
    public async Task FastCheckOCR()
    {
        FullOcrModel model = LocalFullModels.EnglishV3;

        byte[] sampleImageData;
        string sampleImageUrl = @"https://visualstudio.microsoft.com/wp-content/uploads/2021/11/Home-page-extension-visual-updated.png";
        using (HttpClient http = new())
        {
            _console.WriteLine("Download sample image from: " + sampleImageUrl);
            sampleImageData = await http.GetByteArrayAsync(sampleImageUrl);
        }

        using (PaddleOcrAll all = new(model)
        {
            AllowRotateDetection = true,
            Enable180Classification = false,
        })
        {
            // Load local file by following code:
            // using (Mat src2 = Cv2.ImRead(@"C:\test.jpg"))
            using (Mat src = Cv2.ImDecode(sampleImageData, ImreadModes.Color))
            {
                PaddleOcrResult result = all.Run(src);
                _console.WriteLine("Detected all texts: \n" + result.Text);
                foreach (PaddleOcrResultRegion region in result.Regions)
                {
                    _console.WriteLine($"Text: {region.Text}, Score: {region.Score}, RectCenter: {region.Rect.Center}, RectSize:    {region.Rect.Size}, Angle: {region.Rect.Angle}");
                }
            }
        }
    }

    [Fact]
    public async Task QueuedOCR()
    {
        FullOcrModel model = LocalFullModels.EnglishV3;

        byte[] sampleImageData;
        string sampleImageUrl = @"https://visualstudio.microsoft.com/wp-content/uploads/2021/11/Home-page-extension-visual-updated.png";
        using (HttpClient http = new())
        {
            _console.WriteLine("Download sample image from: " + sampleImageUrl);
            sampleImageData = await http.GetByteArrayAsync(sampleImageUrl);
        }

        using QueuedPaddleOcrAll all = new(() => new PaddleOcrAll(model)
        {
            AllowRotateDetection = true,
            Enable180Classification = false,
        }, consumerCount: 1, boundedCapacity: 64);
        all.WaitFactoryReady();

        {
            // Load local file by following code:
            // using (Mat src2 = Cv2.ImRead(@"C:\test.jpg"))
            using (Mat src = Cv2.ImDecode(sampleImageData, ImreadModes.Color))
            {
                PaddleOcrResult result = await all.Run(src);
                _console.WriteLine("Detected all texts: \n" + result.Text);
                foreach (PaddleOcrResultRegion region in result.Regions)
                {
                    _console.WriteLine($"Text: {region.Text}, Score: {region.Score}, RectCenter: {region.Rect.Center}, RectSize:    {region.Rect.Size}, Angle: {region.Rect.Angle}");
                }
            }
        }
    }
}