using Domain.Interfaces;
using Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IGenericRepository<Machinery> _machineryRepository;
    private readonly IGenericRepository<Order> _orderRepository;

    public OrderController(
        IGenericRepository<Machinery> machineryRepository,
        IGenericRepository<Order> orderRepository
    )
    {
        _machineryRepository = machineryRepository;
        _orderRepository = orderRepository;
    }
    
    [HttpGet("all-orders")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Machinery>))]
    [SwaggerOperation(
        Summary = "Retrieve all orders",
        Description = "Gets a list of all orders with their information.",
        OperationId = "GetAllOrders")]
    public IActionResult GetAllOrders()
    {
        var orders = _orderRepository.GetAll();
        return Ok(orders);
    }
}