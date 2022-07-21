using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;


namespace BestBuyBestPractices
{
    internal class Program
    {
        static void Main(string[] args)
        {

          /*** Set up db connection: ***/
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            string connString = config.GetConnectionString("DefaultConnection");
            IDbConnection conn = new MySqlConnection(connString);

            var deptRepo = new DapperDepartmentRepository(conn);
            var prodRepo = new DapperProductRepository(conn);

            bool quit = false;

          /*** main program loop ***/
            while(!quit) {

                Console.Clear();
                SysWrite("\tBestBuy Database Manager\n\t========================");
                SysWrite("\n\n\tWhat would you like to do?");
                Console.WriteLine("\n\t1. See department list\t2. Add department\n\t3. View product list\t4. Add product\n\t5. Update product\t6. Delete product\n\t7. Quit");
                ConsoleKeyInfo k=Console.ReadKey(true);

                switch(k.KeyChar) {

                    case '1':   // print list of departments:
                       SysWrite("\n\n\tDepartment List:");
                        foreach(var v in deptRepo.GetAllDepartments()) Console.WriteLine("\t> " + v.Name);
                        break;

                    case '2':   // add department
                        SysWrite("\n\n\tEnter new department name: ");
                        deptRepo.InsertDepartment(Console.ReadLine());
                        break;

                    case '3':   // print list of products
                        SysWrite("\n\n\tProduct List:\n\tID\tName\t\t\tPrice\tCategory ID\tOn Sale\tStockLevel");
                        foreach(var v in prodRepo.GetAllProducts())
                            Console.WriteLine($"\t\t{v.ProductID}\t{v.Name}\t\t\t{v.Price}\t{v.CategoryID}\t{v.OnSale}\t{v.StockLevel}");
                        break;

                    case '4': // add product record
                        SysWrite("\n\tTo add product, please enter the following information: ");
                        Console.Write("\t\tProduct Name: ");
                        string pName=Console.ReadLine();
                        Console.Write("\t\tProduct Price: ");
                        double pPrice;
                        if(!double.TryParse(Console.ReadLine(), out pPrice)) {
                            SysWrite("\tInvalid price.  Product not added.");
                            break;
                        }
                        Console.Write("\t\tCategory ID: ");
                        int cID;
                        if(!int.TryParse(Console.ReadLine(),out cID)) {
                            SysWrite("\n\tInvalid category ID.  Product not added.");
                            break;
                        }
                        SysWrite($"\tYou want to add the following product: {pName} at a price of {pPrice} with Category ID of {cID}.\n\tIs this correct? (y/n) ");
                        if(Console.ReadKey(true).KeyChar == 'y') {
                            prodRepo.CreateProduct(pName, pPrice, cID);
                            SysWrite("\n\tNew product was added successfully.");
                        }
                        else SysWrite("\n\tNew product was NOT added.");
                        break;

                    case '5': //update product record
                        SysWrite("\n\tEnter Product ID of product you want to update: ");
                        Console.Write("\t\t");
                        int pID;
                        if(!int.TryParse(Console.ReadLine(), out pID)) {
                            SysWrite("\n\tInvalid Product ID.  Product update failed.");
                            break;
                        }
                        IEnumerable<Products> oldP = prodRepo.GetSingleProduct(pID);
                        if(oldP == null) {
                            SysWrite("\n\tInvalid Product ID.  Product update failed.");
                            break;
                        }
                        SysWrite($"\n\tProduct {pID} found:");
                        foreach(var v in oldP) Console.WriteLine("\n\t" + v);

                        // TO_DO: allow user to leave some info alone, e.g. change only price:
                        SysWrite("\n\tTo update product, enter the following information:");
                        Console.Write("\n\t\tProduct name: ");
                        string name = Console.ReadLine();
                        Console.Write("\n\t\tProduct price: ");
                        double price = double.Parse(Console.ReadLine());
                        Console.Write("\n\t\tCategory ID: ");
                        int catid = int.Parse(Console.ReadLine());
                        if(!prodRepo.UpdateProduct(pID, name, price, catid)) {
                            SysWrite("\n\tUpdate failed.");
                            break;
                        }
                        SysWrite("\n\tUpdate successful.  Here is the new product record:");
                        IEnumerable<Products> newP=prodRepo.GetSingleProduct(pID);
                        if(newP == null) {
                            SysWrite("\n\tError displaying product.");
                            break;
                        }
                        else foreach(var v in newP) Console.WriteLine($"\n\tProduct ID: {v.ProductID} Name: {v.Name} Price: {v.Price} Category ID: {v.CategoryID}");
                        break;

                    case '6':   // delete product record
                        SysWrite("\n\tEnter Product ID of product you want to delete: ");
                        Console.Write("\t\t");
                        int delID;
                        if(!int.TryParse(Console.ReadLine(), out delID)) {
                            SysWrite("\n\tInvalid Product ID.  Product update failed.");
                            break;
                        }
                        IEnumerable<Products> pToDelete = prodRepo.GetSingleProduct(delID);
                        if(pToDelete == null) {
                            SysWrite("\n\tInvalid Product ID.  Product update failed.");
                            break;
                        }
                        SysWrite($"\n\tProduct found:");
                        foreach(var v in pToDelete) Console.WriteLine($"\n\tProduct ID: {v.ProductID} Name: {v.Name} Price: {v.Price} Category ID: {v.CategoryID}");
                        SysWrite("\n\tAre you sure you want to delete this product? (y/n)");
                        if(Console.ReadKey(true).KeyChar != 'y') {
                            SysWrite("\n\tProduct NOT deleted.");
                            break;
                        }
                        if(!prodRepo.DeleteProduct(delID)) {
                            SysWrite("\n\tError in delete request.  Product NOT deleted.");
                            break;
                        }
                        SysWrite($"\n\n\tPRODUCT {delID} DELETED!");
                        break;

                    case '7':  // end program
                        quit = true; break;
                    
                    default:  // user pressed invalid key
                        SysWrite("\n\n\tInvalid selection.");
                        break;
                }

                if(!quit) {
                    SysWrite("\n\n\tPress any key to return to main menu.");
                    Console.ReadKey(true);
                }
            }

            Console.Clear();
            SysWrite("\tPress any key to end...");
            Console.ReadKey(true);
        }


        public static void SysWrite(string s)
        {
            Console.ForegroundColor=ConsoleColor.Green;
            Console.WriteLine(s);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
