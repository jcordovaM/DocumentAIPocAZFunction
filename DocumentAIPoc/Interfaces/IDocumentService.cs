using Azure.AI.FormRecognizer.DocumentAnalysis;
using OfficeOpenXml;

namespace DocumentAIPoc.Interfaces
{
    public interface IDocumentService
    {
        Task<AnalyzeResult?> AnalyzeDocument(Stream file);
        ExcelPackage GenerateTablesSpreadsheet(IReadOnlyList<DocumentTable> tables);
    }
}
