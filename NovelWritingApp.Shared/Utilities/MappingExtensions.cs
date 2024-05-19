using NovelWritingApp.Shared.Models;
using System.Linq;

namespace NovelWritingApp.Shared.Utilities
{
    public static class MappingExtensions
    {
        public static Novel MapToEntity(this NovelDTO dto)
        {
            return new Novel
            {
                NovelId = dto.NovelId,
                Title = dto.Title,
                CreationDate = dto.CreationDate,
                LastModified = dto.LastModified,
                Chapters = dto.Chapters.Select(MapToEntity).ToList(),
                Characters = dto.Characters.Select(MapToEntity).ToList()
            };
        }



        public static NovelDTO MapToDTO(this Novel entity)
        {
            return new NovelDTO
            {
                NovelId = entity.NovelId,
                Title = entity.Title,
                CreationDate = entity.CreationDate,
                LastModified = entity.LastModified,
                Chapters = entity.Chapters.Select(MapToDTO).ToList(),
                Characters = entity.Characters.Select(MapToDTO).ToList()
            };
        }

        public static Chapter MapToEntity(this ChapterDTO dto)
        {
            return new Chapter
            {
                ChapterId = dto.ChapterId,
                NovelId = dto.NovelId,
                Title = dto.Title,
                ChapSynopsis = dto.ChapSynopsis,
                ContentFilePath = dto.ContentFilePath
            };
        }

        public static ChapterDTO MapToDTO(this Chapter entity)
        {
            return new ChapterDTO
            {
                ChapterId = entity.ChapterId,
                NovelId = entity.NovelId,
                Title = entity.Title,
                ChapSynopsis = entity.ChapSynopsis,
                ContentFilePath = entity.ContentFilePath
            };
        }

        public static Character MapToEntity(this CharacterDTO dto)
        {
            return new Character
            {
                CharacterId = dto.CharacterId,
                NovelId = dto.NovelId,
                Name = dto.Name,
                Description = dto.Description,
                Motivations = dto.Motivations,
                CharSynopsis = dto.CharSynopsis
            };
        }

        public static CharacterDTO MapToDTO(this Character entity)
        {
            return new CharacterDTO
            {
                CharacterId = entity.CharacterId,
                NovelId = entity.NovelId,
                Name = entity.Name,
                Description = entity.Description,
                Motivations = entity.Motivations,
                CharSynopsis = entity.CharSynopsis
            };
        }
    }
}
