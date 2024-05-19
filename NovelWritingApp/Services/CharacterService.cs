using NovelWritingApp.Shared.Models;
using System.Net.Http.Json;

namespace NovelWritingApp.Services
{
    public class CharacterService
    {
        private readonly HttpClient _httpClient;

        public CharacterService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<CharacterDTO>> GetCharactersAsync(int novelId)
        {
            return await _httpClient.GetFromJsonAsync<List<CharacterDTO>>($"api/character/novel/{novelId}");
        }

        public async Task<CharacterDTO> GetCharacterAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<CharacterDTO>($"api/character/{id}");
        }

        public async Task<CharacterDTO> CreateCharacterAsync(CharacterDTO characterDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/character", characterDto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CharacterDTO>();
        }



        public async Task UpdateCharacterAsync(CharacterDTO characterDto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/character/{characterDto.CharacterId}", characterDto);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error updating character");
            }
        }


        public async Task DeleteCharacterAsync(int id)
        {
            await _httpClient.DeleteAsync($"api/character/{id}");
        }
    }
}
