using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using OllamaSharp;


var ollamaEndPoint = "http://localhost:11434";
var mcpEndPoint = "https://learn.microsoft.com/api/mcp";
var modelName = "llama3.2";

IChatClient client = new OllamaApiClient(new Uri(ollamaEndPoint), modelName);


await using McpClient mcpClient = await McpClient.CreateAsync(new HttpClientTransport(
    new HttpClientTransportOptions {
    Endpoint = new Uri(mcpEndPoint) ,
    TransportMode = HttpTransportMode.StreamableHttp }
)) ;
IList<McpClientTool> mcpTools = await mcpClient.ListToolsAsync();
AIAgent aIAgent = client.AsAIAgent(
                  instructions: 
                  "You are  a great software developer in c sharp and .net using Microsoft Agent Framework.Use tool to find your knowledege " ,
                  name: "Programmer",
                  tools: mcpTools.Cast<AITool>().ToList());
AgentSession session = await aIAgent.CreateSessionAsync();

while (true)
{
    Console.WriteLine("Dr KnowItAll is ready. Type 'exit' to quit.\n");
    Console.Write("Please ask your question: ");
    var userInput = Console.ReadLine();
    if (userInput!.Equals("exit", StringComparison.OrdinalIgnoreCase))
        break;

   List <AgentResponseUpdate> responses = [];
    await foreach (var update in aIAgent.RunStreamingAsync(userInput, session))
    {
        responses.Add(update);
        Console.Write(update.Text);

    }
    AgentResponse agentResponse = responses.ToAgentResponse();
    Console.WriteLine("........................agent information.................\n");
    Console.WriteLine("Agent Id: " + agentResponse.AgentId);
    Console.WriteLine("Agent Input Token Used: " + agentResponse.Usage.InputTokenCount);
    Console.WriteLine("Agent Output Token Used: " + agentResponse.Usage.OutputTokenCount);
    Console.WriteLine("Agent Total Token Used: " + agentResponse.Usage.TotalTokenCount);



}