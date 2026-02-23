using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.AI;
using System.ComponentModel;
using LetsChatAppBackEnd.Services;
using System.Text;

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
        3. Answer based ONLY on the search results using the information provided
        4. Be specific and quote relevant parts from the results

        If no search results are found, say 'I could not find information about that in the documents.'

        Use simple markdown to format your responses.
        ";

    // Store search results for citation generation
    private readonly List<SearchResultCitation> _currentSearchResults = new();

    public async Task SendMessage(string message, List<ChatMessageDto> conversationHistory, string? conversationId)
    {
        try
        {
            Console.WriteLine($"📨 Received message: {message}");

            // Clear previous search results
            _currentSearchResults.Clear();

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

            var responseBuilder = new StringBuilder();

            await foreach (var update in chatClient.GetStreamingResponseAsync(messages, chatOptions))
            {
                // Log if function call is happening
                if (update.Contents?.Any(c => c is FunctionCallContent) == true)
                {
                    Console.WriteLine("🔧 LLM is calling a function!");
                }

                var text = update.Text ?? string.Empty;
                responseBuilder.Append(text);
                await Clients.Caller.SendAsync("ReceiveMessageChunk", text);

                if (update.ConversationId != null)
                {
                    conversationId = update.ConversationId;
                }
            }

            // Append citations if search was performed
            if (_currentSearchResults.Count > 0)
            {
                Console.WriteLine($"📝 Appending {_currentSearchResults.Count} citations");

                var citationsText = GenerateCitationsMarkdown(_currentSearchResults);
                await Clients.Caller.SendAsync("ReceiveMessageChunk", citationsText);
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

        // Store results for citation generation
        _currentSearchResults.Clear();
        foreach (var result in results)
        {
            _currentSearchResults.Add(new SearchResultCitation
            {
                DocumentId = result.DocumentId,
                PageNumber = result.PageNumber,
                Text = result.Text
            });
        }

        return results.Select(result =>
            $"<result filename=\"{result.DocumentId}\" page_number=\"{result.PageNumber}\">{result.Text}</result>");
    }

    private static string GenerateCitationsMarkdown(List<SearchResultCitation> citations)
    {
        if (citations.Count == 0) return string.Empty;

        var sb = new StringBuilder();
        sb.AppendLine("\n\n---");
        sb.AppendLine("\n**Sources:**\n");

        var groupedByDocument = citations
            .GroupBy(c => c.DocumentId)
            .OrderBy(g => g.Key);

        foreach (var docGroup in groupedByDocument)
        {
            var pages = docGroup
                .Select(c => c.PageNumber)
                .Distinct()
                .OrderBy(p => p);

            sb.AppendLine($"- **{docGroup.Key}** (Pages: {string.Join(", ", pages)})");
        }

        return sb.ToString();
    }

    private class SearchResultCitation
    {
        public required string DocumentId { get; init; }
        public int PageNumber { get; init; }
        public required string Text { get; init; }
    }
}

public record ChatMessageDto(string Role, string Content);
