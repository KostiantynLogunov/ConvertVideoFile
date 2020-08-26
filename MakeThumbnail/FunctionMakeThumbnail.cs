using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using CompressVideo.Services;
using System.Linq;
using System.Net.Http;

namespace MakeThumbnail
{
    public static class FunctionMakeThumbnail
    {
        [FunctionName("function-make-thumbnail")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var videoFile = req.Form.Files.First();

            var services = new ServiceCollection();
            
            services.AddTransient<VideoFileService>();
            services.AddTransient<ProcessFile>();

            var provider = services.BuildServiceProvider();

            var processFile = provider.GetService<ProcessFile>();

            var result = await processFile.GetThumbnailAsync(videoFile);

            return result;
        }
    }
}