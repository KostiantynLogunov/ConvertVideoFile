using CompresVideo.Services;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CompresVideo
{
    public class ProcessFile
    {
        private VideoFileService _videoFileService;

        public ProcessFile(VideoFileService videoFileService)
        {
            _videoFileService = videoFileService;
        }

        public async Task<HttpResponseMessage> GetCompressedVideoAsync(IFormFile videoFile)
        {
            var streamVideo = await _videoFileService.ConvertVideo(videoFile);
            
            if (streamVideo == null)
            {
                return null;
            }

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StreamContent(streamVideo);
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = "thumbnail";
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentLength = streamVideo.Length;

            return result;
        }
    }
}
