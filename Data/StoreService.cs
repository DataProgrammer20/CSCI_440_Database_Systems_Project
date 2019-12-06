using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Project_v2.Data
{
    public class StoreService
    {
        private string ConnectionString = "Server=tcp:msu440.database.windows.net,1433;Initial Catalog=msu440;Persist Security Info=False;User ID=allen@psimpsonmotionencoding.onmicrosoft.com;Password=MSUDatabases440!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Authentication=\"Active Directory Password\";";
        public class Address
        {
            public Guid Id { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Country { get; set; }
            public string AddressLine1 { get; set; }
            public string AddressLine2 { get; set; }
        }

        public class Product
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public double Cost { get; set; }
            public int InventoryCount { get; set; }
            public int MinAgeRestriction { get; set; }
            public Guid DistributorId { get; set; }
        }

        public class Card
        {
            public Guid Id { get; set; }
            public string CardNumber { get; set; }
            public string Cvv { get; set; }
            public DateTime Expires { get; set; }
            public string CardType { get; set; }
            public string CardHolder { get; set; }
        }

        public class Customer
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public DateTime Dob { get; set; }
            public string Username { get; set; }
            public string PasswordSalt { get; set; }
            public string EncryptedPassword { get; set; }
            public Guid ShippingAddress { get; set; }
            public Guid BillingAddress { get; set; }
            public List<Guid> Cards { get; set; }
        }

        public class Distributor
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public Guid AddressId { get; set; }
        }

        public class Order
        {
            public Guid OrderId { get; set; }
            public Guid CustomerId { get; set; }
            public int TrackingNumber { get; set; }
            public DateTime Date { get; set; }
        }

        public class OrderLine
        {
            public Guid Id { get; set; }
            public Guid OrderId { get; set; }
            public int ProductQuantity { get; set; }
            public Guid ProductId { get; set; }
            public double LineCost { get; set; }
        }

        public async Task<List<Product>> GetOrdersFor(Guid customerId)
        {
            List<Product> results = new List<Product>();
            var conn = new SqlConnection(ConnectionString);
            await conn.OpenAsync();
            var cmd = conn.CreateCommand();

            cmd.CommandText =
                @"
                SELECT id,name,cost,inventory,minAgeRestrictionInYears,distributerId
                FROM STORE.PRODUCT;
                ";

            var rdr = await cmd.ExecuteReaderAsync();

            while (await rdr.ReadAsync())
            {
                results.Add(
                    new Product
                    {
                        Id = rdr.GetGuid(0),
                        Name = rdr.GetString(1),
                        Cost = rdr.GetDouble(2),
                        InventoryCount = rdr.GetInt32(3),
                        MinAgeRestriction = rdr.GetInt32(4),
                        DistributorId = rdr.GetGuid(5)
                    }
                 );
            }

            return results;
        }

        public async Task<List<Customer>> GetCustomers()
        {
            var result = new List<Customer>();
            var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();
            var cmd = connection.CreateCommand();

            cmd.CommandText = @"
                                SELECT id, name
                                FROM STORE.CUSTOMER
                                ORDER BY name;
                               ";

            var rdr = await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
            {
                result.Add(
                    new Customer
                    {
                        Id = rdr.GetGuid(0),
                        Name = rdr.GetString(1)
                    });
            }

            return result;
        }

        public async Task<List<Distributor>> GetDistributors()
        {
            List<Distributor> results = new List<Distributor>();
            var conn = new SqlConnection(ConnectionString);
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
                        Id = rdr.GetGuid(0),
                        Name = rdr.GetString(1),
                        AddressId = rdr.GetGuid(2)
                    }
                 );
            }
            return results;
        }


        // Work in progress...
        // =======================================================================
        public async Task<List<Product>> GetProducts()
        {
            var results = new List<Product>();
            var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();
            var cmd = connection.CreateCommand();

            cmd.CommandText = @"
                SELECT id,name,cost,inventory,minAgeRestrictionInYears,distributerId
                FROM STORE.PRODUCT p
                WHERE p.inventory > 0
                ORDER BY p.cost;
                ";

            var rdr = await cmd.ExecuteReaderAsync();

            while (await rdr.ReadAsync())
            {
                results.Add(
                    new Product
                    {
                        Id = rdr.GetGuid(0),
                        Name = rdr.GetString(1),
                        Cost = rdr.GetDouble(2),
                        InventoryCount = rdr.GetInt32(3),
                        MinAgeRestriction = rdr.GetInt32(4),
                        DistributorId = rdr.GetGuid(5)
                    }
                 );
            }
            return results;
        }
        // =======================================================================


        public async Task<List<Product>> GetDistributorsProducts(Guid distId)
        {
            List<Product> results = new List<Product>();

            var conn = new SqlConnection(ConnectionString);
            await conn.OpenAsync();
            var cmd = conn.CreateCommand();

            cmd.CommandText =
                @"
                SELECT id,name,cost,inventory,minAgeRestrictionInYears,distributerId
                FROM STORE.PRODUCT
                WHERE distributerId = @distID
                ORDER BY name;
                ";
            cmd.Parameters.AddWithValue("distID", distId);
            var rdr = await cmd.ExecuteReaderAsync();

            while (await rdr.ReadAsync())
            {
                results.Add(
                    new Product
                    {
                        Id = rdr.GetGuid(0),
                        Name = rdr.GetString(1),
                        Cost = rdr.GetDouble(2),
                        InventoryCount = rdr.GetInt32(3),
                        MinAgeRestriction = rdr.GetInt32(4),
                        DistributorId = rdr.GetGuid(5)
                    }
                 );
            }
            return results;
        }

        public async void SaveNewProduct(Product prod)
        {
            List<Product> results = new List<Product>();
            var conn = new SqlConnection(ConnectionString);
            await conn.OpenAsync();
            var cmd = conn.CreateCommand();

            cmd.CommandText =
                @"
                INSERT INTO STORE.PRODUCT VALUES 
                (@id,@name,@cost,@inv,@minAge,@distId);
                ";
            cmd.Parameters.AddWithValue("id", prod.Id);
            cmd.Parameters.AddWithValue("name", prod.Name);
            cmd.Parameters.AddWithValue("cost", prod.Cost);
            cmd.Parameters.AddWithValue("inv", prod.InventoryCount);
            cmd.Parameters.AddWithValue("minAge", prod.MinAgeRestriction);
            cmd.Parameters.AddWithValue("distId", prod.DistributorId);

            cmd.ExecuteNonQuery();
        }
        public async Task<List<Tuple<Product, int>>> GetSalesByDistributor(Guid distId)
        {
            List<Tuple<Product, int>> results = new List<Tuple<Product, int>>();

            var conn = new SqlConnection(ConnectionString);
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
            cmd.Parameters.AddWithValue("distID", distId);
            var rdr = await cmd.ExecuteReaderAsync();

            while (await rdr.ReadAsync())
            {
                var p = new Product
                {
                    Id = rdr.GetGuid(0),
                    Name = rdr.GetString(1),
                    Cost = rdr.GetDouble(2),
                    InventoryCount = rdr.GetInt32(3),
                    MinAgeRestriction = rdr.GetInt32(4),
                    DistributorId = rdr.GetGuid(5)
                };
                results.Add(new Tuple<Product, int>(p, rdr.GetInt32(6)));
            }
            return results;
        }

        public async void DeleteProduct(Product p)
        {
            var conn = new SqlConnection(ConnectionString);
            await conn.OpenAsync();
            var cmd = conn.CreateCommand();

            cmd.CommandText =
                @"
                DELETE FROM STORE.PRODUCT 
                WHERE id = @productID
                ";
            cmd.Parameters.AddWithValue("productID", p.Id);

            cmd.ExecuteNonQuery();
        }

        public async Task<List<Tuple<Product, int>>> GetCart(Customer c)
        {
            List<Tuple<Product, int>> results = new List<Tuple<Product, int>>();

            var conn = new SqlConnection(ConnectionString);
            await conn.OpenAsync();
            var cmd = conn.CreateCommand();

            cmd.CommandText =
                @"
                SELECT p.id,p.name,p.cost,p.inventory,p.minAgeRestrictionInYears,p.distributerId,ol.productQuanitity
                FROM STORE.PRODUCT p INNER JOIN STORE.ORDERLINE ol ON p.id = ol.productId INNER JOIN cartOrders o ON o.orderID = ol.orderID
                WHERE o.customerID = @custID 	
                ";
            cmd.Parameters.AddWithValue("custID", c.Id);
            var rdr = await cmd.ExecuteReaderAsync();

            while (await rdr.ReadAsync())
            {
                var p = new Product
                {
                    Id = rdr.GetGuid(0),
                    Name = rdr.GetString(1),
                    Cost = rdr.GetDouble(2),
                    InventoryCount = rdr.GetInt32(3),
                    MinAgeRestriction = rdr.GetInt32(4),
                    DistributorId = rdr.GetGuid(5)
                };
                results.Add(new Tuple<Product, int>(p, rdr.GetInt32(6)));
            }
            return results;
        }

        public async void AddToCart(Customer c, Product p)
        {
            var conn = new SqlConnection(ConnectionString);
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
            cmd.Parameters.AddWithValue("custID", c.Id);
            cmd.Parameters.AddWithValue("prodID", p.Id);

            cmd.ExecuteNonQuery();
        }

        public async void RemoveProductFromCart(Product product, Customer customer)
        {
            var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();
            var cmd = connection.CreateCommand();

            cmd.CommandText = @"
                DELETE
                FROM STORE.ORDERLINE
                WHERE id IN (
	                SELECT ol.id
	                FROM STORE.ORDERLINE ol, STORE.[ORDER] o 
	                WHERE ol.orderId = o.orderId AND ol.productId = @productID AND o.customerId = @custId
	                );
                ";

            cmd.Parameters.AddWithValue("productID", product.Id);
            cmd.Parameters.AddWithValue("custID", customer.Id);
            cmd.ExecuteNonQuery();
        }

        public async void PurchaseCart(Customer customer) {
            var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();
            var cmd = connection.CreateCommand();

            cmd.CommandText = @"
            UPDATE STORE.[ORDER] 
            SET orderStatus = 1
            WHERE customerID = @custID AND orderStatus = 0;

            INSERT INTO STORE.[ORDER] (orderID, customerID,orderStatus)
	            SELECT NEWID(), c.id, 0
	            FROM CustomersWithoutCarts c;
            ";

            cmd.Parameters.AddWithValue("custID", customer.Id);
            cmd.ExecuteNonQuery();
        }
    }
}
