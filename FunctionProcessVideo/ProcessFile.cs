using CompresVideo.Services;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MakeThumbnail
{
    public class ProcessFile
    {
        private VideoFileService _videoFileService;

        public ProcessFile(VideoFileService videoFileService)
        {
            _videoFileService = videoFileService;
        }

        public async Task<HttpResponseMessage> GetThumbnailAsync(IFormFile videoFile, string dirApp)
        {
            var streamPng = await _videoFileService.MakeThumbnail(videoFile, dirApp);

            return PrepareResult(streamPng);
        }

        public async Task<HttpResponseMessage> GetCompressedVideoAsync(IFormFile videoFile, string dirApp)
        {
            var streamVideo = await _videoFileService.ConvertVideo(videoFile, dirApp);

            return PrepareResult(streamVideo);
        }

        private HttpResponseMessage PrepareResult(FileStream fileStream)
        {
            if (fileStream == null)
            {
                return null;
            }

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StreamContent(fileStream);
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = "thumbnail";
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentLength = fileStream.Length;

            return result;
        }
    }
}
