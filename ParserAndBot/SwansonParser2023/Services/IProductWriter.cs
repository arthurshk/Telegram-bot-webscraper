using SwansonParser2023.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwansonParser2023.Services;

public interface IProductWriter
{
    public void SaveAs(string file, List<SiteProduct> products);
    public void SaveAs(string file, List<Product> products);
}
