using Azure.Storage.Files.Shares;

namespace DocumentAIPoc.Interfaces
{
    public interface IFileManager
    {
        Stream GetFile(string filename);
        ShareFileClient GetFileClient(string filename);
        //string GetCleanFilename(string filename);
    }
}
