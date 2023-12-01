using Microsoft.AspNetCore.Mvc;
using OpenAI_API;
using OpenAI_API.Completions;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OpenAiController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public OpenAiController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    [HttpGet]
    [Route("UseChatGPT")]
    [SwaggerOperation(
        Summary = "Get answer from chat gpt",
        Description = "Get answer from chat gpt.",
        OperationId = "UseChatGpt")]
    public async Task<IActionResult> UseChatGpt(string query)
    {
        var outputResult = "";
        var openai = new OpenAIAPI(_configuration["OpenAI:ApiKey"]);
        var messages = new[]
        {
            new { role = "system", content = "You are a helpful assistant." },
            new { role = "system", content = "You are working on online service for renting construction, work and agricultural equipment" },
            // new { role = "user", content = "Hello" },
        };
        var context = string.Join("\n", messages.Select(msg => $"{msg.role}: {msg.content}"));
        var promptWithContext = $"{context}\nUser: {query}";

        var completionRequest = new CompletionRequest
        {
            Prompt = promptWithContext,
            Model = OpenAI_API.Models.Model.DavinciText,
            MaxTokens = 256
        };

        var completions = await openai.Completions.CreateCompletionAsync(completionRequest);

        foreach (var completion in completions.Completions)
        {
            outputResult += completion.Text;
        }

        return Ok(outputResult);
    }
}