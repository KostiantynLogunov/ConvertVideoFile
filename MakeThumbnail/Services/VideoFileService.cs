using FFMpegCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace CompresVideo.Services
{
    public class VideoFileService
    {
        private string _dir;

        public async Task<string> GetFileNameAndSaveFile(IFormFile file)
        {
            //TODO fileName
            var fileName = "file";

            try
            {              
                var filePath = Path.Combine(_dir, fileName + ".mp4");

                if (!File.Exists(filePath))
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
            }
            catch (Exception)
            {
                //TODO
                //throw;
            }

            return fileName;
        }

        public async Task<FileStream> MakeThumbnail(IFormFile videoFile, string dir, int timeThumbnail = 5)
        {
            _dir = dir;

            var fileName = await GetFileNameAndSaveFile(videoFile);
           
            if (String.IsNullOrEmpty(fileName))
            {
                return null;
            }

            var input = Path.Combine(_dir, fileName + ".mp4");
            var outputPng = Path.Combine(_dir, fileName + ".png");
            var mediaInfo = await FFProbe.AnalyseAsync(input);

            var hConvert = mediaInfo.PrimaryVideoStream.Height;
            var wConvert = mediaInfo.PrimaryVideoStream.Width;

            try
            {
                FFMpeg.Snapshot(mediaInfo, outputPng, new Size(wConvert, hConvert), TimeSpan.FromSeconds(timeThumbnail));
                var stream = new FileStream(outputPng, FileMode.Open);

                return stream;
            }
            catch (Exception)
            {
                //File.Delete(outputPng);
                return null;
                //throw;
            }

            /*File.Delete(outputPng);
            File.Delete(input);*/
        }
    }
}