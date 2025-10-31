using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;
using iTextSharp.text.pdf;
using OfficeOpenXml; // EPPlus
using System.IO; // Explicitly use System.IO for MemoryStream
using System.Reflection;

// NOTE: The following using statements were removed or kept clean:
// - Removed 'using iTextSharp.text;' to eliminate ambiguity.
// - Removed unnecessary 'using System.Xml.Linq;' and 'using System.ComponentModel;'.

namespace Corporate_Banking_Payment_Application.Utilities
{
    public static class ReportGenerator
    {
        private const string DefaultReportTitle = "Corporate Banking Application Report";

        /// <summary>
        /// Generates a report file (Excel or PDF) from a collection of data objects.
        /// </summary>
        /// <typeparam name="T">The data model type (e.g., Payment, SalaryDisbursement).</typeparam>
        /// <param name="data">The list of data objects to include in the report.</param>
        /// <param name="format">The desired output format (PDF or EXCEL).</param>
        /// <param name="reportType">The type of content (e.g., PAYMENT, SALARY) for titling.</param>
        /// <returns>A MemoryStream containing the generated file content.</returns>
        public static MemoryStream Generate<T>(
            IEnumerable<T> data,
            ReportOutputFormat format,
            ReportType reportType) where T : class
        {
            if (data == null || !data.Any())
            {
                // Return an empty file stream to avoid downstream errors, 
                // though service layer should ideally catch this.
                return new MemoryStream();
            }

            // Dynamically get all public properties from the data model (T)
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var title = $"{reportType} Report - {DateTime.Now:yyyy-MM-dd}";

            return format switch
            {
                ReportOutputFormat.EXCEL => GenerateExcel(data, properties, title),
                ReportOutputFormat.PDF => GeneratePdf(data, properties, title),
                _ => throw new ArgumentException($"Unsupported report format: {format}")
            };
        }

        // --- Private Generation Methods ---

        /// <summary>
        /// Generates an Excel file using EPPlus.
        /// </summary>
        private static MemoryStream GenerateExcel<T>(IEnumerable<T> data, PropertyInfo[] properties, string title)
        {
            // Set the license context for EPPlus
            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage.License.SetNonCommercialPersonal("Corporate Banking Application");

            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add(title);

                // Add Headers
                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i].Name;
                }

                // Add Data Rows
                int row = 2;
                foreach (var item in data)
                {
                    for (int i = 0; i < properties.Length; i++)
                    {
                        var value = properties[i].GetValue(item);
                        worksheet.Cells[row, i + 1].Value = value;
                    }
                    row++;
                }

                // AutoFit columns and apply header styling
                worksheet.Cells.AutoFitColumns();
                worksheet.Cells["A1:" + worksheet.Cells[1, properties.Length].Address].Style.Font.Bold = true;

                package.Save();
            }
            stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// Generates a PDF file using iTextSharp.
        /// </summary>
        private static MemoryStream GeneratePdf<T>(IEnumerable<T> data, PropertyInfo[] properties, string title)
        {
            var stream = new MemoryStream();
            // FIX: Use fully qualified names for ALL iTextSharp objects
            var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), 10f, 10f, 10f, 0f);
            var writer = PdfWriter.GetInstance(document, stream);

            document.Open();

            // 1. Title
            var titleFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 16);
            document.Add(new iTextSharp.text.Paragraph(DefaultReportTitle, titleFont) { Alignment = iTextSharp.text.Element.ALIGN_CENTER });
            document.Add(new iTextSharp.text.Paragraph(title, titleFont) { Alignment = iTextSharp.text.Element.ALIGN_CENTER });
            document.Add(new iTextSharp.text.Paragraph(Environment.NewLine));

            // 2. Create Table
            var pdfTable = new PdfPTable(properties.Length)
            {
                WidthPercentage = 100,
                DefaultCell = { Padding = 3 }
            };

            // Add Headers
            var headerFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 8);
            foreach (var prop in properties)
            {
                var cell = new PdfPCell(new iTextSharp.text.Phrase(prop.Name, headerFont));
                cell.BackgroundColor = iTextSharp.text.BaseColor.LightGray;
                pdfTable.AddCell(cell);
            }

            // Add Data Rows
            var dataFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 8);
            foreach (var item in data)
            {
                foreach (var prop in properties)
                {
                    var value = prop.GetValue(item)?.ToString() ?? string.Empty;
                    // Format DateTime to be more readable in PDF
                    if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                    {
                        if (DateTime.TryParse(value, out DateTime dt))
                        {
                            value = dt.ToString("yyyy-MM-dd HH:mm");
                        }
                    }
                    pdfTable.AddCell(new iTextSharp.text.Phrase(value, dataFont));
                }
            }

            document.Add(pdfTable);
            document.Close();

            stream.Position = 0;
            return stream;
        }
    }
}
