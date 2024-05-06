using Azure.AI.FormRecognizer.DocumentAnalysis;
using OfficeOpenXml;

namespace DocumentAIPoc.Interfaces
{
    public interface IDocumentService
    {
        /// <summary>
        /// Analyze a document using the Document Analysis service.
        /// </summary>
        /// <param name="file">The file stream to be analyzed.</param>
        /// <returns>The result of the analysis.</returns>
        Task<AnalyzeResult?> AnalyzeDocument(Stream file);

        /// <summary>
        /// Generate an Excel spreadsheet from a list of tables.
        /// </summary>
        /// <param name="tables">Tables containing rows and cols to generate the spreadsheet.</param>
        /// <returns>The excel package with the sheets and cells generated.</returns>
        ExcelPackage GenerateTablesSpreadsheet(IReadOnlyList<DocumentTable> tables);
    }
}
