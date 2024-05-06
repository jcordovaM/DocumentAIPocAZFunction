using Azure.Storage.Files.Shares;

namespace DocumentAIPoc.Interfaces
{
    public interface IFileManager
    {
        /// <summary>
        /// Get a file from the file share.
        /// </summary>
        /// <param name="filename">The filename to get.</param>
        /// <returns>The file stream.</returns>
        Stream GetFile(string filename);

        /// <summary>
        /// Get a file client from the file share.
        /// </summary>
        /// <param name="filename">The filename of the file to use.</param>
        /// <returns>The file client.</returns>
        ShareFileClient GetFileClient(string filename);

        //string GetCleanFilename(string filename);
    }
}
