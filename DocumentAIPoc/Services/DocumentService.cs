using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using DocumentAIPoc.Interfaces;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;

namespace DocumentAIPoc.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IConfiguration Config;
        private readonly AzureKeyCredential credential;
        private readonly DocumentAnalysisClient docAIClient;

        public DocumentService(IConfiguration configuration)
        {
            try
            {
                Config = configuration;
                credential = new AzureKeyCredential(Config["DocumentAI:Key"]);
                docAIClient = new DocumentAnalysisClient(new Uri(Config["DocumentAI:Endpoint"]), credential);
            }
            catch (Exception ex)
            {
                throw new Exception("Document Service Initialization Failed", ex);
            }
        }

        public async Task<AnalyzeResult?> AnalyzeDocument(Stream file)
        {
            try
            {
                AnalyzeDocumentOperation operation = await docAIClient.AnalyzeDocumentAsync(WaitUntil.Completed, Config["DocumentAI:ModelId"], file);
                AnalyzeResult result = operation.Value;
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Analyze document failed.", ex);
            }
        }

        public ExcelPackage GenerateTablesSpreadsheet(IReadOnlyList<DocumentTable> tables)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage excel = new ExcelPackage();
                ExcelWorksheet workSheet;

                int i = 1;
                foreach (var table in tables)
                {
                    workSheet = excel.Workbook.Worksheets.Add($"Sheet{i}");
                    foreach (var cell in table.Cells)
                    {
                        int row = int.Parse(cell.RowIndex.ToString()) + 1;
                        int col = int.Parse(cell.ColumnIndex.ToString()) + 1;
                        workSheet.Cells[row, col].Value = cell.Content;
                        workSheet.Column(col).AutoFit();
                        if (row == 1)
                        {
                            workSheet.Cells[row, col].Style.Font.Bold = true;
                        }
                    }
                    i++;
                }
                excel.Save();

                return excel;
            }
            catch (Exception ex)
            {
                throw new Exception("Generate Spreadsheet failed.", ex);
            }

        }

    }
}
