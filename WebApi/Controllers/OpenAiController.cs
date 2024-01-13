using Microsoft.AspNetCore.Mvc;
using OpenAI_API;
using OpenAI_API.Chat;
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
            Model = OpenAI_API.Models.Model.CurieText,
            MaxTokens = 256
        };

        var result = await openai.Chat.CreateChatCompletionAsync(new ChatRequest()
        {
            Model = OpenAI_API.Models.Model.ChatGPTTurbo,
            MaxTokens = 50,
            Temperature = 0.5,
            Messages = new ChatMessage[]
            {
                new (ChatMessageRole.System, "You are a helpful assistant."),
                new (ChatMessageRole.System, "You are working on online service for renting construction, work and agricultural equipment"),
                new (ChatMessageRole.System, "Do not mention availability or location-based variances in your responses."),
                //new (ChatMessageRole.System, "On 'Thank you' you need to answer 'You are welcome.' without additional sentence."),
                new (ChatMessageRole.User, query)
            }
        });

        foreach (var completion in result.Choices)
        {
            outputResult += completion.Message.Content.Trim();
        }

        return Ok(outputResult);
    }
}