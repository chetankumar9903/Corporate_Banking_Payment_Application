using Corporate_Banking_Payment_Application.Models;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using System.Reflection;


namespace Corporate_Banking_Payment_Application.Utilities
{
    public static class ReportGenerator
    {
        private const string DefaultReportTitle = "Corporate Banking Application Report";



        public static MemoryStream Generate<T>(
            IEnumerable<T> data,
            ReportOutputFormat format,
            ReportType reportType) where T : class
        {
            if (data == null || !data.Any())
            {

                return new MemoryStream();
            }

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var title = $"{reportType} Report - {DateTime.Now:yyyy-MM-dd}";

            return format switch
            {
                ReportOutputFormat.EXCEL => GenerateExcel(data, properties, title),
                ReportOutputFormat.PDF => GeneratePdf(data, properties, title),
                _ => throw new ArgumentException($"Unsupported report format: {format}")
            };
        }


        private static MemoryStream GenerateExcel<T>(IEnumerable<T> data, PropertyInfo[] properties, string title)
        {

            ExcelPackage.License.SetNonCommercialPersonal("Corporate Banking Application");

            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add(title);

                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i].Name;
                }

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


                worksheet.Cells.AutoFitColumns();
                worksheet.Cells["A1:" + worksheet.Cells[1, properties.Length].Address].Style.Font.Bold = true;

                package.Save();
            }
            stream.Position = 0;
            return stream;
        }


        private static MemoryStream GeneratePdf<T>(IEnumerable<T> data, PropertyInfo[] properties, string title)
        {
            var stream = new MemoryStream();

            var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), 10f, 10f, 10f, 0f);
            var writer = PdfWriter.GetInstance(document, stream);

            document.Open();


            var titleFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 16);
            document.Add(new iTextSharp.text.Paragraph(DefaultReportTitle, titleFont) { Alignment = iTextSharp.text.Element.ALIGN_CENTER });
            document.Add(new iTextSharp.text.Paragraph(title, titleFont) { Alignment = iTextSharp.text.Element.ALIGN_CENTER });
            document.Add(new iTextSharp.text.Paragraph(Environment.NewLine));


            var pdfTable = new PdfPTable(properties.Length)
            {
                WidthPercentage = 100,
                DefaultCell = { Padding = 3 }
            };


            var headerFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 8);
            foreach (var prop in properties)
            {
                var cell = new PdfPCell(new iTextSharp.text.Phrase(prop.Name, headerFont));
                cell.BackgroundColor = iTextSharp.text.BaseColor.LightGray;
                pdfTable.AddCell(cell);
            }


            var dataFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 8);
            foreach (var item in data)
            {
                foreach (var prop in properties)
                {
                    var value = prop.GetValue(item)?.ToString() ?? string.Empty;

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
