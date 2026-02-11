using FunctionCallerLLM;
using Microsoft.Extensions.AI;

var OllamaEndPoint = "http://localhost:11434";

var modelName = "llama3.2";

//IChatClient client = new OllamaSharp.OllamaApiClient(new Uri(OllamaEndPoint), modelName);
IChatClient client = new ChatClientBuilder( new OllamaSharp.OllamaApiClient(new Uri(OllamaEndPoint), modelName)) 
    .UseFunctionInvocation()
    .Build();




var chatFunction = new ChatOptions
{
    Tools = [ AIFunctionFactory.Create(SpecialCalculator.ComplexIntegral , "wheather", "Get the weather of any country" ),
                    
              AIFunctionFactory.Create(SpecialCalculator.DescribeComplexProblem , "fishname", "Get the name of any unknown fish") ,
            ]

};


List<ChatMessage> messageHistory = new()
{
    new ChatMessage(ChatRole.System, "You are a helpful assistant that can perform basic math calculations. You should only respond with the answer to the calculation and nothing else.")
};
// put in a continuos loop until user type exit 
Console.WriteLine("Please enter a math calculation (or type 'exit' to quit):");
var userInput = Console.ReadLine();
while (!userInput!.ToUpper().Equals("EXIT"))
{


    messageHistory.Add(new ChatMessage(ChatRole.User, userInput));
    Console.WriteLine("tHINKING AND Calculating...");
    var response = "";
    await foreach (var message in client.GetStreamingResponseAsync(messageHistory, chatFunction))
    {
        Console.Write(message.Text);
        response += message.Text;
    }
    messageHistory.Add(new ChatMessage(ChatRole.Assistant, response));

    Console.WriteLine("\nPlease enter a math calculation (or type 'exit' to quit):");
    userInput = Console.ReadLine();
}

