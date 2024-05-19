namespace NovelWritingApp.Shared.Models
{
    public class Novel
    {
        public int NovelId { get; set; }
        public string Title { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModified { get; set; }
        public List<Chapter> Chapters { get; set; } = new List<Chapter>();
        public List<Character> Characters { get; set; } = new List<Character>();
    }
}
