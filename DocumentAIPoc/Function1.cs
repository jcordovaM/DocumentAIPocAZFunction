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
using OfficeOpenXml;

namespace DocumentAIPoc
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;
        readonly IDocumentService _documentService;
        readonly IFileManager _fileManager;

        public Function1(ILogger<Function1> logger, IDocumentService documentService, IFileManager fileManager)
        {
            _logger = logger;
            _documentService = documentService;
            _fileManager = fileManager;
        }


        [Function("Analyze")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            string filename = req.Query.Single(q => q.Key == "filename").Value.ToString();

            try
            {
                ShareFileClient fileClient = _fileManager.GetFileClient(filename);
                ShareFileDownloadInfo download = await fileClient.DownloadAsync();
                MemoryStream ms1 = new MemoryStream();
                await download.Content.CopyToAsync(ms1);
                ms1.Position = 0;

                Task<AnalyzeResult?> task = _documentService.AnalyzeDocument(ms1);
                AnalyzeResult? document = task.Result;
                if (document != null)
                {
                    ExcelPackage excel = _documentService.GenerateTablesSpreadsheet(document.Tables);
                    MemoryStream ms2 = new MemoryStream();
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
                throw new Exception("Document analysis failed", ex);
            }
            finally
            {
                _logger.LogInformation("C# HTTP trigger analyze document function processed a request.");
            }

            return new ObjectResult("Document analysis failed");
        }
    }
}
