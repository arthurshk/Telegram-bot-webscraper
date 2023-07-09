using SwansonParser2023.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwansonParser2023.Services;

public class ProductStorage
{
    private ProductsContext _context;
    public ProductStorage(ProductsContext context)
    {
        _context = context;
    }

    public Product? FindByCode(string code)
    {
        return _context.Products.FirstOrDefault(x => x.Code == code);
    }

    public void Update(List<SiteProduct> products)
    {
        products.ForEach(p =>
        {
            var dbProduct = FindByCode(p.Number);
            if (dbProduct == null)
            {
                dbProduct = new Product();
                dbProduct.CreatedAt = DateTime.Now;
                _context.Add(dbProduct);
            }
            dbProduct.Code = p.Number;
            dbProduct.Title = p.Title;
            dbProduct.Vendor = p.Vendor;
            dbProduct.Description = p.Details;
            dbProduct.Price = Decimal.Parse(p.Price, CultureInfo.InvariantCulture);
            dbProduct.UpdatedAt = DateTime.Now;
            dbProduct.Available = p.Status == "In stock";
            dbProduct.FullUrl = p.FullUrl;
            dbProduct.ImageUrl = p.ImageUrl;
        });
        _context.SaveChanges();
    }
}
