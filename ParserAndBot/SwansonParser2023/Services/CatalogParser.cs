using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SwansonParser2023.Services;

public class CatalogParser
{
    public int Page { get; set; }

    private bool _hasMore = true;
    
    public bool HasMore => _hasMore;

    public List<SiteProduct> Parse(string catalogUrl)
    {
        var products = new List<SiteProduct>();

        using (var client = new HttpClient())
        {
            Console.WriteLine($"Page {Page}");
            var url = catalogUrl + "?page=" + Page.ToString();

            var html = client.GetStringAsync(url).Result;
            //File.WriteAllText("page.html", html);

            var pattern = @"adobeRecords"":(.+),""topProduct";
            var matches = Regex.Matches(html, pattern);
            

            if (matches.Count > 0)
            {
                var json = matches[0].Groups[1].Value;
                var pageProducts = JsonSerializer.Deserialize<List<SiteProduct>>(json);
                Console.WriteLine($"{Page} = {pageProducts.Count}");
                products.AddRange(pageProducts);
                if (pageProducts.Count == 0)
                {
                    Console.WriteLine("Done");
                    _hasMore = false;
                }
                Page++;
            } else
            {
                Console.WriteLine("Done");
                _hasMore = false;
            }
        }
        return products;
    }
}
