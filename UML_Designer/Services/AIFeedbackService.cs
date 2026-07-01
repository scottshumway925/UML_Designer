using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;

namespace UML_Designer.Services
{
   public class AIFeedbackService
   {
      private readonly string _apiKey;
      private readonly HttpClient _client;

      public AIFeedbackService(string apiKey)
      {
         _apiKey = apiKey;
         _client = new HttpClient();
         _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
         _client.DefaultRequestHeaders.Add("HTTP-Referer", "UML-Designer");
      }

      public async Task<string> GetFeedback(string prompt)
      {
         var requestBody = new
         {
            // backup model: cohere/north-mini-code:free
            model = "nvidia/nemotron-3-ultra-550b-a55b:free",
            messages = new[]
            {
               new { role = "user", content = prompt }
            }
         };

         var json = JsonSerializer.Serialize(requestBody);
         var content = new StringContent(json, Encoding.UTF8, "application/json");
         try
         {
            _client.Timeout = TimeSpan.FromMinutes(5);
         } catch (Exception)
         {

         }
         
         var response = await _client.PostAsync("https://openrouter.ai/api/v1/chat/completions", content);

         var responseJson = await response.Content.ReadAsStringAsync();

         System.Diagnostics.Debug.WriteLine($"Status: {response.StatusCode}");
         System.Diagnostics.Debug.WriteLine($"Response: {responseJson}");

         using var doc = JsonDocument.Parse(responseJson);

         var root = doc.RootElement;

         // Check if the response contains an error
         if (root.TryGetProperty("error", out var error))
         {
            var message = error.GetProperty("message").GetString();
            return $"API Error: {message}";
         }

         return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "No Response Received";
      }
   }
}
