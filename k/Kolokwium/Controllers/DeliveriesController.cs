using Kolokwium.Models;
using Kolokwium.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kolokwium.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveriesController : ControllerBase
    {
        private readonly IDbService _dbService;

        public DeliveriesController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDeliveriesInfo(int id)
        {
            var result = await _dbService.getDeliveriesInfo(id);
            if (result == null)
            {
                return NotFound($"Delivery with id {id} not found");
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddDeliveries([FromBody] NewDeliveryDTO newDelivery)
        {
            var deliveryExist = await _dbService.DeliveryExists(newDelivery.deliveryId.Value);
            if (deliveryExist)
            {
                return BadRequest("Delivery already exists");
            }

            var clientExist = await _dbService.ClinetExists(newDelivery.customerId.Value);
            if (!clientExist)
            {
                return NotFound("Client does not exist");
            }

            var driverExist = await _dbService.DriverExists(newDelivery.licenceNumber);
            if (!driverExist)
            {
                return NotFound("Driver does not exist");
            }

            var productList = newDelivery.products;
            foreach (var product in productList)
            {

                var productNameExist = await _dbService.ProductExists(product.name);
                if (!productNameExist)
                {
                    return NotFound($"Product '{product.name}' not found");
                }
            }

            var newDeliery = await _dbService.CreateDelivery(newDelivery);
            if (!newDeliery)
            {
                return StatusCode(500, "Failed to create appointment");
            }
            return Created();
        }
    }
}
