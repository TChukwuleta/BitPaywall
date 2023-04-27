namespace BitPaywall.Core.Entities
{
    public class PostAnalytic : AuditableEntity
    {
        public int PostId { get; set; }
        public int? ReadCount { get; set; }
        public decimal? AmountGenerated { get; set; }
        public string UserId { get; set; }
    }
}
