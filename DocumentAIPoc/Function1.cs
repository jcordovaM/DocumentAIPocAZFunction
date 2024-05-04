using Azure;
using Azure.Storage.Files.Shares;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using DocumentAIPoc.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text;
using Azure.Storage.Files.Shares.Models;

namespace DocumentAIPoc
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;
        readonly IDocumentService _documentService;
        readonly IFileUploadService _fileUploadService;

        public Function1(ILogger<Function1> logger, IDocumentService documentService, IFileUploadService fileUploadService)
        {
            _logger = logger;
            _documentService = documentService;
            _fileUploadService = fileUploadService;
        }


        [Function("Analyze")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            string filename = req.Query.Single(q => q.Key == "filename").Value.ToString();

            try
            {
                var fileClient = _fileUploadService.GetFileClient(filename);
                ShareFileDownloadInfo download = await fileClient.DownloadAsync();
                MemoryStream ms1 = new MemoryStream();
                await download.Content.CopyToAsync(ms1);
                ms1.Position = 0;

                Task<AnalyzeResult?> task = _documentService.AnalyzeDocument(ms1);
                AnalyzeResult? document = task.Result;
                if (document != null)
                {
                    var excel = _documentService.GenerateTablesSpreadsheet(document.Tables);
                    var ms2 = new MemoryStream();
                    ms2.Write(excel.GetAsByteArray(), 0, excel.GetAsByteArray().Length);
                    ms2.Position = 0;
                    excel.Dispose();

                    return new FileContentResult(ms2.ToArray(), "application/octet-stream")
                    {
                        FileDownloadName = $"{Path.GetFileNameWithoutExtension(filename)}.xlsx"
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Document Analysis Failed", ex);
            }
            finally
            {
                _logger.LogInformation("C# HTTP trigger analyze document function processed a request.");
            }

            return new ObjectResult("Document Analysis Failed");
        }
    }
}
