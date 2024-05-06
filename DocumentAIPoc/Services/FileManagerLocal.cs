using Azure.Storage.Files.Shares;
using DocumentAIPoc.Interfaces;

namespace DocumentAIPoc.Services
{
    public class FileManagerLocal// : IFileManager
    {
        string uploadPath = @".\UploadedFiles\";

        public Stream GetFile(string filename)
        {
            try
            {
                using FileStream fileStream = new(Path.Combine(uploadPath, filename), FileMode.Open);
                return fileStream;
            }
            catch (Exception ex)
            {
                throw new Exception("File download failed", ex);
            }
        }


    }
}
