using Domain.DTOs;
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Order>))]
    [SwaggerOperation(
        Summary = "Retrieve all orders",
        Description = "Gets a list of all orders with their information.",
        OperationId = "GetAllOrders")]
    public IActionResult GetAllOrders()
    {
        var orders = _orderRepository.Include(o => o.Machineries).ToList();
        var orderDto = orders.Select(order => new GetOrderDto
        {
            Id = order.Id,
            Status = order.Status,
            CreatedAt = order.CreatedAt,
            Machineries = order.Machineries.Select(
                m => new GetMachineryDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    AddressLine = m.AddressLine,
                    Price = m.Price,
                    SellerId = m.SellerId
                }
            ).ToList()
        }).ToList();
        return Ok(orderDto);
    }
    
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Retrieve an order by ID", Description = "Gets a specific order by their ID.")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Order))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetOrderById(int id)
    {
        var order = _orderRepository
            .Include(o => o.Machineries)
            .SingleOrDefault(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        var orderDto = new GetOrderDto
        {
            Id = order.Id,
            Status = order.Status,
            CreatedAt = order.CreatedAt,
            Machineries = order.Machineries.Select(
                m => new GetMachineryDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    AddressLine = m.AddressLine,
                    Price = m.Price,
                    SellerId = m.SellerId
                }
            ).ToList()
        };
        
        return Ok(orderDto);
    }
    
    [HttpPost("create-order")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Order))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "Create a new order with machinery",
        Description = "Creates a new order with the provided data and associates it with machinery.",
        OperationId = "CreateOrder")]
    public IActionResult CreateOrder([FromBody] CreateOrderDto createOrderDto)
    {
        try
        {
            if (createOrderDto == null || createOrderDto.MachineriesIds == null)
            {
                return BadRequest("Invalid order data.");
            }

            var order = new Order
            {
                Status = createOrderDto.Status,
                BuyerId = createOrderDto.BuyerId,
                CreatedAt = DateTimeOffset.UtcNow
            };

            var machineryList = _machineryRepository
                .GetAll()
                .Where(m => createOrderDto.MachineriesIds.Contains(m.Id))
                .ToList();

            order.Machineries = machineryList;

            _orderRepository.Add(order);
            _orderRepository.SaveChanges();

            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(StatusCodes.Status500InternalServerError, "Error creating a new order.");
        }
    }
    
    [HttpDelete("{id:int}")]
    [SwaggerOperation(Summary = "Delete an order by ID", Description = "Deletes a specific order by their ID.")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Order))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteOrderById(int id)
    {
        try
        {
            var orderToDelete = _orderRepository.GetById(id);

            if (orderToDelete == null)
            {
                return NotFound($"Order with Id = {id} not found");
            }

            _orderRepository.Remove(orderToDelete);
            _orderRepository.SaveChanges();

            return NoContent();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting data");
        }
    }
}