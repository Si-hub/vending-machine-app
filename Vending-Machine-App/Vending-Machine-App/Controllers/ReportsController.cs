using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml; // for Excel generation
using iTextSharp.text; // for PDF generation
using iTextSharp.text.pdf;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Vending_Machine_App.Models;
using OfficeOpenXml.Style;
using System.Drawing;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Globalization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Vending_Machine_App.Controllers
{

    [Route("api/reports")]
    [ApiController]
    
    public class ReportsController : ControllerBase
    {
        private readonly VendingMachineDbContext _dbContext;

        public ReportsController(VendingMachineDbContext dbContext)
        {
            _dbContext = dbContext;
           
        }

        [HttpGet]

        public IActionResult GeneratePurchaseReport(DateTime startDate, DateTime endDate, string format = "excel")
        {
            // Fetch the purchase history data from the database based on the specified date range
            var reportData = _dbContext.Purchases
                .Where(p => p.PurchaseDate >= startDate && p.PurchaseDate <= endDate)
                .ToList();


            if (reportData == null)
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
                var headerRow = new List<string[]>() { new string[] { "Purchase ID", "Item Name", "Amount Paid", "Purchase Date"  } };
                worksheet.Cells["A6:D6"].AutoFitColumns();

                // Determine the header range (e.g. A5:D5)
                string headerRange = "A6:" + Char.ConvertFromUtf32(headerRow[0].Length + 64) + "6";

                // Popular header row data
                worksheet.Cells[headerRange].LoadFromArrays(headerRow);
                worksheet.Cells[headerRange].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[headerRange].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
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
                Document document = new Document();
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                // Set the font for the report
                var baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                var font = new iTextSharp.text.Font(baseFont, 13, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

                // Add a title to the report
                var title = new Paragraph("Purchase History Report", new iTextSharp.text.Font(baseFont, 16, iTextSharp.text.Font.BOLD, BaseColor.LIGHT_GRAY));
                title.Alignment = Element.ALIGN_CENTER;
                document.Add(title);

                // Add a table for the purchase history
                var table = new PdfPTable(3);
                table.WidthPercentage = 100;
                table.SpacingBefore = 10f;
                table.SpacingAfter = 10f;

               
                // Add table headers
                table.AddCell(new PdfPCell(new Phrase("Item", font)));
                table.AddCell(new PdfPCell(new Phrase("Amount Paid", font)));
                table.AddCell(new PdfPCell(new Phrase("Purchase Date", font)));

                // Set table body cell background color
                table.DefaultCell.BackgroundColor = BaseColor.WHITE;

                // Populate the table with purchase data
                foreach (Purchase purchase in reportData)
                {
                    // Add table rows
                    table.AddCell(purchase.ItemName);

                    // Format the amount with ZAR currency
                    PdfPCell amountCell = new PdfPCell(new Phrase(string.Format("R{0:N2}", purchase.AmountPaid)));
                    amountCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.AddCell(amountCell);

                    table.AddCell(purchase.PurchaseDate.ToString("yyyy-MM-dd hh:mm:ss tt"));
                    
                }

                // Add the table to the document
                document.Add(table);
                document.Close();

                return memoryStream.ToArray();
            }

        }

    }

}

