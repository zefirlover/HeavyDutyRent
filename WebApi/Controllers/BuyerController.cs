using Domain.Interfaces;
using Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BuyerController : ControllerBase
{
    private readonly IGenericRepository<Buyer> _buyerRepository;

    public BuyerController(IGenericRepository<Buyer> buyerRepository)
    {
        _buyerRepository = buyerRepository;
    }
    
    [HttpGet("all-buyers")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Buyer>))]
    [SwaggerOperation(
        Summary = "Retrieve all buyers",
        Description = "Gets a list of all buyers with their information.",
        OperationId = "GetAllBuyers")]
    public IActionResult GetAllBuyers()
    {
        var buyers = _buyerRepository.GetAll();
        return Ok(buyers);
    }
    
    /// <summary>
    /// Deletes a specific TodoItem.
    /// </summary>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Retrieve a buyer by ID", Description = "Gets a specific buyer by their ID.")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Buyer))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetBuyerById(int id)
    {
        var buyer = _buyerRepository.GetById(id);

        if (buyer == null)
        {
            return NotFound();
        }

        return Ok(buyer);
    }
}