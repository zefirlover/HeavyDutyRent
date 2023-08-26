using Domain.DTOs;
using Domain.Interfaces;
using Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MachineryController : ControllerBase
{
    private readonly IGenericRepository<Machinery> _machineryRepository;
    private readonly IGenericRepository<Seller> _sellerRepository;

    public MachineryController(
        IGenericRepository<Machinery> machineryRepository,
        IGenericRepository<Seller> sellerRepository
    )
    {
        _machineryRepository = machineryRepository;
        _sellerRepository = sellerRepository;
    }
    
    [HttpGet("all-machineries")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Machinery>))]
    [SwaggerOperation(
        Summary = "Retrieve all machineries",
        Description = "Gets a list of all machineries with their information.",
        OperationId = "GetAllMachineries")]
    public IActionResult GetAllMachineries()
    {
        var machineries = _machineryRepository.GetAll();
        return Ok(machineries);
    }
    
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Retrieve a machinery by ID", Description = "Gets a specific machinery by their ID.")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Machinery))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetMachineryById(int id)
    {
        var machinery = _machineryRepository.GetById(id);

        if (machinery == null)
        {
            return NotFound();
        }

        return Ok(machinery);
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Machinery))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "Create a new Machinery",
        Description = "Creates a new machinery with the provided data.",
        OperationId = "CreateMachinery")]
    public IActionResult CreateMachinery([FromBody] CreateMachineryDto createMachineryDto)
    {
        try
        {
            if (createMachineryDto == null) return BadRequest();

            var seller = _sellerRepository.GetById(createMachineryDto.SellerId);
            if (seller == null)
            {
                return BadRequest($"Seller with Id = {createMachineryDto.SellerId} not found");
            }

            var machinery = new Machinery
            {
                Name = createMachineryDto.Name,
                AddressLine = createMachineryDto.AddressLine,
                Price = createMachineryDto.Price,
                SellerId = createMachineryDto.SellerId
            };

            _machineryRepository.Add(machinery); // Add the buyer to the repository
            _machineryRepository.SaveChanges(); // Save changes to the database using the repository

            return CreatedAtAction(nameof(GetMachineryById), new { id = machinery.Id }, machinery);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Error creating new machinery record");
        }
    }
    
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Update a machinery by ID",
        Description = "Updates an existing machinery with the provided data.",
        OperationId = "UpdateMachinery")]
    public IActionResult UpdateMachinery(int id, [FromBody] UpdateMachineryDto updateMachineryDto)
    {
        try
        {
            if (updateMachineryDto == null) return BadRequest();

            var existingMachinery = _machineryRepository.GetById(id);

            if (existingMachinery == null)
            {
                return NotFound();
            }

            existingMachinery.Name = updateMachineryDto.Name;
            existingMachinery.AddressLine = updateMachineryDto.AddressLine;
            existingMachinery.Price = updateMachineryDto.Price;

            _machineryRepository.SaveChanges();

            return Ok(existingMachinery);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Error updating machinery record");
        }
    }
    
    [HttpDelete("{id:int}")]
    [SwaggerOperation(Summary = "Delete a machinery by ID", Description = "Deletes a specific machinery by their ID.")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Machinery))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteMachineryById(int id)
    {
        try
        {
            var machineryToDelete = _machineryRepository.GetById(id);

            if (machineryToDelete == null)
            {
                return NotFound($"Machinery with Id = {id} not found");
            }

            _machineryRepository.Remove(machineryToDelete);
            _machineryRepository.SaveChanges();

            return NoContent();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting data");
        }
    }
    
    [HttpDelete("delete-range")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "Delete a range of machineries",
        Description = "Deletes a range of machineries based on the provided IDs.",
        OperationId = "DeleteRange")]
    public IActionResult DeleteRange([FromBody] List<int> machineryIds)
    {
        if (machineryIds == null || !machineryIds.Any())
        {
            return BadRequest("Invalid machineries IDs.");
        }

        var machineriesToDelete = _machineryRepository.FindBy(m => machineryIds.Contains(m.Id)).ToList();
        _machineryRepository.RemoveRange(machineriesToDelete);
        _machineryRepository.SaveChanges();

        return NoContent();
    }
}