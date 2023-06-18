namespace BitPaywall.Core.Entities
{
    public class Comment : AuditableEntity
    {
        public int PostId { get; set; }
        public int ParentCommentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Text { get; set; }
        public string UserId { get; set; }
    }
}
