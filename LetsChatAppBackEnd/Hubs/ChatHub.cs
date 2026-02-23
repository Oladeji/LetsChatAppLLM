using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.AI;
using System.ComponentModel;
using LetsChatAppBackEnd.Services;

namespace LetsChatAppBackEnd.Hubs;

public class ChatHub(IChatClient chatClient, SemanticSearch search) : Hub
{
    private const string SystemPrompt = @"
        You are an assistant who answers questions about information in documents.

        IMPORTANT: You MUST use the SearchAsync tool to find information before answering any question.
        Never answer from your own knowledge - always search first using the tool.

        Steps:
        1. Call SearchAsync with the user's question or relevant keywords
        2. Wait for the search results
        3. Answer based ONLY on the search results
        4. Add citations in this XML format: <citation filename='string' page_number='number'>exact quote</citation>

        If no search results are found, say 'I could not find information about that in the documents.'

        Use simple markdown to format your responses.
        The quote in citations must be max 5 words, taken word-for-word from the search result.
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
                // Log if function call is happening
                if (update.Contents?.Any(c => c is FunctionCallContent) == true)
                {
                    Console.WriteLine("🔧 LLM is calling a function!");
                }

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
        [Description("If possible, specify the filename to search that file only. If not provided or empty, the search includes all files.")] string filenameFilter = "")
    {
        Console.WriteLine($"🔍 SearchAsync called with phrase: '{searchPhrase}', filter: '{filenameFilter}'");

        var filenameFilterValue = string.IsNullOrWhiteSpace(filenameFilter) ? null : filenameFilter;
        var results = await search.SearchAsync(searchPhrase, filenameFilterValue, maxResults: 5);

        Console.WriteLine($"📊 Search returned {results.Count} results");

        return results.Select(result =>
            $"<result filename=\"{result.DocumentId}\" page_number=\"{result.PageNumber}\">{result.Text}</result>");
    }
}

public record ChatMessageDto(string Role, string Content);
