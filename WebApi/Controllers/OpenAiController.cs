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
        var completionRequest = new CompletionRequest
        {
            Prompt = query,
            Model = OpenAI_API.Models.Model.DavinciText,
            MaxTokens = 512
        };

        var completions = await openai.Completions.CreateCompletionAsync(completionRequest);

        foreach (var completion in completions.Completions)
        {
            outputResult += completion.Text;
        }

        return Ok(outputResult);

    }
}