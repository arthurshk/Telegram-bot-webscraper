using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using SwansonParser2023.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwansonParser2023.Services;

public class ExcelProductWriter : IProductWriter
{
    public void SaveAs(string file, List<SiteProduct> products)
    {
        using (var workbook = new XLWorkbook())
        {
            var sheet = workbook.Worksheets.Add("Products");

            sheet.Cell("A1").Value = "Vendor";
            sheet.Cell("B1").Value = "Title";
            sheet.Cell("C1").Value = "Number";
            sheet.Cell("D1").Value = "Details";
            sheet.Cell("E1").Value = "Price";
            sheet.Cell("F1").Value = "Status";
            sheet.Cell("G1").Value = "Url";
            sheet.Cell("H1").Value = "Image";

            int row = 2;

            products.ForEach(p =>
            {
                sheet.Cell($"A{row}").Value = p.Vendor;
                sheet.Cell($"B{row}").Value = p.Title;
                sheet.Cell($"C{row}").Value = p.Number;
                sheet.Cell($"D{row}").Value = p.Details;
                sheet.Cell($"E{row}").Value = p.Price;
                sheet.Cell($"F{row}").Value = p.Status;
                sheet.Cell($"G{row}").Value = p.FullUrl;
                sheet.Cell($"H{row}").Value = p.ImageUrl;
                row++;
            });

            workbook.SaveAs(file);

        }
    }

    public void SaveAs(string file, List<Product> products)
    {
        using (var workbook = new XLWorkbook())
        {
            var sheet = workbook.Worksheets.Add("Products");

            sheet.Cell("A1").Value = "Vendor";
            sheet.Cell("B1").Value = "Title";
            sheet.Cell("C1").Value = "Number";
            sheet.Cell("D1").Value = "Details";
            sheet.Cell("E1").Value = "Price";
            sheet.Cell("F1").Value = "Status";
            sheet.Cell("G1").Value = "Url";
            sheet.Cell("H1").Value = "Image";

            int row = 2;

            products.ForEach(p =>
            {
                sheet.Cell($"A{row}").Value = p.Vendor;
                sheet.Cell($"B{row}").Value = p.Title;
                sheet.Cell($"C{row}").Value = p.Code;
                sheet.Cell($"D{row}").Value = p.Description;
                sheet.Cell($"E{row}").Value = p.Price;
                sheet.Cell($"F{row}").Value = p.Available ? "Available": "Not available";
                sheet.Cell($"G{row}").Value = p.FullUrl;
                sheet.Cell($"H{row}").Value = p.ImageUrl;
                row++;
            });

            workbook.SaveAs(file);

        }
    }
}
