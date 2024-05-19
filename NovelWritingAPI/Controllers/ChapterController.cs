using Microsoft.AspNetCore.Mvc;
using NovelWritingApp.Shared.Models;
using NovelWritingAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace NovelWritingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChapterController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _environment;

        public ChapterController(DataContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpGet("novel/{novelId}")]
        public async Task<ActionResult<List<ChapterDTO>>> GetChapters(int novelId)
        {
            var chapters = await _context.Chapters.Where(c => c.NovelId == novelId).ToListAsync();

            var chapterDTOs = chapters.Select(c => new ChapterDTO
            {
                ChapterId = c.ChapterId,
                NovelId = c.NovelId,
                Title = c.Title,
                ChapSynopsis = c.ChapSynopsis,
                ContentFilePath = c.ContentFilePath
            }).ToList();

            return Ok(chapterDTOs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ChapterDTO>> GetChapter(int id)
        {
            var chapter = await _context.Chapters.FindAsync(id);

            if (chapter == null)
            {
                return NotFound();
            }

            // Map the entity to DTO
            var chapterDto = new ChapterDTO
            {
                ChapterId = chapter.ChapterId,
                NovelId = chapter.NovelId,
                Title = chapter.Title,
                ChapSynopsis = chapter.ChapSynopsis,
                ContentFilePath = chapter.ContentFilePath
            };

            return chapterDto;
        }

        [HttpPost]
        public async Task<ActionResult<ChapterDTO>> CreateChapter(ChapterDTO chapterDto)
        {
            // Log the incoming ChapterDTO
            Console.WriteLine($"Received ChapterDTO: {JsonConvert.SerializeObject(chapterDto)}");

            if (chapterDto == null || chapterDto.NovelId <= 0)
            {
                Console.WriteLine("Invalid chapter data received.");
                return BadRequest("Invalid chapter data.");
            }

            try
            {
                var chapterCount = await _context.Chapters.CountAsync(c => c.NovelId == chapterDto.NovelId);
                Console.WriteLine($"Chapter count for NovelId {chapterDto.NovelId}: {chapterCount}");

                var newChapter = new Chapter
                {
                    NovelId = chapterDto.NovelId,
                    Title = $"New Chapter {chapterCount + 1}",
                    ChapSynopsis = "Synopsis",
                    ContentFilePath = $"ContentFiles/{chapterDto.NovelId}/{chapterCount + 1}/content.txt"
                };

                _context.Chapters.Add(newChapter);
                await _context.SaveChangesAsync();

                // Ensure the directory exists and create the empty content file
                string contentFilePath = Path.Combine(_environment.ContentRootPath, newChapter.ContentFilePath);
                string chapterDirectory = Path.GetDirectoryName(contentFilePath);

                if (!Directory.Exists(chapterDirectory))
                {
                    Directory.CreateDirectory(chapterDirectory);
                }

                await System.IO.File.WriteAllTextAsync(contentFilePath, string.Empty);

                // Map the entity to DTO
                var createdChapterDto = new ChapterDTO
                {
                    ChapterId = newChapter.ChapterId,
                    NovelId = newChapter.NovelId,
                    Title = newChapter.Title,
                    ChapSynopsis = newChapter.ChapSynopsis,
                    ContentFilePath = newChapter.ContentFilePath
                };

                Console.WriteLine($"Created ChapterDTO: {JsonConvert.SerializeObject(createdChapterDto)}");

                return CreatedAtAction(nameof(GetChapter), new { id = newChapter.ChapterId }, createdChapterDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating chapter: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }






        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateChapter(int id, Chapter chapter)
        {
            if (id != chapter.ChapterId)
            {
                return BadRequest();
            }

            _context.Entry(chapter).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChapterExists(id))
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

        [HttpPut("{chapterId}/content")]
        public async Task<IActionResult> UpdateChapterContent(int chapterId, [FromBody] ChapterContentUpdateModel updateModel)
        {
            if (chapterId != updateModel.ChapterId)
            {
                return BadRequest();
            }

            // Resolve the full path on the server-side
            var fullContentFilePath = Path.Combine(_environment.ContentRootPath, updateModel.ContentFilePath);

            // Ensure the directory exists
            var directoryPath = Path.GetDirectoryName(fullContentFilePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Write content to the file
            await System.IO.File.WriteAllTextAsync(fullContentFilePath, updateModel.Content);

            return NoContent();
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChapter(int id)
        {
            var chapter = await _context.Chapters.FindAsync(id);
            if (chapter == null)
            {
                return NotFound();
            }

            _context.Chapters.Remove(chapter);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool ChapterExists(int id)
        {
            return _context.Chapters.Any(e => e.ChapterId == id);
        }
    }
}
