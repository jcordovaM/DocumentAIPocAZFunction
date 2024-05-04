namespace DocumentAIPoc.Interfaces
{
    public interface IFileUploadService
    {
        Stream GetFile(string filename);
        //string GetCleanFilename(string filename);
    }
}
