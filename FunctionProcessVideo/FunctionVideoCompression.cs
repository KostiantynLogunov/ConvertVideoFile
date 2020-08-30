using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using CompresVideo.Services;
using FFMpegCore;
using MakeThumbnail;

namespace CompresVideo
{
    public static class FunctionVideoCompression
    {
        [FunctionName("function-compress-video")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            var dirApp = context.FunctionAppDirectory;

            FFMpegOptions.Configure(new FFMpegOptions { RootDirectory = dirApp + @"\\ffmpeg", TempDirectory = dirApp });

            var videoFile = req.Form.Files.First();

            var services = new ServiceCollection();

            services.AddTransient<VideoFileService>(x => new VideoFileService(log));
            services.AddTransient<ProcessFile>();

            var provider = services.BuildServiceProvider();

            var processFile = provider.GetService<ProcessFile>();

            var result = await processFile.GetCompressedVideoAsync(videoFile, dirApp);

            return result;
        }
    }
}
