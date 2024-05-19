namespace NovelWritingApp.Shared.Models
{
    public class ChapterDTO
    {
        public int ChapterId { get; set; }
        public int NovelId { get; set; }
        public string? Title { get; set; }
        public string? ChapSynopsis { get; set; }
        public string? ContentFilePath { get; set; }
    }
}
