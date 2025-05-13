using Kolokwium.Models;

namespace Kolokwium.Services;

public interface IDbService
{
    Task<DeliveriesDTO?> getDeliveriesInfo(int Id);
    
    Task<bool> DeliveryExists(int deliveryId);
    
    Task<bool> ClinetExists(int customerId);
    
    Task<bool> DriverExists(string licenceNumber);
    
    Task<bool> ProductExists(string name);
    
    Task<bool> CreateDelivery(NewDeliveryDTO newDelivery);

}