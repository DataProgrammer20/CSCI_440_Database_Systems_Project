using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Project_v2.Data
{
    public class StoreService
    {
        private string connStr = "Server=tcp:msu440.database.windows.net,1433;Initial Catalog=msu440;Persist Security Info=False;User ID=allen@psimpsonmotionencoding.onmicrosoft.com;Password=MSUDatabases440!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Authentication=\"Active Directory Password\";";
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
            public Guid distributorID { get; set; }
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

        public async Task<List<Product>> GetOrdersFor(Guid customerId) {
            List<Product> results = new List<Product>();
            var conn = new SqlConnection(connStr);
            await conn.OpenAsync();
            var cmd = conn.CreateCommand();

            cmd.CommandText =
                @"
                SELECT id,name,cost,inventory,minAgeRestrictionInYears,distributerId
                FROM STORE.PRODUCT;
                ";

            var rdr = await cmd.ExecuteReaderAsync();

            while (await rdr.ReadAsync()) {
                results.Add(
                    new Product {
                        id = rdr.GetGuid(0),
                        name = rdr.GetString(1),
                        cost = rdr.GetDouble(2),
                        inventoryCount = rdr.GetInt32(3),
                        minAgeRestriction = rdr.GetInt32(4),
                        distributorID = rdr.GetGuid(5)
                    }
                 );
            }

            return results;
        }

        public async Task<List<Distributor>> GetDistributors() {
            List<Distributor> results = new List<Distributor>();
            var conn = new SqlConnection(connStr);
            await conn.OpenAsync();
            var cmd = conn.CreateCommand();

            cmd.CommandText =
                @"
                SELECT id, name, addressId
                FROM STORE.DISTRIBUTER
                ORDER BY name;
                ";

            var rdr = await cmd.ExecuteReaderAsync();

            while (await rdr.ReadAsync()) {
                results.Add(
                    new Distributor {
                        id = rdr.GetGuid(0),
                        name = rdr.GetString(1),
                        addressID = rdr.GetGuid(2)
                    }
                 );
            }

            return results;
        }

        public async Task<List<Product>> GetDistributorsProducts(Guid distID) {
            List<Product> results = new List<Product>();

            var conn = new SqlConnection(connStr);
            await conn.OpenAsync();
            var cmd = conn.CreateCommand();

            cmd.CommandText =
                @"
                SELECT id,name,cost,inventory,minAgeRestrictionInYears,distributerId
                FROM STORE.PRODUCT
                WHERE distributerId = @distID
                ORDER BY name;
                ";
            cmd.Parameters.AddWithValue("distID", distID);
            var rdr = await cmd.ExecuteReaderAsync();

            while (await rdr.ReadAsync()) {
                results.Add(
                    new Product {
                        id = rdr.GetGuid(0),
                        name = rdr.GetString(1),
                        cost = rdr.GetDouble(2),
                        inventoryCount = rdr.GetInt32(3),
                        minAgeRestriction = rdr.GetInt32(4),
                        distributorID = rdr.GetGuid(5)
                    }
                 );
            }

            return results;
        }

        public async void SaveNewProduct(Product prod) {
            List<Product> results = new List<Product>();
            var conn = new SqlConnection(connStr);
            await conn.OpenAsync();
            var cmd = conn.CreateCommand();

            cmd.CommandText =
                @"
                INSERT INTO STORE.PRODUCT VALUES 
                (@id,@name,@cost,@inv,@minAge,@distId);
                ";
            cmd.Parameters.AddWithValue("id", prod.id);
            cmd.Parameters.AddWithValue("name", prod.name);
            cmd.Parameters.AddWithValue("cost", prod.cost);
            cmd.Parameters.AddWithValue("inv", prod.inventoryCount);
            cmd.Parameters.AddWithValue("minAge", prod.minAgeRestriction);
            cmd.Parameters.AddWithValue("distId", prod.distributorID);

            cmd.ExecuteNonQuery();
        }
        public async Task<List<Tuple<Product,int>>> getSalesByDistributer(Guid distID) {
            List<Tuple<Product,int>> results = new List<Tuple<Product,int>>();

            var conn = new SqlConnection(connStr);
            await conn.OpenAsync();
            var cmd = conn.CreateCommand();

            cmd.CommandText =
                @"
                SELECT p.id,p.name,p.cost,p.inventory,p.minAgeRestrictionInYears,p.distributerId,SUM(ol.productQuanitity) as cnt 
                FROM STORE.ORDERLINE ol 
	                INNER JOIN STORE.PRODUCT p ON ol.productId = p.id 
                WHERE p.distributerId = @distID
                GROUP BY p.id,p.name,p.cost,p.inventory,p.minAgeRestrictionInYears,p.distributerId;
                ";
            cmd.Parameters.AddWithValue("distID", distID);
            var rdr = await cmd.ExecuteReaderAsync();

            while (await rdr.ReadAsync()) {
                var p = new Product {
                        id = rdr.GetGuid(0),
                        name = rdr.GetString(1),
                        cost = rdr.GetDouble(2),
                        inventoryCount = rdr.GetInt32(3),
                        minAgeRestriction = rdr.GetInt32(4),
                        distributorID = rdr.GetGuid(5)
                };
                results.Add(new Tuple<Product, int>(p,rdr.GetInt32(6)));
            }

            return results;
        }

        public async void DeleteProduct(Product p) {
            var conn = new SqlConnection(connStr);
            await conn.OpenAsync();
            var cmd = conn.CreateCommand();

            cmd.CommandText =
                @"
                DELETE FROM STORE.PRODUCT 
                WHERE id = @productID
                ";
            cmd.Parameters.AddWithValue("productID", p.id);
            
            cmd.ExecuteNonQuery();
        }

        public async Task<List<Tuple<Product, int>>> GetCart (Customer c) {
            List<Tuple<Product, int>> results = new List<Tuple<Product, int>>();

            var conn = new SqlConnection(connStr);
            await conn.OpenAsync();
            var cmd = conn.CreateCommand();

            cmd.CommandText =
                @"
                SELECT p.id,p.name,p.cost,p.inventory,p.minAgeRestrictionInYears,p.distributerId,ol.productQuanitity
                FROM STORE.PRODUCT p INNER JOIN STORE.ORDERLINE ol ON p.id = ol.productId INNER JOIN cartOrders o ON o.orderID = ol.orderID
                WHERE o.customerID = @custID 	
                ";
            cmd.Parameters.AddWithValue("custID", c.id);
            var rdr = await cmd.ExecuteReaderAsync();

            while (await rdr.ReadAsync()) {
                var p = new Product {
                    id = rdr.GetGuid(0),
                    name = rdr.GetString(1),
                    cost = rdr.GetDouble(2),
                    inventoryCount = rdr.GetInt32(3),
                    minAgeRestriction = rdr.GetInt32(4),
                    distributorID = rdr.GetGuid(5)
                };
                results.Add(new Tuple<Product, int>(p, rdr.GetInt32(6)));
            }
            return results;
        }

        public async void addToCart(Customer c, Product p) {
            var conn = new SqlConnection(connStr);
            await conn.OpenAsync();
            var cmd = conn.CreateCommand();

            cmd.CommandText =
                @"
                DECLARE @customerCart uniqueidentifier;

                SELECT TOP 1 @customerCart = o.orderID 
                FROM cartOrders o 
                WHERE o.customerID = @custID
                BEGIN TRANSACTION;
                    BEGIN TRY
                        IF NOT Exists(
                            SELECT ol.id
                            FROM cartOrders o, STORE.ORDERLINE ol
                            WHERE o.orderID = ol.orderID AND ol.productId = @prodID AND o.customerID = @custID
                            )
                            BEGIN
                                INSERT INTO STORE.ORDERLINE VALUES(NEWID(), @customerCart,1,@prodID,(SELECT cost FROM STORE.PRODUCT WHERE id = @prodID));
                            END
                        ELSE
                            BEGIN
                                UPDATE STORE.ORDERLINE
                                SET productQuanitity = productQuanitity + 1, 
					                lineCost = productQuanitity * (SELECT cost FROM STORE.PRODUCT WHERE id = @prodID)
				                WHERE
                                    orderID = @customerCart AND productId = @prodID;
                            END
                        UPDATE STORE.PRODUCT
                        SET inventory = inventory - 1
                        WHERE
                            id = @prodID;
                        COMMIT TRANSACTION;
                    END TRY
                    BEGIN CATCH
                        ROLLBACK TRANSACTION;
                    END CATCH
                ";
            cmd.Parameters.AddWithValue("custID", c.id);
            cmd.Parameters.AddWithValue("prodID", p.id);

            cmd.ExecuteNonQuery();
        }

        public async void RemoveProductFromCart(Product p, Guid custID)
        {
            var conn = new SqlConnection(connStr);
            await conn.OpenAsync();
            var cmd = conn.CreateCommand();

            cmd.CommandText =
                @"
                --SELECT orderline given a productID and custID, then delete it.
                ";
            cmd.Parameters.AddWithValue("productID", p.id);
            cmd.Parameters.AddWithValue("custID", custID);

            cmd.ExecuteNonQuery();
        }
    }
