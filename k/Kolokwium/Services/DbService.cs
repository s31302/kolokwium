using Kolokwium.Models;
using Microsoft.Data.SqlClient;

namespace Kolokwium.Services;

public class DbService : IDbService
{
    //zmien nazwe bazy danych
    private readonly string _connectionString =
        "Server=localhost\\SQLEXPRESS;Database=kolokwium;Trusted_Connection=True;TrustServerCertificate=True;";

    public async Task<DeliveriesDTO?> getDeliveriesInfo(int id)
    {
        DeliveriesDTO? deliveriesInfo = null;

        string command =
            "SELECT Delivery.date, Customer.first_name, Customer.last_name, Customer.date_of_birth, Driver.first_name, Driver.last_name, Driver.licence_number, Product.name, Product.price, Product_Delivery.amount FROM Delivery join Customer on Delivery.customer_id=Customer.customer_id JOIN Driver ON Delivery.driver_id=Driver.driver_id JOIN Product_Delivery ON Delivery.delivery_id = Product_Delivery.delivery_id JOIN Product ON Product.product_id = Product_Delivery.product_id WHERE Delivery.delivery_id = @Id";
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@id", id);
            await conn.OpenAsync();
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    if (deliveriesInfo == null)
                    {
                        deliveriesInfo = new DeliveriesDTO
                        {
                            date = reader.GetDateTime(0),
                            customer = new CustomerDTO()
                            {
                                firstName = reader.GetString(1),
                                lastName = reader.GetString(2),
                                dateOfBirth = reader.GetDateTime(3),
                            },
                            driver = new DriverDTO()
                            {
                                firstName = reader.GetString(4),
                                lastName = reader.GetString(5),
                                licenceNumber = reader.GetString(6),
                            },
                            products = new List<ProductsDTO>()
                        };
                    }

                    deliveriesInfo.products.Add(new ProductsDTO
                    {
                        name = reader.GetString(7),
                        price = Convert.ToDouble(reader.GetDecimal(8)),
                        amount = reader.GetInt32(9)
                    });

                }
            }
        }

        return deliveriesInfo;
    }

    public async Task<bool> DeliveryExists(int deliveryId)
    {
        string command = "SELECT * FROM Delivery WHERE delivery_id = @id";
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@id", deliveryId);
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                return await reader.ReadAsync();
            }
        }
    }


    public async Task<bool> ClinetExists(int customerId)
    {
        string command = "SELECT * FROM Customer WHERE customer_id = @id";
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@id", customerId);
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                return await reader.ReadAsync();
            }
        }
    }

    public async Task<bool> DriverExists(string licenceNumber)
    {
        string command = "SELECT * FROM Driver WHERE driver_id = @id";
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@id", licenceNumber);
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                return await reader.ReadAsync();
            }
        }
    }

    public async Task<bool> ProductExists(string name)
    {
        string command = "SELECT * FROM Product WHERE name = @id";
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@id", name);
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                return await reader.ReadAsync();
            }
        }
    }

    public async Task<bool> CreateDelivery(NewDeliveryDTO newDelivery)
    {
        int driverId;
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            await conn.OpenAsync();
            string selectDriverId = "SELECT driver_id FROM Driver WHERE licence_number = @licenceNumber";
            using (SqlCommand cmd = new SqlCommand(selectDriverId, conn))
            {
                cmd.Parameters.AddWithValue("@licenceNumber", newDelivery.licenceNumber);
                var driver = await cmd.ExecuteScalarAsync();
                if (driver == null)
                    throw new Exception("Doctor not found");
                driverId = Convert.ToInt32(driver);
            }

            string command =
                "INSERT INTO Delivery (delivery_id, customer_id, driver_id, date) VALUES (@deliveryId, @customerId, @driverId, @date)";
            using (SqlCommand insertCmd = new SqlCommand(command, conn))
            {
                insertCmd.Parameters.AddWithValue("@deliveryId", newDelivery.deliveryId);
                insertCmd.Parameters.AddWithValue("@customerId", newDelivery.customerId);
                insertCmd.Parameters.AddWithValue("@driverId", driverId);
                insertCmd.Parameters.AddWithValue("@date", DateTime.Now);

                await insertCmd.ExecuteNonQueryAsync();
            }
            
        }
        return true;
    }
}
