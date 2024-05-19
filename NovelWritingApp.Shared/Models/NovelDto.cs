namespace NovelWritingApp.Shared.Models
{
    public class NovelDTO
    {
        public int NovelId { get; set; }
        public string Title { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModified { get; set; }
        public List<ChapterDTO> Chapters { get; set; } = new List<ChapterDTO>();
        public List<CharacterDTO> Characters { get; set; } = new List<CharacterDTO>();
    }
}
