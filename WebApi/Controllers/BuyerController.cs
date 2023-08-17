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
            
            var existingUserByUsername = _buyerRepository.FindBy(user => user.UserName == buyerDto.UserName).FirstOrDefault();
            if (existingUserByUsername != null)
            {
                return BadRequest("Username is already in use.");
            }
            
            var existingUserByEmail = _buyerRepository.FindBy(user => user.Email == buyerDto.Email).FirstOrDefault();
            if (existingUserByEmail != null)
            {
                return BadRequest("Email is already in use.");
            }
            
            var existingUserByPhoneNumber = _buyerRepository.FindBy(user => user.PhoneNumber == buyerDto.PhoneNumber).FirstOrDefault();
            if (existingUserByPhoneNumber != null)
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
                return NotFound($"Employee with Id = {id} not found");
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
}