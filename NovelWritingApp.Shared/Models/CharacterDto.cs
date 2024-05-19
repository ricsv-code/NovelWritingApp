namespace NovelWritingApp.Shared.Models
{
    public class CharacterDTO
    {
        public int CharacterId { get; set; }
        public int NovelId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Motivations { get; set; }
        public string CharSynopsis { get; set; }
    }
}
