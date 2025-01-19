﻿// Implicit using statements are included
using System.Text;
using System.ClientModel;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Azure;

// Add Azure OpenAI packages
using Azure.AI.OpenAI;
using OpenAI.Chat;


// Build a config object and retrieve user settings.
class ChatMessageLab
{

    static string? oaiEndpoint;
    static string? oaiKey;
    static string? oaiDeploymentName;
    static void Main(string[] args)
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        oaiEndpoint = config["AzureOAIEndpoint"];
        oaiKey = config["AzureOAIKey"];
        oaiDeploymentName = config["AzureOAIDeploymentName"];


        do
        {
            // Pause for system message update
            Console.WriteLine("-----------\nPausing the app to allow you to change the system prompt.\nPress any key to continue...");
            Console.ReadKey();

            Console.WriteLine("\nUsing system message from system.txt");
            string systemMessage = System.IO.File.ReadAllText("system.txt");
            systemMessage = systemMessage.Trim();

            Console.WriteLine("\nEnter user message or type 'quit' to exit:");
            string userMessage = Console.ReadLine() ?? "";
            userMessage = userMessage.Trim();

            if (systemMessage.ToLower() == "quit" || userMessage.ToLower() == "quit")
            {
                break;
            }
            else if (string.IsNullOrEmpty(systemMessage) || string.IsNullOrEmpty(userMessage))
            {
                Console.WriteLine("Please enter a system and user message.");
                continue;
            }
            else
            {
                GetResponseFromOpenAI(systemMessage, userMessage);
            }
        } while (true);

    }

    private static void GetResponseFromOpenAI(string systemMessage, string userMessage)
    {
        Console.WriteLine("\nSending prompt to Azure OpenAI endpoint...\n\n");

        if (string.IsNullOrEmpty(oaiEndpoint) || string.IsNullOrEmpty(oaiKey) || string.IsNullOrEmpty(oaiDeploymentName))
        {
            Console.WriteLine("Please check your appsettings.json file for missing or incorrect values.");
            return;
        }

        Console.WriteLine("\nAdding grounding context from grounding.txt");
        string groundingText = System.IO.File.ReadAllText("grounding.txt");
        userMessage = groundingText + userMessage;

        // Configure the Azure OpenAI client
        AzureOpenAIClient azureClient = new(new Uri(oaiEndpoint), new ApiKeyCredential(oaiKey));
        ChatClient chatClient = azureClient.GetChatClient(oaiDeploymentName);
        ChatCompletion completion = chatClient.CompleteChat(
        [
            new SystemChatMessage(systemMessage),
            new UserChatMessage(userMessage),
        ]);



        // Get response from Azure OpenAI
        Console.WriteLine($"{completion.Role}: {completion.Content[0].Text}");




    }

}