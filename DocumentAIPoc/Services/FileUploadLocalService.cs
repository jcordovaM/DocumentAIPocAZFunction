using Azure.Storage.Files.Shares;
using DocumentAIPoc.Interfaces;

namespace DocumentAIPoc.Services
{
    public class FileUploadLocalService// : IFileUploadService
    {
        string uploadPath = @".\UploadedFiles\";

        public Stream GetFile(string filename)
        {
            try
            {
                using var fileStream = new FileStream(Path.Combine(uploadPath, filename), FileMode.Open);
                return fileStream;
            }
            catch (Exception ex)
            {
                throw new Exception("File Download Failed", ex);
            }
        }


    }
}
