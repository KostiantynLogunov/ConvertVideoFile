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
using CompressVideo.Services;

namespace CompressVideo.Controllers
{
    public class HomeController : Controller
    {
        private string _dir;
        private VideoFileService _videoFileService;

        public HomeController( IHostingEnvironment env, VideoFileService videoFileService)
        {
            _dir = env.WebRootPath;
            _videoFileService = videoFileService;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> Index(IFormFile file) 
        {
            await _videoFileService.MakeThumbnail(file);

            await _videoFileService.ConvertVideo(file);

            return RedirectToAction("Index");
        }
    }
}