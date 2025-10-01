using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Windows.Media;
using Mscc.GenerativeAI;

namespace NoteApp_DoAn.View
{

    public class NoteView
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public SolidColorBrush Background { get; set; }

        public bool IsActive { get; set; }

        public string TitlePreview => string.IsNullOrWhiteSpace(Title) ? "(Không có tiêu đề)" : Title;
        public string PreviewContent => Content.Length > 80 ? Content.Substring(0, 80) + "..." : Content;
        public string DateString => UpdatedAt.ToString("g");

        public string SummarizeString { get; set; }

        public async Task InitializeAsync()
        {
            SummarizeString = await FetchValueFromApiAsync();
        }

        public async Task<string> FetchValueFromApiAsync()
        {
            var apiCaller = new GeminiApiCaller();
            string prompt = $"Tóm tắt nội dung sau trong số lượng từ ít hơn đầu vào, nếu không thể thì mô tả nó trong 1 hoặc 2 câu:\n{Content}\n Lưu ý: Chỉ trả về văn bản tóm tắt không trả về thêm bất cứ văn bản nào ngoài nó.";
            string response = await apiCaller.GenerateContentAsync(prompt);
            return response;
        }
    }

    public class Part
    {
        public string text { get; set; }
    }

    public class Content
    {
        public List<Part> parts { get; set; }
    }

    public class GeminiRequest
    {
        public List<Content> contents { get; set; }
        // Có thể thêm GenerationConfig, SafetySettings... tại đây
    }

    public class Candidate
    {
        public Content content { get; set; }
        // Thường lấy text từ content.parts[0].text
    }

    public class GeminiResponse
    {
        public List<Candidate> candidates { get; set; }
    }

    public class GeminiApiCaller
    {
        private const string ApiKey = "API-KEY"; 
        private const string Model = "gemini-2.5-flash";
        private static readonly HttpClient client = new HttpClient();

        public async Task<string> GenerateContentAsync(string prompt)
        {
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/{Model}:generateContent?key={ApiKey}";

            var requestBody = new GeminiRequest
            {
                contents = new List<Content>
            {
                new Content
                {
                    parts = new List<Part> { new Part { text = prompt } }
                }
            }
            };

            string jsonRequest = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync(url, content);
                string jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var geminiResponse = JsonConvert.DeserializeObject<GeminiResponse>(jsonResponse);

                    if (geminiResponse?.candidates?.Count > 0)
                    {
                        return geminiResponse.candidates[0].content.parts[0].text;
                    }
                    return "Không tìm thấy nội dung phản hồi.";
                }
                else
                {
                    return $"Lỗi API: {response.StatusCode}. Chi tiết: {jsonResponse}";
                }
            }
            catch (Exception ex)
            {
                return $"Lỗi ngoại lệ: {ex.Message}";
            }
        }
    }
}
