namespace Redox.DataModel.Builder.Services
{
    using Microsoft.Extensions.Logging;
    using Redox.DataModel.Builder.Extensions;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    internal class DownloadService : IDownloadService
    {
        private readonly ILogger<DownloadService> _logger;

        public DownloadService(ILogger<DownloadService> logger)
        {
            _logger = logger;
        }

        public async Task<Dictionary<string, byte[]>> DownloadFileAsync(string fileUri)
        {
            _logger.LogInformation(nameof(DownloadFileAsync));
            var result = new Dictionary<string, byte[]>();
            using (HttpClientHandler httpClientHandler = new HttpClientHandler())
            {
                using var httpClient = new HttpClient(httpClientHandler);
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("Accept", "application/zip");
                using var response = await httpClient.GetAsync(fileUri).ConfigureAwait(false);
                using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                result = UnZipToMemory(stream);
            }
            return result;
        }

        public async Task<HttpStatusCode> DownloadFileAsync(string fileUri, string destinationFolder)
        {
            _logger.LogInformation(nameof(DownloadFileAsync));
            var fileName = GetFileNameFromUrl(fileUri);
            var destinationFile = Path.Combine(destinationFolder, fileName);
            var result = HttpStatusCode.BadRequest;
            using (HttpClientHandler httpClientHandler = new HttpClientHandler())
            {
                using var httpClient = new HttpClient(httpClientHandler);
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("Accept", "application/zip");
                using var response = await httpClient.GetAsync(fileUri).ConfigureAwait(false);
                using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                using var file = File.OpenWrite(destinationFile);
                stream.CopyTo(file);
                result = response.StatusCode;
            }
            return result;
        }

        private static Dictionary<string, byte[]> UnZipToMemory(Stream zipStream)
        {
            var result = new Dictionary<string, byte[]>();
            zipStream.Position = 0;
            var zipArchive = new ZipArchive(zipStream);
            foreach (var entry in zipArchive.Entries.Where(e => !string.IsNullOrWhiteSpace(e.Name)))
            {
                Stream data = entry.Open();
                result.Add(entry.FullName, data.ToBytes());
            }
            return result;
        }

        private static string GetFileNameFromUrl(string url)
        {
            Uri SomeBaseUri = new Uri("http://localhost");
            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
            {
                uri = new Uri(SomeBaseUri, url);
            }
            return Path.GetFileName(uri.LocalPath);
        }
    }
}
