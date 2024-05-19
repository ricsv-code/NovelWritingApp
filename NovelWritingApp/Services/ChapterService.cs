using NovelWritingApp.Shared.Models;
using NovelWritingApp.Shared.Utilities; // Add this for mapping extensions
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace NovelWritingApp.Services
{
    public class ChapterService
    {
        private readonly HttpClient _httpClient;

        public ChapterService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ChapterDTO> GetChapterAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<ChapterDTO>($"api/chapter/{id}");
        }

        public async Task<string> GetChapterContentAsync(string contentFilePath)
        {
            var baseUrl = "https://localhost:7292/";
            var fullUrl = $"{baseUrl}{contentFilePath}";

            var response = await _httpClient.GetAsync(fullUrl);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return string.Empty;
        }



        public async Task UpdateChapterContentAsync(int chapterId, string content)
        {
            var chapter = await GetChapterAsync(chapterId);
            if (chapter != null)
            {
                // Get all chapters of the novel to determine the chapter number within the novel
                var chapters = await _httpClient.GetFromJsonAsync<List<ChapterDTO>>($"api/chapter/novel/{chapter.NovelId}");
                var chapterIndex = chapters.FindIndex(c => c.ChapterId == chapterId) + 1; // +1 to make it 1-based index

                if (chapterIndex > 0)
                {
                    var contentFilePath = $"ContentFiles/{chapter.NovelId}/{chapterIndex}/content.txt";

                    var contentUpdateModel = new ChapterContentUpdateModel
                    {
                        ChapterId = chapterId,
                        Content = content,
                        ContentFilePath = contentFilePath // Use relative path
                    };

                    await _httpClient.PutAsJsonAsync($"api/chapter/{chapterId}/content", contentUpdateModel);
                }
                else
                {
                    Console.WriteLine($"Chapter with ID {chapterId} not found in the list of chapters for novel {chapter.NovelId}.");
                }
            }
            else
            {
                Console.WriteLine($"Chapter with ID {chapterId} not found.");
            }
        }




        public async Task<ChapterDTO> CreateChapterAsync(ChapterDTO chapterDto)
        {
            try
            {
                Console.WriteLine($"Sending ChapterDTO: {JsonConvert.SerializeObject(chapterDto)}");
                var response = await _httpClient.PostAsJsonAsync("api/chapter", chapterDto);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error response: {errorContent}");
                }

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<ChapterDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception creating chapter: {ex.Message}");
                throw;
            }
        }





        public async Task UpdateChapterAsync(ChapterDTO chapterDto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/chapter/{chapterDto.ChapterId}", chapterDto);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error updating chapter");
            }
        }


        public async Task DeleteChapterAsync(int id)
        {
            await _httpClient.DeleteAsync($"api/chapter/{id}");
        }
    }
}
