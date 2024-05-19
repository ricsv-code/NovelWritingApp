using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NovelWritingAPI.Data;
using NovelWritingApp.Shared.Models;
using System.IO;
using System.Linq;

namespace NovelWritingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NovelController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _environment;

        public NovelController(DataContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpGet]
        public async Task<ActionResult<List<NovelDTO>>> GetNovels()
        {
            var novels = await _context.Novels
                .Include(n => n.Chapters)
                .Include(n => n.Characters)
                .ToListAsync();

            var novelDTOs = novels.Select(n => new NovelDTO
            {
                NovelId = n.NovelId,
                Title = n.Title,
                CreationDate = n.CreationDate,
                LastModified = n.LastModified,
                Chapters = n.Chapters.Select(c => new ChapterDTO
                {
                    ChapterId = c.ChapterId,
                    NovelId = c.NovelId,
                    Title = c.Title,
                    ChapSynopsis = c.ChapSynopsis,
                    ContentFilePath = c.ContentFilePath
                }).ToList(),
                Characters = n.Characters.Select(c => new CharacterDTO
                {
                    CharacterId = c.CharacterId,
                    NovelId = c.NovelId,
                    Name = c.Name,
                    Description = c.Description,
                    Motivations = c.Motivations,
                    CharSynopsis = c.CharSynopsis
                }).ToList()
            }).ToList();

            return Ok(novelDTOs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NovelDTO>> GetNovel(int id)
        {
            var novel = await _context.Novels
                .Include(n => n.Chapters)
                .Include(n => n.Characters)
                .FirstOrDefaultAsync(n => n.NovelId == id);

            if (novel == null)
                return NotFound();

            var novelDTO = new NovelDTO
            {
                NovelId = novel.NovelId,
                Title = novel.Title,
                CreationDate = novel.CreationDate,
                LastModified = novel.LastModified,
                Chapters = novel.Chapters.Select(c => new ChapterDTO
                {
                    ChapterId = c.ChapterId,
                    NovelId = c.NovelId,
                    Title = c.Title,
                    ChapSynopsis = c.ChapSynopsis,
                    ContentFilePath = c.ContentFilePath
                }).ToList(),
                Characters = novel.Characters.Select(c => new CharacterDTO
                {
                    CharacterId = c.CharacterId,
                    NovelId = c.NovelId,
                    Name = c.Name,
                    Description = c.Description,
                    Motivations = c.Motivations,
                    CharSynopsis = c.CharSynopsis
                }).ToList()
            };

            return Ok(novelDTO);
        }

        [HttpPost]
        public async Task<ActionResult<Novel>> AddNovel(Novel novel)
        {
            _context.Novels.Add(novel);
            await _context.SaveChangesAsync();

            var initialChapter = new Chapter
            {
                NovelId = novel.NovelId,
                Title = "First Chapter",
                ChapSynopsis = "First Chapter Synopsis",
                ContentFilePath = $"ContentFiles/{novel.NovelId}/1/content.txt"
            };

            _context.Chapters.Add(initialChapter);
            await _context.SaveChangesAsync();

            // Ensure the directory exists and create the content file
            string novelDirectory = Path.Combine(_environment.ContentRootPath, "ContentFiles", novel.NovelId.ToString());
            string chapterDirectory = Path.Combine(novelDirectory, "1");
            string contentFilePath = Path.Combine(chapterDirectory, "content.txt");

            if (!Directory.Exists(chapterDirectory))
            {
                Directory.CreateDirectory(chapterDirectory);
            }

            await System.IO.File.WriteAllTextAsync(contentFilePath, "Chapter content");

            // Ensure ContentFilePath remains a relative path
            initialChapter.ContentFilePath = $"ContentFiles/{novel.NovelId}/1/content.txt";

            _context.Entry(initialChapter).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNovel), new { id = novel.NovelId }, novel);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNovel(int id, Novel novel)
        {
            if (id != novel.NovelId)
                return BadRequest();

            _context.Entry(novel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NovelExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNovel(int id)
        {
            var novel = await _context.Novels.FindAsync(id);
            if (novel == null)
                return NotFound();

            _context.Novels.Remove(novel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NovelExists(int id)
        {
            return _context.Novels.Any(e => e.NovelId == id);
        }
    }
}
