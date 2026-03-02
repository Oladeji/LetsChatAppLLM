////using Microsoft.Extensions.AI;
////using Microsoft.Agents.AI   ; 
////using Microsoft.Extensions.DependencyInjection;

////var services = new ServiceCollection();

////// Configure Ollama chat client
////var ollamaEndPoint = "http://localhost:11434";
////var modelName = "llama3.2";

////services.AddChatClient(
////    new OllamaSharp.OllamaApiClient(new Uri(ollamaEndPoint), modelName)
////);

////// Build DI container
////var provider = services.BuildServiceProvider();

////// Get the underlying chat client from DI
////var chatClient = provider.GetRequiredService<IChatClient>();

////// Create an agent with system prompt
////var agent = new ChatAgent(
////    chatClient,
////    new ChatAgentOptions
////    {
////        SystemPrompt = "You are a medical doctor. You know general medicine and your name is Dr KnowItAll."
////    }
////);

////Console.WriteLine("Dr KnowItAll is ready. Type 'exit' to quit.\n");

////while (true)
////{
////    Console.Write("Please ask your question: ");
////    var userInput = Console.ReadLine();

////    if (string.IsNullOrWhiteSpace(userInput))
////    {
////        continue;
////    }

////    if (userInput.Equals("exit", StringComparison.OrdinalIgnoreCase))
////    {
////        break;
////    }

////    Console.WriteLine("Thinking.......................");

////    // Streaming agent response
////    var responseBuilder = new System.Text.StringBuilder();

////    await foreach (var update in agent.GetStreamingResponseAsync(userInput))
////    {
////        // Each update is a ChatResponseUpdate; take new text content
////        foreach (var content in update.Contents)
////        {
////            if (content is TextContent text && !string.IsNullOrEmpty(text.Text))
////            {
////                Console.Write(text.Text);
////                responseBuilder.Append(text.Text);
////            }
////        }
////    }

////    Console.WriteLine("\n\n");
////}

//using Microsoft.Agents.AI;
//using Microsoft.Extensions.AI;
//using OllamaSharp;
//using System.Text;

//var ollamaEndPoint = "http://localhost:11434";
//var modelName = "llama3.2";

//IChatClient client = new OllamaApiClient(new Uri(ollamaEndPoint), modelName);
//AIAgent aIAgent = client.AsAIAgent(
//     instructions:"You are a medical doctor. You know general medicine and your name is Dr KnowItAll.",
//        name: "Doctor");

//AgentSession session = await aIAgent.CreateSessionAsync();

//while (true)
//{
//    Console.WriteLine("Dr KnowItAll is ready. Type 'exit' to quit.\n");
//    Console.Write("Please ask your question: ");
//    var userInput = Console.ReadLine();
//    if (userInput.Equals("exit", StringComparison.OrdinalIgnoreCase))
//        Console.WriteLine("Thinking.......................");

//   List <AgentResponseUpdate> responses = [];
//    await foreach (var update in aIAgent.RunStreamingAsync(userInput, session))
//    {
//        responses.Add(update);
//        Console.Write(update.Text);
//        //foreach (var content in update.Contents)
//        //{
//        //    if (content is TextContent text && !string.IsNullOrEmpty(text.Text))
//        //    {

//        //        Console.Write(text.Text);
//        //    //    Console.Write(text.);
//        //    }
//        //}
//    }
//    AgentResponse agentResponse = responses.ToAgentResponse();
//    Console.WriteLine("........................agent information.................\n");
//    Console.WriteLine("Agent Id: " + agentResponse.AgentId);
//    Console.WriteLine("Agent Input Token Used: " + agentResponse.Usage.InputTokenCount);
//    Console.WriteLine("Agent Output Token Used: " + agentResponse.Usage.OutputTokenCount);
//    Console.WriteLine("Agent Total Token Used: " + agentResponse.Usage.TotalTokenCount);



//}