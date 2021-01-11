namespace Redox.DataModel.Builder.Services
{
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;

    internal interface IDownloadService
    {
        Task<HttpStatusCode> DownloadFileAsync(string fileUri, string destinationFolder);

        Task<Dictionary<string, byte[]>> DownloadFileAsync(string fileUri);
    }
}