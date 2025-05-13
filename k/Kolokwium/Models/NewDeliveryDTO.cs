using Microsoft.Build.Framework;

namespace Kolokwium.Models;

public class NewDeliveryDTO
{
    [Required] public int? deliveryId { get; set; }
    [Required] public int? customerId { get; set; }
    [Required] public string licenceNumber { get; set; }
    [Required] public List<ProductsDTO> products { get; set; }
    
}

public class ProductDTO
{
    [Required] public string name { get; set; }
    [Required] public int? amount { get; set; }
}