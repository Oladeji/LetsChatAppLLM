using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.AI;
using System.ComponentModel;
using LetsChatAppBackEnd.Services;

namespace LetsChatAppBackEnd.Hubs;

public class ChatHub(IChatClient chatClient, SemanticSearch search) : Hub
{
    private const string SystemPrompt = @"
        You are an assistant who answers questions about information you retrieve.
        Do not answer questions about anything else.
        Use only simple markdown to format your responses.

        Use the search tool to find relevant information. When you do this, end your
        reply with citations in the special XML format:

        <citation filename='string' page_number='number'>exact quote here</citation>

        Always include the citation in your response if there are results.

        The quote must be max 5 words, taken word-for-word from the search result, and is the basis for why the citation is relevant.
        Don't refer to the presence of citations; just emit these tags right at the end, with no surrounding text.
        ";

    public async Task SendMessage(string message, List<ChatMessageDto> conversationHistory, string? conversationId)
    {
        try
        {
            Console.WriteLine($"📨 Received message: {message}");

            var messages = new List<ChatMessage>
            {
                new(ChatRole.System, SystemPrompt)
            };

            messages.AddRange(conversationHistory.Select(m => new ChatMessage(
                m.Role == "user" ? ChatRole.User : ChatRole.Assistant,
                m.Content
            )));

            messages.Add(new ChatMessage(ChatRole.User, message));

            var chatOptions = new ChatOptions
            {
                Tools = [AIFunctionFactory.Create(SearchAsync)],
                ConversationId = conversationId
            };

            await Clients.Caller.SendAsync("ReceiveMessageStart");
            Console.WriteLine("✅ Started streaming response");

            await foreach (var update in chatClient.GetStreamingResponseAsync(messages, chatOptions))
            {
                await Clients.Caller.SendAsync("ReceiveMessageChunk", update.Text ?? string.Empty);

                if (update.ConversationId != null)
                {
                    conversationId = update.ConversationId;
                }
            }

            await Clients.Caller.SendAsync("ReceiveMessageEnd", conversationId);
            Console.WriteLine("✅ Finished streaming response");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error in SendMessage: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            await Clients.Caller.SendAsync("ReceiveError", ex.Message);
            throw;
        }
    }

    [Description("Searches for information using a phrase or keyword")]
    private async Task<IEnumerable<string>> SearchAsync(
        [Description("The phrase to search for.")] string searchPhrase,
        [Description("If possible, specify the filename to search that file only. If not provided or empty, the search includes all files.")] string? filenameFilter = null)
    {
        var results = await search.SearchAsync(searchPhrase, filenameFilter, maxResults: 5);
        return results.Select(result =>
            $"<result filename=\"{result.DocumentId}\" page_number=\"{result.PageNumber}\">{result.Text}</result>");
    }
}

public record ChatMessageDto(string Role, string Content);
