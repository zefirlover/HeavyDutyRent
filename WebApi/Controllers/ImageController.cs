using Domain.DTOs;
using Domain.Interfaces;
using Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImageController : ControllerBase
{
    private readonly IGenericRepository<Image> _imageRepository;
    private readonly IGenericRepository<Machinery> _machineryRepository;

    public ImageController(
        IGenericRepository<Image> imageRepository,
        IGenericRepository<Machinery> machineryRepository
    )
    {
        _imageRepository = imageRepository;
        _machineryRepository = machineryRepository;
    }
    
    [HttpGet("machinery/{machineryId}/images")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Image>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Get all images by MachineryId",
        Description = "Retrieves all images associated with a Machinery by its ID.",
        OperationId = "GetImagesByMachineryId")]
    public IActionResult GetImagesByMachineryId(int machineryId)
    {
        // Retrieve all images associated with the specified MachineryId
        var images = _imageRepository.FindBy(image => image.MachineryId == machineryId).ToList();

        if (images == null || !images.Any())
        {
            return NotFound($"No images found for Machinery with ID = {machineryId}");
        }

        return Ok(images);
    }
    
    [HttpGet("machinery/{machineryId}/image/{url}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Image>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Get all images by ImageUrl and MachineryId",
        Description = "Retrieves all images associated with a Machinery by its ID and its url.",
        OperationId = "GetImageByImageUrlAndMachineryId")]
    public IActionResult GetImageByImageUrlAndMachineryId(string url, int machineryId)
    {
        var images = _imageRepository.FindBy(image => image.MachineryId == machineryId && image.Url == url).FirstOrDefault();

        if (images == null)
        {
            return NotFound($"Image with Url = {url} and machineryId = {machineryId} was not found");
        }

        return Ok(images);
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Image))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "Create a new Image",
        Description = "Creates a new image with the provided data.",
        OperationId = "CreateImage")]
    public IActionResult CreateImage([FromBody] ImageDto imageDto)
    {
        try
        {
            if (imageDto == null) return BadRequest();

            var machinery = _machineryRepository.GetById(imageDto.MachineryId);
            if (machinery == null)
            {
                return BadRequest($"Machinery with Id = {imageDto.MachineryId} not found");
            }

            var image = new Image
            {
                Url = imageDto.Url,
                MachineryId = imageDto.MachineryId
            };

            _imageRepository.Add(image); // Add the buyer to the repository
            _imageRepository.SaveChanges(); // Save changes to the database using the repository

            return CreatedAtAction(nameof(GetImageByImageUrlAndMachineryId), new { url = image.Url, machineryId = image.MachineryId }, image);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Error creating new machinery record");
        }
    }
    
    [HttpDelete("machinery/{machineryId}/image/{url}")]
    [SwaggerOperation(Summary = "Delete an image by Url and MachineryId", Description = "Deletes a specific image by their Url and MachineryId.")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Image))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteImageByUrlAndMachineryId(string url, int machineryId)
    {
        try
        {
            var image = _imageRepository.FindBy(image => image.MachineryId == machineryId && image.Url == url).FirstOrDefault();

            if (image == null)
            {
                return NotFound($"Image with Url = {url} and machineryId = {machineryId} was not found");
            }

            _imageRepository.Remove(image);
            _imageRepository.SaveChanges();

            return NoContent();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting data");
        }
    }
}