namespace NovelWritingApp.Shared.Models
{
    public class ChapterContentUpdateModel
    {
        public int ChapterId { get; set; }
        public string Content { get; set; }
        public string ContentFilePath { get; set; }
    }
}