using FFMpegCore;
using FFMpegCore.Enums;
using Microsoft.AspNetCore.Http;
using System;
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
                //throw;
            }

            return fileName;
        }

        public async Task<FileStream> ConvertVideo(IFormFile file, string dir)
        {
            _dir = dir;

            var fileName = await GetFileNameAndSaveFile(file);

            if (String.IsNullOrEmpty(fileName))
            {
                return null;
            }

            var input = Path.Combine(_dir, fileName + ".mp4");
            var output = Path.Combine(_dir, fileName + "_converted.mp4");

            var mediaInfo = await FFProbe.AnalyseAsync(input);

            int hConvert = 0;
            int wConvert = 0;

            //we'll compress video to 480p(858 x 480)
            var maxSize = mediaInfo.PrimaryVideoStream.Height;
            
            if (mediaInfo.PrimaryVideoStream.Width > mediaInfo.PrimaryVideoStream.Height)
            {
                maxSize = mediaInfo.PrimaryVideoStream.Width;
            }

            if (maxSize > 858)
            {
                hConvert = 858;
                wConvert = 480;
            }
            else
            {
                hConvert = mediaInfo.PrimaryVideoStream.Height;
                wConvert = mediaInfo.PrimaryVideoStream.Width;
            }

            try
            {
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

                var stream = new FileStream(output, FileMode.Open);

                return stream;
            }
            catch (Exception)
            {
                return null;
                //throw;
            }
        }
    }
}