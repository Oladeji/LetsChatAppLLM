//////using Microsoft.Extensions.AI;
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
//// System instructions as the first message in history
//var history = new List<ChatMessage>
//{
//    new(ChatRole.System,
//        "You are a medical doctor. You know general medicine and your name is Dr KnowItAll.")
//};

////AIAgent aIAgent2 = new OllamaApiClient(new Uri(ollamaEndPoint), modelName).AsAIAgent(instructions: "You are a medical doctor.You know general medicine and your name is Dr KnowItAll.", name:"Doctor");
//AIAgent aIAgent = client.AsAIAgent( name: "Doctor");


////AgentSession session = await aIAgent.CreateSessionAsync();

//Console.WriteLine("Dr KnowItAll is ready. Type 'exit' to quit.\n");
//Console.Write("Please ask your question: ");
//var userInput = Console.ReadLine();
//if (userInput.Equals("exit", StringComparison.OrdinalIgnoreCase))   
//    Console.WriteLine("Thinking.......................");
//    // Add user turn to history
//    history.Add(new ChatMessage(ChatRole.User, userInput));
//    var sb = new StringBuilder();

//    await foreach (var update in aIAgent.RunStreamingAsync(history))
//{
//        foreach (var content in update.Contents)
//        {
//            if (content is TextContent text && !string.IsNullOrEmpty(text.Text))
//            {
                
//                Console.Write(text.Text);
//                sb.Append(text.Text);
//            }
//        }
//    }
//    Console.WriteLine("\n");

//    // Add assistant reply to history
//    var assistantReply = sb.ToString();
//    history.Add(new ChatMessage(ChatRole.Assistant, assistantReply));
//}