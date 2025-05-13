namespace Kolokwium.Models;

public class DeliveriesDTO
{
    public DateTime date { get; set; }
    public CustomerDTO customer { get; set; }
    public DriverDTO driver { get; set; }
    public List<ProductsDTO> products { get; set; }
    
}

public class CustomerDTO
{
    public string firstName { get; set; }
    public string lastName { get; set; }
    public DateTime dateOfBirth { get; set; }
}

public class DriverDTO
{
    public string firstName { get; set; }
    public string lastName { get; set; }
    public string licenceNumber { get; set; }
}

public class ProductsDTO
{
    public string name { get; set; }
    public double price { get; set; }
    public int amount { get; set; }
}