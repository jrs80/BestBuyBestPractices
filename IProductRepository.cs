using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BestBuyBestPractices
{
    internal interface IProductRepository
    {
        IEnumerable<Products> GetAllProducts();

        bool CreateProduct(string name, double price, int categoryID);
        IEnumerable<Products> GetSingleProduct(int prodID);
        bool UpdateProduct(int prodID, string name, double price, int categoryID);
        bool DeleteProduct(int productID);


    }
}
