using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using System.Text.Json;
using System.ClientModel;

namespace Fall2025_Project3_gbward.Services
{
    public class AzureOpenAIService
    {
        private readonly string _endpoint;
        private readonly string _apiKey;
        private readonly string _deploymentName;

        public AzureOpenAIService(IConfiguration configuration)
        {
            _endpoint = configuration["AzureOpenAI:Endpoint"] ?? throw new ArgumentNullException("AzureOpenAI:Endpoint");
            _apiKey = configuration["AzureOpenAI:ApiKey"] ?? throw new ArgumentNullException("AzureOpenAI:ApiKey");
            _deploymentName = configuration["AzureOpenAI:DeploymentName"] ?? "gpt-4.1-nano";
        }

        public async Task<List<string>> GenerateMovieReviewsAsync(string movieTitle, int count = 10)
        {
            var client = new AzureOpenAIClient(new Uri(_endpoint), new AzureKeyCredential(_apiKey));
            var chatClient = client.GetChatClient(_deploymentName);

            var prompt = $@"Generate exactly {count} diverse movie reviews for the movie ""{movieTitle}"". 
Each review should be 2-3 sentences long and represent different perspectives (positive, negative, mixed).
Return ONLY a valid JSON array of strings, with no additional text or formatting.
Example format: [""review1"", ""review2"", ""review3""]";

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("You are a helpful assistant that generates movie reviews. Always respond with valid JSON arrays only."),
                new UserChatMessage(prompt)
            };

            var options = new ChatCompletionOptions
            {
                MaxOutputTokenCount = 1500,
                Temperature = 0.8f
            };

            var response = await chatClient.CompleteChatAsync(messages, options);
            var content = response.Value.Content[0].Text;

            // Clean up the response to ensure it's valid JSON
            content = content.Trim();
            if (content.StartsWith("```json"))
            {
                content = content.Substring(7);
            }
            if (content.StartsWith("```"))
            {
                content = content.Substring(3);
            }
            if (content.EndsWith("```"))
            {
                content = content.Substring(0, content.Length - 3);
            }
            content = content.Trim();

            try
            {
                var reviews = JsonSerializer.Deserialize<List<string>>(content);
                return reviews ?? new List<string>();
            }
            catch (JsonException)
            {
                // If JSON parsing fails, try to extract reviews manually
                return ExtractReviewsManually(content, count);
            }
        }

        public async Task<List<string>> GenerateActorTweetsAsync(string actorName, int count = 20)
        {
            var client = new AzureOpenAIClient(new Uri(_endpoint), new AzureKeyCredential(_apiKey));
            var chatClient = client.GetChatClient(_deploymentName);

            var prompt = $@"Generate exactly {count} diverse tweets about the actor ""{actorName}"". 
Each tweet should be 1-2 sentences and represent different perspectives and topics (career highlights, recent news, fan reactions, critical acclaim, etc.).
Keep tweets under 280 characters each.
Return ONLY a valid JSON array of strings, with no additional text or formatting.
Example format: [""tweet1"", ""tweet2"", ""tweet3""]";

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("You are a helpful assistant that generates social media content. Always respond with valid JSON arrays only."),
                new UserChatMessage(prompt)
            };

            var options = new ChatCompletionOptions
            {
                MaxOutputTokenCount = 2000,
                Temperature = 0.8f
            };

            var response = await chatClient.CompleteChatAsync(messages, options);
            var content = response.Value.Content[0].Text;

            // Clean up the response
            content = content.Trim();
            if (content.StartsWith("```json"))
            {
                content = content.Substring(7);
            }
            if (content.StartsWith("```"))
            {
                content = content.Substring(3);
            }
            if (content.EndsWith("```"))
            {
                content = content.Substring(0, content.Length - 3);
            }
            content = content.Trim();

            try
            {
                var tweets = JsonSerializer.Deserialize<List<string>>(content);
                return tweets ?? new List<string>();
            }
            catch (JsonException)
            {
                return ExtractReviewsManually(content, count);
            }
        }

        private List<string> ExtractReviewsManually(string content, int expectedCount)
        {
            var reviews = new List<string>();

            // Try to split by common delimiters
            var lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var cleaned = line.Trim().Trim('"', ',', '[', ']');
                if (!string.IsNullOrWhiteSpace(cleaned) && cleaned.Length > 10)
                {
                    reviews.Add(cleaned);
                }

                if (reviews.Count >= expectedCount)
                {
                    break;
                }
            }

            return reviews;
        }
    }
}
