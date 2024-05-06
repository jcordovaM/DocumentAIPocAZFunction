using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using DocumentAIPoc.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DocumentAIPoc.Services
{
    public class FileManager : IFileManager
    {
        private readonly IConfiguration Config;
        private readonly ShareClient shareClient;
        private readonly ShareDirectoryClient dirClient;

        public FileManager(IConfiguration configuration)
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
                throw new Exception("File download from Cloud failed");
            }
        }

        public ShareFileClient GetFileClient(string filename)
        {
            ShareFileClient file = dirClient.GetFileClient(filename);
            if (file.Exists())
            {
                return file;
            }
            else
            {
                throw new Exception("File client download from Cloud failed, does not exist");
            }
        }

        public string GetCleanFilename(string filename)
        {
            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
            string cleanFilename = new(filename.Where(ch => !invalidFileNameChars.Contains(ch)).ToArray());
            return cleanFilename;
        }
    }
}
