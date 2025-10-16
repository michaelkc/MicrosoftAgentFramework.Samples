using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using OpenAI;
using System.ClientModel;
using System.ComponentModel;

var endpoint = 
    Environment.GetEnvironmentVariable("SEGESAGENT_OPENAI_ENDPOINT") ?? 
    throw new InvalidOperationException("SEGESAGENT_OPENAI_ENDPOINT is not set.");
var deploymentName = 
    Environment.GetEnvironmentVariable("SEGESAGENT_OPENAI_DEPLOYMENT_NAME") ?? 
    "gpt-4o";
var apiKey = Environment.GetEnvironmentVariable("SEGESAGENT_OPENAI_APIKEY") ?? 
    throw new InvalidOperationException("SEGESAGENT_OPENAI_APIKEY is not set.");

[Description("Get the weather for a given location.")]
static string GetWeather([Description("The location to get the weather for.")] string location)
    => $"The weather in {location} is cloudy with a high of 15°C.";

var client = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apiKey!));
var chatClient = client.GetChatClient(deploymentName);
var agent = chatClient.CreateAIAgent(
    instructions: "You are a helpful assistant",
    tools: [AIFunctionFactory.Create(GetWeather)]);

Console.WriteLine(await agent.RunAsync("What is the weather like in Amsterdam?"));

await foreach (var update in agent.RunStreamingAsync("What is the weather like in Amsterdam?"))
{
    Console.Write(update);
}
