using Azure.Storage.Files.Shares;

namespace DocumentAIPoc.Interfaces
{
    public interface IFileUploadService
    {
        Stream GetFile(string filename);
        ShareFileClient GetFileClient(string filename);
        //string GetCleanFilename(string filename);
    }
}
