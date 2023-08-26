using Domain.DTOs;
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

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Buyer))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "Create a new buyer",
        Description = "Creates a new buyer with the provided data.",
        OperationId = "CreateBuyer")]
    public IActionResult CreateBuyer([FromBody] BuyerDto buyerDto)
    {
        try
        {
            if (buyerDto == null) return BadRequest();
            
            var existingBuyerByUsername = _buyerRepository.FindBy(buyer => buyer.UserName == buyerDto.UserName).FirstOrDefault();
            if (existingBuyerByUsername != null)
            {
                return BadRequest("Username is already in use.");
            }
            
            var existingBuyerByEmail = _buyerRepository.FindBy(buyer => buyer.Email == buyerDto.Email).FirstOrDefault();
            if (existingBuyerByEmail != null)
            {
                return BadRequest("Email is already in use.");
            }
            
            var existingBuyerByPhoneNumber = _buyerRepository.FindBy(buyer => buyer.PhoneNumber == buyerDto.PhoneNumber).FirstOrDefault();
            if (existingBuyerByPhoneNumber != null)
            {
                return BadRequest("Phone number is already in use.");
            }

            var buyer = new Buyer
            {
                Name = buyerDto.Name,
                Surname = buyerDto.Surname,
                AddressLine = buyerDto.AddressLine,

                UserName = buyerDto.UserName,
                PasswordHash = buyerDto.PasswordHash,

                Email = buyerDto.Email,
                EmailConfirmed = buyerDto.EmailConfirmed,

                PhoneNumber = buyerDto.PhoneNumber,
                PhoneNumberConfirmed = buyerDto.PhoneNumberConfirmed,

                TwoFactorEnabled = buyerDto.TwoFactorEnabled,
                LockoutEnabled = buyerDto.LockoutEnabled,
                AccessFailedCount = buyerDto.AccessFailedCount
            };

            _buyerRepository.Add(buyer); // Add the buyer to the repository
            _buyerRepository.SaveChanges(); // Save changes to the database using the repository

            return CreatedAtAction(nameof(GetBuyerById), new { id = buyer.Id }, buyer);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Error creating new buyer record");
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Update a buyer by ID",
        Description = "Updates an existing buyer with the provided data.",
        OperationId = "UpdateBuyer")]
    public IActionResult UpdateBuyer(int id, [FromBody] BuyerDto buyerDto)
    {
        try
        {
            if (buyerDto == null) return BadRequest();

            var existingBuyer = _buyerRepository.GetById(id);

            if (existingBuyer == null)
            {
                return NotFound();
            }
            
            var existingBuyerByUsername = _buyerRepository.FindBy(buyer =>
                buyer.Id != id && buyer.UserName == buyerDto.UserName).FirstOrDefault();
            if (existingBuyerByUsername != null)
            {
                return BadRequest("Username is already in use.");
            }

            var existingBuyerByEmail = _buyerRepository.FindBy(buyer =>
                buyer.Id != id && buyer.Email == buyerDto.Email).FirstOrDefault();
            if (existingBuyerByEmail != null)
            {
                return BadRequest("Email is already in use.");
            }

            var existingBuyerByPhoneNumber = _buyerRepository.FindBy(buyer =>
                buyer.Id != id && buyer.PhoneNumber == buyerDto.PhoneNumber).FirstOrDefault();
            if (existingBuyerByPhoneNumber != null)
            {
                return BadRequest("Phone number is already in use.");
            }

            // Update the existing buyer properties
            existingBuyer.Name = buyerDto.Name;
            existingBuyer.Surname = buyerDto.Surname;
            existingBuyer.AddressLine = buyerDto.AddressLine;

            existingBuyer.UserName = buyerDto.UserName;
            existingBuyer.Email = buyerDto.Email;
            existingBuyer.EmailConfirmed = buyerDto.EmailConfirmed;

            existingBuyer.PasswordHash = buyerDto.PasswordHash;

            existingBuyer.PhoneNumber = buyerDto.PhoneNumber;
            existingBuyer.PhoneNumberConfirmed = buyerDto.PhoneNumberConfirmed;

            existingBuyer.TwoFactorEnabled = buyerDto.TwoFactorEnabled;
            existingBuyer.LockoutEnabled = buyerDto.LockoutEnabled;
            existingBuyer.AccessFailedCount = buyerDto.AccessFailedCount;
            // Update other properties as needed

            _buyerRepository.SaveChanges();

            return Ok(existingBuyer);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Error creating new buyer record");
        }
    }
    
    [HttpDelete("{id:int}")]
    [SwaggerOperation(Summary = "Delete a buyer by ID", Description = "Deletes a specific buyer by their ID.")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Buyer))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteBuyerById(int id)
    {
        try
        {
            var buyerToDelete = _buyerRepository.GetById(id);

            if (buyerToDelete == null)
            {
                return NotFound($"Buyer with Id = {id} not found");
            }

            _buyerRepository.Remove(buyerToDelete); // Remove the buyer from the repository
            _buyerRepository.SaveChanges(); // Save changes to the database

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
        Summary = "Delete a range of buyers",
        Description = "Deletes a range of buyers based on the provided IDs.",
        OperationId = "DeleteRange")]
    public IActionResult DeleteRange([FromBody] List<int> buyerIds)
    {
        if (buyerIds == null || !buyerIds.Any())
        {
            return BadRequest("Invalid buyer IDs.");
        }

        var buyersToDelete = _buyerRepository.FindBy(b => buyerIds.Contains(b.Id)).ToList();
        _buyerRepository.RemoveRange(buyersToDelete);
        _buyerRepository.SaveChanges();

        return NoContent();
    }
}