using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Project_v2.Data
{
    public class StoreService
    {
        public class Address
        {
            public Guid id { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string country { get; set; }
            public string addressLine1 { get; set; }
            public string addressLine2 { get; set; }

        }

        public class Product
        {
            public Guid id { get; set; }
            public string name { get; set; }
            public double cost { get; set; }
            public int inventoryCount { get; set; }
            public int minAgeRestriction { get; set; }
        }

        public class Card
        {
            public Guid id { get; set; }
            public string cardNumber { get; set; }
            public string cvv { get; set; }
            public DateTime expires { get; set; }
            public string cardType { get; set; }
            public string cardHolder { get; set; }
        }
        
        public class Customer
        {
            public Guid id { get; set; }
            public string name { get; set; }
            public string email { get; set; }
            public DateTime DOB { get; set; }
            public string username { get; set; }
            public string passwordSalt { get; set; }
            public string encryptedPassword { get; set; }
            public Guid shippingAddress { get; set; }
            public Guid billingAddress { get; set; }
            public List<Guid> cards { get; set; }
        }

        public class Distributor
        {
            public Guid id { get; set; }
            public string name { get; set; }
            public Guid addressID { get; set; }
        }

        public class Order
        {
            public Guid orderId { get; set; }
            public Guid customerId { get; set; }
            public int trackingNumber { get; set; }
            public DateTime date { get; set; }
        }
        
        public class OrderLine
        {
            public Guid id { get; set; }
            public Guid orderId { get; set; }
            public int productQuanityt { get; set; }
            public Guid productId { get; set; }
            public double lineCost { get; set; }

        }

        public async Task<List<Product>> GetOrdersFor(Guid customerId)
        {
            List<Product> results = new List<Product>();
            var conn = new SqlConnection("Server=tcp:msu440.database.windows.net,1433;Initial Catalog=msu440;Persist Security Info=False;User ID=allen@psimpsonmotionencoding.onmicrosoft.com;Password=MSUDatabases440!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Authentication=\"Active Directory Password\";");
            
            await conn.OpenAsync();
            var cmd = conn.CreateCommand();
            
            cmd.CommandText =
                @"
                SELECT id,name,cost,inventory,minAgeRestrictionInYears
                FROM STORE.PRODUCT;
                ";

            var rdr = await cmd.ExecuteReaderAsync();
                
            while (await rdr.ReadAsync())
            {
                results.Add(
                    new Product {
                        id = rdr.GetGuid(0),
                        name = rdr.GetString(1), 
                        cost = rdr.GetDouble(2), 
                        inventoryCount = rdr.GetInt32(3), 
                        minAgeRestriction = rdr.GetInt32(4)
                    }
                 );
            }            
            
            return results;
        }

        public async Task<List<Distributor>> GetDistributors()
        {
            List<Distributor> results = new List<Distributor>();
            var conn = new SqlConnection("Server=tcp:msu440.database.windows.net,1433;Initial Catalog=msu440;Persist Security Info=False;User ID=allen@psimpsonmotionencoding.onmicrosoft.com;Password=MSUDatabases440!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Authentication=\"Active Directory Password\";");
            await conn.OpenAsync();
            var cmd = conn.CreateCommand();

            cmd.CommandText =
                @"
                SELECT id, name, addressId
                FROM STORE.DISTRIBUTER
                ORDER BY name;
                ";

            var rdr = await cmd.ExecuteReaderAsync();

            while (await rdr.ReadAsync())
            {
                results.Add(
                    new Distributor
                    {
                        id = rdr.GetGuid(0),
                        name = rdr.GetString(1),
                        addressID = rdr.GetGuid(2)
                    }
                 );
            }

            return results;
        }
    }
}
