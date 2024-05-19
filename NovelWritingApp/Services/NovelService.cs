using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using NovelWritingApp.Shared.Models;
using NovelWritingApp.Shared.Utilities;

namespace NovelWritingApp.Services
{
    public class NovelService
    {
        private readonly HttpClient _httpClient;

        public NovelService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<NovelDTO>> GetNovelsAsync()
        {
            var response = await _httpClient.GetAsync("api/novel");
            response.EnsureSuccessStatusCode();

            var novels = await response.Content.ReadFromJsonAsync<List<NovelDTO>>();
            if (novels == null)
            {
                throw new InvalidOperationException("Failed to retrieve novels.");
            }

            return novels;
        }

        public async Task<NovelDTO> GetNovelByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<NovelDTO>($"api/novel/{id}");
        }

        public async Task<Novel> AddNovelAsync(NovelDTO novelDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/novel", novelDto);
            if (response.IsSuccessStatusCode)
            {
                var createdNovelDto = await response.Content.ReadFromJsonAsync<NovelDTO>();
                return createdNovelDto.MapToEntity();
            }
            else
            {
                throw new Exception("Error creating novel");
            }
        }


        public async Task UpdateNovelAsync(NovelDTO novel)
        {
            if (novel.NovelId > 0)
            {
                var response = await _httpClient.PutAsJsonAsync($"api/novel/{novel.NovelId}", novel);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error updating novel: {errorContent}");
                    throw new HttpRequestException($"Error updating novel: {response.StatusCode}");
                }
            }
            else
            {
                Console.WriteLine("Invalid NovelId.");
            }
        }

        public async Task DeleteNovelAsync(int id)
        {
            await _httpClient.DeleteAsync($"api/novel/{id}");
        }


    }
}
