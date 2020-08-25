using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using CompressVideo.BackgroundTasks;
using FFMpegCore;
using FFMpegCore.Enums;
using System.Drawing;

namespace CompressVideo.Controllers
{
    public class HomeController : Controller
    {
        private string _dir;
        private IBackgroundQueue _queue;

        public HomeController( IHostingEnvironment env, IBackgroundQueue queue)
        {
            _dir = env.WebRootPath;
            _queue = queue;
            FFMpegOptions.Configure(new FFMpegOptions { RootDirectory = _dir + @"\\ffmpeg", TempDirectory = _dir });
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> Index(IFormFile file) 
        {
            using (var fileStream =
                new FileStream(Path.Combine(_dir, "file.mp4"), FileMode.Create, FileAccess.Write))
            {
                await file.CopyToAsync(fileStream);
            }

            await ConvertVideo();

            return RedirectToAction("Index");
        }


        public async Task<bool> ConvertVideo()
        {
            

            var input = Path.Combine(_dir, "file.mp4");
            var output = Path.Combine(_dir, "converted.mp4");
            var outputPng = Path.Combine(_dir, Guid.NewGuid() + ".png");

            var mediaInfo = await FFProbe.AnalyseAsync(input);

            int hConvert = 480;
            int wConvert = 852;

            if (mediaInfo.PrimaryVideoStream.Height < hConvert)
            {
                hConvert = 360;
                wConvert = 640;
            }

            FFMpeg.Snapshot(mediaInfo, outputPng, new Size(852, 480), TimeSpan.FromSeconds(5));

            FFMpegArguments
                .FromInputFiles(input)
                .WithVideoCodec(VideoCodec.LibX264)
                .WithConstantRateFactor(21)
                .WithAudioCodec(AudioCodec.Aac)
                .WithVariableBitrate(4)
                .WithFastStart()
                .Scale(wConvert, hConvert)
                .OutputToFile(output)
                
                .ProcessSynchronously();

            return true;
        }


        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            using (var fileStream =
                new FileStream(Path.Combine(_dir, "file.mp4"), FileMode.Create, FileAccess.Write))
            {
                await file.CopyToAsync(fileStream);
            }

            return Ok();
        }

        /*[HttpPost]
        public IActionResult TrimVideo(double start, double end)
        {
            _queue.QueueTask(async token =>
            {
                await ConvertVideo(start, end, token);
            });

            return RedirectToAction("Index");
        }*/

        /*public async Task<bool> ConvertVideo(double start, double end, CancellationToken ct)
        {
            try
            {
                var input = Path.Combine(_dir, "file.mp4");
                var output = Path.Combine(_dir, "converted.mp4");

                MyFFmpeg.ExecutablesPath = Path.Combine(_dir, "ffmpeg");

                var startSpan = TimeSpan.FromSeconds(start);
                var endSpan = TimeSpan.FromSeconds(end);
                var duration = endSpan - startSpan;

                var info = await MediaInfo.Get(input);

                var videoStream = info.VideoStreams.First()
                    .SetCodec(VideoCodec.h264)
                    .SetSize(VideoSize.Hd480)
                    .Split(startSpan, duration);

                await Conversion.New()
                    .AddStream(videoStream)
                    .SetOutput(output)
                    .Start(ct);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return true;
        }*/

    }


}
