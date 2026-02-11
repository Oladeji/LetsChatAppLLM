using Microsoft.Extensions.AI;

var ollamaEndPoint = "http://localhost:11434";
var modelName = "llama3.2";

IChatClient  client = new OllamaSharp.OllamaApiClient(new Uri(ollamaEndPoint), modelName);
//Console.WriteLine("Starting......");
//var responseStream =  client.GetStreamingResponseAsync("What is the most mentioned name of a person  ");

//await   foreach (var response in responseStream)
//{
//   Console.Write(response.Text);


//}

List<ChatMessage> messageHistory = new()
{
    new ChatMessage(ChatRole.System, "You a medical doctor you know general medicine  and your name id Dr KnowItAll")
};


while (true)
{
    // get user input and add it to the message history
    Console.WriteLine("Please ask your question:?");
    var userInput = Console.ReadLine();

    messageHistory.Add(new ChatMessage(ChatRole.User, userInput));
    // Get the response from the chat client and add it to the message history
    Console.WriteLine("Thinking.......................");
    var response ="";
    // The GetStreamingResponseAsync method returns an async stream of messages, which we can iterate over using await foreach
    // and output the text of each message as it arrives. We also concatenate the text of each message to build the full response, which we add to the message history once the stream is complete.
    await foreach (var message in client.GetStreamingResponseAsync(messageHistory))
    {
        Console.Write(message.Text);
        response += message.Text;
    }
    // Once the stream is complete, we add the full response to the message history
    messageHistory.Add(new ChatMessage(ChatRole.Assistant, response));

    Console.WriteLine("\n\n");
}


