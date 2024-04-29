using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YoutubeDownloader.BaseClass;
using YoutubeExplode;

namespace YoutubeDownloader.Extensions
{
    public static class YoutubeClientExtensions
    {

        public static async Task<DownloadedVideo> DownloadYouTubeVideoAsync(this YoutubeClient youtube, string videoUrl, string outputDirectory)
        {
            try
            {
                var video = await youtube.Videos.GetAsync(videoUrl);
                if (video == null)
                    throw new InvalidOperationException("Video not found.");

                // Sanitize the video title to remove invalid characters from the file name
                string sanitizedTitle = string.Join("_", video.Title.Split(Path.GetInvalidFileNameChars()));

                // Get all available muxed streams
                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
                var muxedStreams = streamManifest.GetMuxedStreams().OrderByDescending(s => s.VideoQuality).ToList();

                if (!muxedStreams.Any())
                {
                    throw new InvalidOperationException($"No suitable video stream found for {video.Title}");
                }

                var streamInfo = muxedStreams.First();

                using (var httpClient = new HttpClient())
                {
                    var stream = await httpClient.GetStreamAsync(streamInfo.Url);

                    // Verificar si el directorio de salida existe, si no, crearlo
                    if (!Directory.Exists(outputDirectory))
                    {
                        Directory.CreateDirectory(outputDirectory);
                    }

                    string outputFilePath = Path.Combine(outputDirectory, $"{sanitizedTitle}.{streamInfo.Container}");

                    using (var outputStream = File.Create(outputFilePath))
                    {
                        await stream.CopyToAsync(outputStream);
                    }

                    // Crear un objeto VideoDescargado con los datos relevantes y devolverlo
                    return new DownloadedVideo(sanitizedTitle, videoUrl, streamInfo.Container.ToString(), DateTime.Now, outputFilePath);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error downloading video: {ex.Message}");
            }
        }

        #region Youtube urls

        public static async Task<List<Encolado>> ObtenerURLsYTTitleAsync(this string clipboardText)
        {
            var urls = clipboardText.ObtenerURLs();
            var results = new List<Encolado>();

            foreach (var url in urls)
            {
                if (EsURLYouTube(url))
                {
                    var title = await GetYouTubeVideoTitle(url);
                    results.Add(new Encolado(url, title));
                }
            }

            return results;
        }

        private static string[] ObtenerURLs(this string clipboardText)
        {
            var urlsEncontradas = Regex.Matches(clipboardText, @"(https?://\S+)");
            var urls = new List<string>();

            foreach (Match match in urlsEncontradas)
            {
                urls.Add(match.Groups[1].Value);
            }

            return urls.ToArray();
        }

        private static bool EsURLYouTube(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
                   uri.Host.EndsWith("youtube.com", StringComparison.OrdinalIgnoreCase);
        }

        private static async Task<string> GetYouTubeVideoTitle(string videoUrl)
        {
            try
            {
                HttpClient client = new HttpClient();
                string html = await client.GetStringAsync(videoUrl);

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);

                HtmlNode titleNode = doc.DocumentNode.SelectSingleNode("//meta[@name='title']");
                if (titleNode != null)
                {
                    return titleNode.GetAttributeValue("content", "");
                }
                else
                {
                    return "Título no encontrado";
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        #endregion
    }
}
