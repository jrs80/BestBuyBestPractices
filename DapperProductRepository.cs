using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace BestBuyBestPractices
{
    internal class DapperProductRepository:IProductRepository
    {
        private readonly IDbConnection dbConnection;
        public DapperProductRepository(IDbConnection dbC)
        {
            dbConnection = dbC;
        }

        public IEnumerable<Products> GetAllProducts()
        {
            return dbConnection.Query<Products>("SELECT * FROM Products;");
        }

        public bool CreateProduct(string prodName, double price, int catID)
        {
            int prodCreated = dbConnection.Execute("INSERT INTO Products (Name, Price, CategoryID) VALUES (@pn,@pr,@c);",
                new { pn=prodName, pr=price, c=catID });
            return prodCreated != 0;
        }

        public bool UpdateProduct(int prodID, string prodName, double price, int catID)
        {
            int prodUpdated = dbConnection.Execute("update Products set Name=@pn, price=@pr, categoryid=@c where productid=@pid;",
                new { pn = prodName, pr = price, c = catID, pid = prodID });
            return prodUpdated != 0;
        }

        public IEnumerable<Products> GetSingleProduct(int prodID)
        {
            return dbConnection.Query<Products>("SELECT * FROM Products WHERE ProductID=@pID;",
                new { pID = prodID });
        }
        
        public bool DeleteProduct(int prodID)
        {
            int rowsDeleted = dbConnection.Execute(
                "DELETE FROM products, sales, reviews " +
                "USING products left join sales on sales.productid=products.productid left join reviews on products.productid=reviews.productid " +
                "WHERE products.ProductID=@pid;",
                new { pid = prodID });
            return rowsDeleted != 0;
        }

    }
}
