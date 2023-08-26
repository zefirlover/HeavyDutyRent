using Domain.DTOs;
using Domain.Interfaces;
using Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SellerController : ControllerBase
{
    private readonly IGenericRepository<Seller> _sellerRepository;
    
    public SellerController(IGenericRepository<Seller> sellerRepository)
    {
        _sellerRepository = sellerRepository;
    }
    
    [HttpGet("all-sellers")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Seller>))]
    [SwaggerOperation(
        Summary = "Retrieve all sellers",
        Description = "Gets a list of all sellers with their information.",
        OperationId = "GetAllSellers")]
    public IActionResult GetAllSellers()
    {
        var sellers = _sellerRepository.GetAll();
        return Ok(sellers);
    }
    
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Retrieve a seller by ID", Description = "Gets a specific seller by their ID.")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Seller))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetSellerById(int id)
    {
        var seller = _sellerRepository.GetById(id);

        if (seller == null)
        {
            return NotFound();
        }

        return Ok(seller);
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Seller))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "Create a new Seller",
        Description = "Creates a new seller with the provided data.",
        OperationId = "CreateSeller")]
    public IActionResult CreateSeller([FromBody] SellerDto sellerDto)
    {
        try
        {
            if (sellerDto == null) return BadRequest();
            
            var existingSellerByUsername = _sellerRepository.FindBy(seller => seller.UserName == sellerDto.UserName).FirstOrDefault();
            if (existingSellerByUsername != null)
            {
                return BadRequest("Username is already in use.");
            }
            
            var existingSellerByEmail = _sellerRepository.FindBy(seller => seller.Email == sellerDto.Email).FirstOrDefault();
            if (existingSellerByEmail != null)
            {
                return BadRequest("Email is already in use.");
            }
            
            var existingSellerByPhoneNumber = _sellerRepository.FindBy(seller => seller.PhoneNumber == sellerDto.PhoneNumber).FirstOrDefault();
            if (existingSellerByPhoneNumber != null)
            {
                return BadRequest("Phone number is already in use.");
            }

            var seller = new Seller
            {
                AddressLine = sellerDto.AddressLine,
                LogoUrl = sellerDto.LogoUrl,

                UserName = sellerDto.UserName,
                PasswordHash = sellerDto.PasswordHash,

                Email = sellerDto.Email,
                EmailConfirmed = sellerDto.EmailConfirmed,

                PhoneNumber = sellerDto.PhoneNumber,
                PhoneNumberConfirmed = sellerDto.PhoneNumberConfirmed,

                TwoFactorEnabled = sellerDto.TwoFactorEnabled,
                LockoutEnabled = sellerDto.LockoutEnabled,
                AccessFailedCount = sellerDto.AccessFailedCount
            };

            _sellerRepository.Add(seller); // Add the buyer to the repository
            _sellerRepository.SaveChanges(); // Save changes to the database using the repository

            return CreatedAtAction(nameof(GetSellerById), new { id = seller.Id }, seller);
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
        Summary = "Update a seller by ID",
        Description = "Updates an existing seller with the provided data.",
        OperationId = "UpdateSeller")]
    public IActionResult UpdateSeller(int id, [FromBody] SellerDto sellerDto)
    {
        try
        {
            if (sellerDto == null) return BadRequest();

            var existingSeller = _sellerRepository.GetById(id);

            if (existingSeller == null)
            {
                return NotFound();
            }
            
            var existingSellerByUsername = _sellerRepository.FindBy(seller =>
                seller.Id != id && seller.UserName == sellerDto.UserName).FirstOrDefault();
            if (existingSellerByUsername != null)
            {
                return BadRequest("Username is already in use.");
            }

            var existingSellerByEmail = _sellerRepository.FindBy(seller =>
                seller.Id != id && seller.Email == sellerDto.Email).FirstOrDefault();
            if (existingSellerByEmail != null)
            {
                return BadRequest("Email is already in use.");
            }

            var existingSellerByPhoneNumber = _sellerRepository.FindBy(seller =>
                seller.Id != id && seller.PhoneNumber == sellerDto.PhoneNumber).FirstOrDefault();
            if (existingSellerByPhoneNumber != null)
            {
                return BadRequest("Phone number is already in use.");
            }

            // Update the existing buyer properties
            existingSeller.AddressLine = sellerDto.AddressLine;
            existingSeller.LogoUrl = sellerDto.LogoUrl;

            existingSeller.UserName = sellerDto.UserName;
            existingSeller.Email = sellerDto.Email;
            existingSeller.EmailConfirmed = sellerDto.EmailConfirmed;

            existingSeller.PasswordHash = sellerDto.PasswordHash;

            existingSeller.PhoneNumber = sellerDto.PhoneNumber;
            existingSeller.PhoneNumberConfirmed = sellerDto.PhoneNumberConfirmed;

            existingSeller.TwoFactorEnabled = sellerDto.TwoFactorEnabled;
            existingSeller.LockoutEnabled = sellerDto.LockoutEnabled;
            existingSeller.AccessFailedCount = sellerDto.AccessFailedCount;
            // Update other properties as needed

            _sellerRepository.SaveChanges();

            return Ok(existingSeller);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Error creating new buyer record");
        }
    }
    
    [HttpDelete("{id:int}")]
    [SwaggerOperation(Summary = "Delete a seller by ID", Description = "Deletes a specific seller by their ID.")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Seller))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteSellerById(int id)
    {
        try
        {
            var sellerToDelete = _sellerRepository.GetById(id);

            if (sellerToDelete == null)
            {
                return NotFound($"Seller with Id = {id} not found");
            }

            _sellerRepository.Remove(sellerToDelete); // Remove the buyer from the repository
            _sellerRepository.SaveChanges(); // Save changes to the database

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
        Summary = "Delete a range of sellers",
        Description = "Deletes a range of sellers based on the provided IDs.",
        OperationId = "DeleteRange")]
    public IActionResult DeleteRange([FromBody] List<int> sellerIds)
    {
        if (sellerIds == null || !sellerIds.Any())
        {
            return BadRequest("Invalid sellers IDs.");
        }

        var buyersToDelete = _sellerRepository.FindBy(b => sellerIds.Contains(b.Id)).ToList();
        _sellerRepository.RemoveRange(buyersToDelete);
        _sellerRepository.SaveChanges();

        return NoContent();
    }
}