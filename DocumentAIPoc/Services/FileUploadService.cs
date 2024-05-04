using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using DocumentAIPoc.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DocumentAIPoc.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IConfiguration Config;
        private readonly ShareClient shareClient;
        private readonly ShareDirectoryClient dirClient;
        private string folderName = "";

        public FileUploadService(IConfiguration configuration)
        {
            Config = configuration;
            shareClient = new(Config["Storage:ConnectionString"], Config["AzureFileShare:AzureShareName"]);
            dirClient = shareClient.GetDirectoryClient(Config["Storage:RootDirectoryName"]);
        }

        public Stream GetFile(string filename)
        {
            ShareFileClient file = dirClient.GetFileClient(filename);
            if (file.Exists())
            {
                return file.OpenReadAsync().Result;
            }
            else
            {
                throw new Exception("File Download from Cloud Failed");
            }
        }

        public string GetCleanFilename(string filename)
        {
            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
            string cleanFilename = new string(filename.Where(ch => !invalidFileNameChars.Contains(ch)).ToArray());
            return cleanFilename;
        }
    }
}
