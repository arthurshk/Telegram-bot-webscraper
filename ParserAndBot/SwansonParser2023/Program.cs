// See https://aka.ms/new-console-template for more information
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using SwansonParser2023;
using SwansonParser2023.Models;
using SwansonParser2023.Services;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

Console.WriteLine("Hello, World!");

/*
 * https://media.swansonvitamins.com/images/items/master/SW1876.jpg
https://www.swansonvitamins.com/ncat1/Vitamins+and+Supplements/ncat2/Multivitamins/ncat3/Multivitamins+with+Iron/q
https://www.swansonvitamins.com/ncat1/Vitamins+and+Supplements/ncat2/Multivitamins/ncat3/Multivitamins+with+Iron/q?page=2
https://www.swansonvitamins.com/ncat1/Vitamins+and+Supplements/ncat2/Multivitamins/ncat3/Multivitamins+with+Iron/q?page=3
 * 
 * https://www.swansonvitamins.com/q
 */

//var html = File.ReadAllText("page.html");

var catalogUrl = "https://www.swansonvitamins.com/q";

var parser = new CatalogParser() { Page = 1 };

//IProductWriter writer = new ExcelProductWriter();
//writer.SaveAs("products123.xlsx", products);

using (var db = new ProductsContext())
{
    var storage = new ProductStorage(db);
    while (parser.HasMore)
    {
        var products = parser.Parse(catalogUrl);
        storage.Update(products);
        Thread.Sleep(3000);
    }
}
