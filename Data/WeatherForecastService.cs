using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Project_v2.Data
{
    public class WeatherForecastService
    {
        public class Product
        {
            public string name { get; set; }
            public double cost { get; set; }
            public int inventoryCount { get; set; }
            public int minAgeRestriction { get; set; }
        }

        public async Task<List<Product>> GetOrdersFor(Guid customerId)
        {
            List<Product> results = new List<Product>();
            var conn = new SqlConnection("Server=tcp:msu440.database.windows.net,1433;Initial Catalog=msu440;Persist Security Info=False;User ID=allen@psimpsonmotionencoding.onmicrosoft.com;Password=MSUDatabases440!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Authentication=\"Active Directory Password\";");
            
            await conn.OpenAsync();
            var cmd = conn.CreateCommand();
            
            cmd.CommandText =
                @"
                SELECT name,cost,inventory,minAgeRestrictionInYears
                FROM STORE.PRODUCT;
                ";

            var rdr = await cmd.ExecuteReaderAsync();
                
            while (await rdr.ReadAsync())
            {
                results.Add(
                    new Product {
                        name = rdr.GetString(0), 
                        cost = rdr.GetDouble(1), 
                        inventoryCount = rdr.GetInt32(2), 
                        minAgeRestriction = rdr.GetInt32(3)
                    }
                 );
            }            
            
            return results;
        }
    }
}
