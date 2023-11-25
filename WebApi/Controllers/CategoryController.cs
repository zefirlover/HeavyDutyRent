using Domain.DTOs;
using Domain.Interfaces;
using Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly IGenericRepository<Category> _categoryRepository;
    private readonly IGenericRepository<Machinery> _machineryRepository;

    public CategoryController(
        IGenericRepository<Machinery> machineryRepository,
        IGenericRepository<Category> categoryRepository
    )
    {
        _machineryRepository = machineryRepository;
        _categoryRepository = categoryRepository;
    }
    
    [HttpGet("all-categories")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Category>))]
    [SwaggerOperation(
        Summary = "Retrieve all categories",
        Description = "Gets a list of all categories with their information.",
        OperationId = "GetAllCategories")]
    public IActionResult GetAllCategories()
    {
        var categories = _categoryRepository.GetAll();
        return Ok(categories);
    }
    
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Retrieve a category by ID", Description = "Gets a specific category by their ID.")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Category))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetCategoryById(int id)
    {
        var category = _categoryRepository
            .Include(c => c.Machineries)
            .SingleOrDefault(c => c.Id == id);
        
        var machinery = _machineryRepository
            .Include(m => m.ImagesUrls)
            .ToList();

        if (category == null)
        {
            return NotFound();
        }

        var categoryDto = new GetCategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Machineries = category.Machineries.Select(
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
        
        categoryDto.Machineries = machinery
            .Where(m => category.Machineries.Any(cm => cm.Id == m.Id))
            .Select(m => new GetMachineryDto
            {
                Id = m.Id,
                Name = m.Name,
                AddressLine = m.AddressLine,
                Price = m.Price,
                SellerId = m.SellerId,
                Images = m.ImagesUrls.Select(
                    u => new ImageDto
                    {
                        Url = u.Url,
                        MachineryId = u.MachineryId
                    }
                ).ToList()
            }).ToList();

        return Ok(categoryDto);
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Category))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "Create a new category",
        Description = "Creates a new category with the provided data.",
        OperationId = "Category")]
    public IActionResult CreateCategory([FromBody] CategoryDto categoryDto)
    {
        try
        {
            if (categoryDto == null || categoryDto.MachineriesIds == null)
                return BadRequest("Invalid order data.");
            
            var existingCategoryByName = _categoryRepository.FindBy(category => category.Name == categoryDto.Name).FirstOrDefault();
            if (existingCategoryByName != null)
            {
                return BadRequest("Category name is already in use.");
            }

            var category = new Category
            {
                Name = categoryDto.Name
            };

            var machineryList = _machineryRepository
                .GetAll()
                .Where(m => categoryDto.MachineriesIds.Contains(m.Id))
                .ToList();

            category.Machineries = machineryList;

            _categoryRepository.Add(category);
            _categoryRepository.SaveChanges();

            return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Error creating new category record");
        }
    }
    
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Update a category by ID",
        Description = "Updates an existing category with the provided data.",
        OperationId = "UpdateCategory")]
    public IActionResult UpdateCategory(int id, [FromBody] CategoryDto categoryDto)
    {
        try
        {
            if (categoryDto == null || categoryDto.MachineriesIds == null)
                return BadRequest("Invalid order data.");

            var existingCategory = _categoryRepository.GetById(id);

            if (existingCategory == null)
            {
                return NotFound();
            }
            
            var existingCategoryByName = _categoryRepository.FindBy(category =>
                category.Id != id && category.Name == categoryDto.Name).FirstOrDefault();
            if (existingCategoryByName != null)
            {
                return BadRequest("Name is already in use.");
            }
            
            // Ensure that Machineries collection is initialized
            if (existingCategory.Machineries == null)
            {
                existingCategory.Machineries = new List<Machinery>();
            }

            // Determine machineries to remove
            var machineriesToRemove = existingCategory.Machineries
                .Where(m => !categoryDto.MachineriesIds.Contains(m.Id))
                .ToList();

            // Determine machineries to add
            var machineriesToAdd = _machineryRepository
                .GetAll()
                .Where(m => categoryDto.MachineriesIds.Contains(m.Id) && !existingCategory.Machineries.Contains(m))
                .ToList();

            // Remove machineries
            foreach (var machineryToRemove in machineriesToRemove)
            {
                existingCategory.Machineries.Remove(machineryToRemove);
            }

            // Add machineries
            foreach (var machineryToAdd in machineriesToAdd)
            {
                existingCategory.Machineries.Add(machineryToAdd);
            }

            existingCategory.Name = categoryDto.Name;

            _categoryRepository.SaveChanges();

            return Ok(existingCategory);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(StatusCodes.Status500InternalServerError, "Error creating new buyer record");
        }
    }
    
    [HttpDelete("{id:int}")]
    [SwaggerOperation(Summary = "Delete a category by ID", Description = "Deletes a specific category by their ID.")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Category))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteSellerById(int id)
    {
        try
        {
            var categoryToDelete = _categoryRepository.GetById(id);

            if (categoryToDelete == null)
            {
                return NotFound($"Category with Id = {id} not found");
            }

            _categoryRepository.Remove(categoryToDelete);
            _categoryRepository.SaveChanges();

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
        Summary = "Delete a range of categories",
        Description = "Deletes a range of categories based on the provided IDs.",
        OperationId = "DeleteRange")]
    public IActionResult DeleteRange([FromBody] List<int> categoryIds)
    {
        if (categoryIds == null || !categoryIds.Any())
        {
            return BadRequest("Invalid sellers IDs.");
        }

        var categoriesToDelete = _categoryRepository.FindBy(b => categoryIds.Contains(b.Id)).ToList();
        _categoryRepository.RemoveRange(categoriesToDelete);
        _categoryRepository.SaveChanges();

        return NoContent();
    }
}