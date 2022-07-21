using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace BestBuyBestPractices
{
    internal class DapperDepartmentRepository:IDepartmentRepository
    {
        private readonly IDbConnection _dbConnection;

        public DapperDepartmentRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public IEnumerable<Department> GetAllDepartments()
        {
            return _dbConnection.Query<Department>("SELECT * FROM Departments;");
        }

        public void InsertDepartment(string newDepartmentName)
        {
            _dbConnection.Execute("INSERT INTO Departments (Name) VALUES (@newDepart);",
                new { newDepart = newDepartmentName });
        }

    }
}
