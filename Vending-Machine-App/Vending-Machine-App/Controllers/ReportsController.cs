using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml; // for Excel generation
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Vending_Machine_App.Models;
using OfficeOpenXml.Style;
using System.Drawing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Layout.Properties;

namespace Vending_Machine_App.Controllers
{
    /// <summary>
    /// Controller class for generating purchase reports.
    /// </summary>
    [Route("api/reports")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly VendingMachineDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportsController"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public ReportsController(VendingMachineDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Generates a purchase report based on the specified date range and format.
        /// </summary>
        /// <param name="startDate">The start date of the report.</param>
        /// <param name="endDate">The end date of the report.</param>
        /// <param name="format">The format of the report (default is "excel").</param>
        /// <returns>The generated report file.</returns>
        [HttpGet]
        public IActionResult GeneratePurchaseReport(
            DateTime? startDate = null,
            DateTime? endDate = null,
            string format = "excel")
        {
            // If startDate is not provided, default to the last 5 days
            startDate ??= DateTime.Now.AddDays(-5);

            // If endDate is not provided, default to the current date
            endDate ??= DateTime.Now;

            // Fetch the purchase history data from the database based on the specified date range
            var reportData = _dbContext.Purchases
                .Where(p => p.PurchaseDate >= startDate && p.PurchaseDate <= endDate.Value.AddDays(1))
                .ToList();

            if (reportData == null || !reportData.Any())
            {
                return NotFound("No data found for the specified date range.");
            }

            byte[] reportBytes;

            if (format.Equals("pdf", StringComparison.OrdinalIgnoreCase))
            {
                reportBytes = GeneratePDFReport(reportData);
                return File(reportBytes, "application/pdf", "purchase_report.pdf");
            }
            else if (format.Equals("excel", StringComparison.OrdinalIgnoreCase))
            {
                reportBytes = GenerateExcelReport(reportData);
                return File(reportBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "purchase_report.xlsx");
            }
            else
            {
                return BadRequest("Invalid report format. Specify either PDF or Excel format.");
            }
        }

        private byte[] GenerateExcelReport(List<Purchase> reportData)
        {
            // Create a new Excel package using EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage())
            {
                // Create a worksheet in the Excel package
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Purchase Report");

                #region formats the headers
                // Add Excel content here based on purchase history data
                worksheet.Cells["A1"].Value = "Report Title : ";
                worksheet.Cells["A1:B1"].Merge = true;
                worksheet.Column(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Row(1).Style.Font.Size = 14;

                worksheet.Cells["D1"].Value = "Purchase History Report";
                worksheet.Cells["D1:F1"].Merge = true;
                worksheet.Column(5).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Row(1).Style.Font.Size = 14;

                worksheet.Cells["A3"].Value = "Date Created : ";
                worksheet.Cells["A3:B3"].Merge = true;
                worksheet.Column(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Row(3).Style.Font.Size = 14;

                worksheet.Cells["D3"].Value = string.Format("{0:dd MMMM yyyy} at {0:H: mm tt}", DateTimeOffset.Now);
                worksheet.Cells["D3:F3"].Merge = true;
                worksheet.Column(5).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Row(3).Style.Font.Size = 12;
                #endregion

                #region formats the column headers
                var headerRow = new List<string[]>() { new string[] { "Purchase ID", "Item Name", "Amount Paid", "Purchase Date" } };
                worksheet.Cells["A6:D6"].AutoFitColumns();

                // Determine the header range (e.g. A5:D5)
                string headerRange = "A6:" + Char.ConvertFromUtf32(headerRow[0].Length + 64) + "6";

                // Popular header row data
                worksheet.Cells[headerRange].LoadFromArrays(headerRow);
                worksheet.Cells[headerRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[headerRange].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                worksheet.Row(6).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[headerRange].Style.Font.Bold = true;
                worksheet.Cells[headerRange].Style.Font.Size = 11;
                #endregion

                var row = 7;
                foreach (var purchase in reportData)
                {
                    worksheet.Cells[$"A{row}"].Value = purchase.PurchaseId;
                    worksheet.Cells[$"B{row}"].Value = purchase.ItemName;
                    worksheet.Cells[$"C{row}"].Value = purchase.AmountPaid;
                    worksheet.Cells[$"C{row}"].Style.Numberformat.Format = "R#,##0.00";
                    worksheet.Cells[$"D{row}"].Value = purchase.PurchaseDate.ToString("yyyy-MM-dd hh:mm:ss tt");
                    row++;
                }

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                return package.GetAsByteArray();
            }
        }

        private byte[] GeneratePDFReport(List<Purchase> reportData)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Initialize PDF Writer and Document
                PdfWriter writer = new PdfWriter(memoryStream);
                PdfDocument pdfDocument = new PdfDocument(writer);
                Document document = new Document(pdfDocument);

                // Set Font (Using Helvetica)
                PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                // Add Title
                Paragraph title = new Paragraph("Purchase History Report")
                    .SetFont(boldFont)
                    .SetFontSize(16)
                    .SetFontColor(ColorConstants.BLACK)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(20);

                document.Add(title);

                // Create Table with 3 Columns
                Table table = new Table(3);
                table.SetWidth(UnitValue.CreatePercentValue(100));

                // Add Table Headers
                table.AddHeaderCell(new Cell().Add(new Paragraph("Item").SetFont(boldFont)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Amount Paid").SetFont(boldFont)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Purchase Date").SetFont(boldFont)));

                // Populate Table with Data
                foreach (Purchase purchase in reportData)
                {
                    table.AddCell(new Cell().Add(new Paragraph(purchase.ItemName).SetFont(font)));

                    // Format the amount with ZAR currency
                    table.AddCell(new Cell().Add(new Paragraph($"R{purchase.AmountPaid:N2}").SetFont(font)));

                    table.AddCell(new Cell().Add(new Paragraph(purchase.PurchaseDate.ToString("yyyy-MM-dd hh:mm:ss tt")).SetFont(font)));
                }

                // Add Table to Document
                document.Add(table);

                // Close Document
                document.Close();

                return memoryStream.ToArray();
            }
        }
    }
}