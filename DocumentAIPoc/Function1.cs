using Azure.AI.FormRecognizer.DocumentAnalysis;
using DocumentAIPoc.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text;

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


        [Function("Function1")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            string filename = req.Query.Single(q => q.Key == "filename").Value.ToString();

            try
            {
                Task<AnalyzeResult?> task = _documentService.AnalyzeDocument(_fileUploadService.GetFile(filename));
                AnalyzeResult? document = task.Result;

                if (document != null)
                {
                    var excel = _documentService.GenerateTablesSpreadsheet(document.Tables);
                    //var stream = new MemoryStream();
                    //stream.Write(excel.GetAsByteArray(), 0, excel.GetAsByteArray().Length);
                    //stream.Position = 0;
                    excel.Dispose();

                    //return File(stream, "application/octet-stream", $"{Path.GetFileNameWithoutExtension(filename)}.xlsx");

                    req.HttpContext.Response.Headers.Add("content-disposition", $"attachment;filename={Path.GetFileNameWithoutExtension(filename)}.xlsx");
                    req.HttpContext.Response.ContentType = "application/octet-stream";

                    return (ActionResult)new OkObjectResult(excel.GetAsByteArray());
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Document Analysis Failed", ex);
            }

            //_logger.LogInformation("C# HTTP trigger function processed a request.");
            return new ObjectResult("Document Analysis Failed");
        }
    }
}
