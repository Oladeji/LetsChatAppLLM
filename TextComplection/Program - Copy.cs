//using Microsoft.Extensions.AI;
//var ollamaEndPoint = "http://localhost:11434";
//var modelName = "llama3.2";

//IChatClient  client = new OllamaSharp.OllamaApiClient(new Uri(ollamaEndPoint), modelName);


//List<ChatMessage> messageHistory = new()
//{
//    new ChatMessage(ChatRole.System, "You a medical doctor you know general medicine  and your name id Dr KnowItAll")
//};


//while (true)
//{

//    Console.WriteLine("Please ask your question:?");
//    var userInput = Console.ReadLine();

//    messageHistory.Add(new ChatMessage(ChatRole.User, userInput));

//    Console.WriteLine("Thinking.......................");
//    var response ="";
//    await foreach (var message in client.GetStreamingResponseAsync(messageHistory))
//    {
//        Console.Write(message.Text);
//        response += message.Text;
//    }
//    messageHistory.Add(new ChatMessage(ChatRole.Assistant, response));

//    Console.WriteLine("\n\n");
//}


