using Microsoft.AspNetCore.Mvc;
using NovelWritingApp.Shared.Models;
using NovelWritingAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace NovelWritingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CharacterController : ControllerBase
    {
        private readonly DataContext _context;

        public CharacterController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("novel/{novelId}")]
        public async Task<ActionResult<List<CharacterDTO>>> GetCharacters(int novelId)
        {
            var characters = await _context.Characters.Where(c => c.NovelId == novelId).ToListAsync();

            var characterDTOs = characters.Select(c => new CharacterDTO
            {
                CharacterId = c.CharacterId,
                NovelId = c.NovelId,
                Name = c.Name,
                Description = c.Description,
                Motivations = c.Motivations,
                CharSynopsis = c.CharSynopsis
            }).ToList();

            return Ok(characterDTOs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CharacterDTO>> GetCharacter(int id)
        {
            var character = await _context.Characters.FindAsync(id);

            if (character == null)
            {
                return NotFound();
            }

            // Map the entity to DTO
            var characterDto = new CharacterDTO
            {
                CharacterId = character.CharacterId,
                NovelId = character.NovelId,
                Name = character.Name,
                Description = character.Description,
                Motivations = character.Motivations,
                CharSynopsis = character.CharSynopsis
            };

            return characterDto;
        }

        [HttpPost]
        public async Task<ActionResult<CharacterDTO>> CreateCharacter(CharacterDTO characterDto)
        {
            if (characterDto == null || characterDto.NovelId <= 0)
            {
                Console.WriteLine("API: Invalid character data received.");
                return BadRequest("Invalid character data.");
            }

            Console.WriteLine($"API: Creating character for NovelId {characterDto.NovelId}");
            var characterCount = await _context.Characters.CountAsync(c => c.NovelId == characterDto.NovelId);

            var newCharacter = new Character
            {
                NovelId = characterDto.NovelId,
                Name = $"New Character {characterCount + 1}",
                Description = "Character description",
                Motivations = "Character motivations",
                CharSynopsis = "Character synopsis"
            };

            _context.Characters.Add(newCharacter);
            await _context.SaveChangesAsync();

            // Map the entity to DTO
            var createdCharacterDto = new CharacterDTO
            {
                CharacterId = newCharacter.CharacterId,
                NovelId = newCharacter.NovelId,
                Name = newCharacter.Name,
                Description = newCharacter.Description,
                Motivations = newCharacter.Motivations,
                CharSynopsis = newCharacter.CharSynopsis
            };

            Console.WriteLine($"API: Created character with ID {createdCharacterDto.CharacterId}");
            return CreatedAtAction(nameof(GetCharacter), new { id = newCharacter.CharacterId }, createdCharacterDto);
        }




        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCharacter(int id, Character character)
        {
            if (id != character.CharacterId)
            {
                return BadRequest();
            }

            _context.Entry(character).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CharacterExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCharacter(int id)
        {
            var character = await _context.Characters.FindAsync(id);
            if (character == null)
            {
                return NotFound();
            }

            _context.Characters.Remove(character);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool CharacterExists(int id)
        {
            return _context.Characters.Any(e => e.CharacterId == id);
        }
    }
}
