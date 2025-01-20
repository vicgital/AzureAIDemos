// Install the .NET library via NuGet: dotnet add package Azure.AI.OpenAI --prerelease
using Azure;
using Azure.AI.OpenAI;
using Azure.AI.OpenAI.Images;
using Azure.Identity;
using OpenAI.Images;
using System.ClientModel;
using static System.Environment;

string endpoint = GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");

DefaultAzureCredential credential = new DefaultAzureCredential();
AzureOpenAIClient azureClient = new AzureOpenAIClient(new Uri(endpoint), credential);
ImageClient client = azureClient.GetImageClient("dalle3");

ClientResult<GeneratedImage> imageResult = 
    await client.GenerateImageAsync(
        @"Monterrey, Mexico in the year 2050, 
        include the most iconic landmark Cerro de la Silla 
        and people on the streets wearing jerseys from the two local team Tigres", new()
{
    Quality = GeneratedImageQuality.Standard,
    Size = GeneratedImageSize.W1024xH1024,
    ResponseFormat = GeneratedImageFormat.Uri
});

// Image Generations responses provide URLs you can use to retrieve requested images
GeneratedImage image = imageResult.Value;
Console.WriteLine($"Image URL: {image.ImageUri}");