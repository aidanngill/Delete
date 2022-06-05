namespace Delete.Api
{
    internal enum RelationshipType
    {
        NONE = 0,
        NORMAL = 1,
        BLOCKED = 2,
        PENDING_INCOMING = 3,
        PENDING_OUTGOING = 4,
    }
    internal class Relationship
    {
        public string ID { get; set; }
        public RelationshipType Type { get; set; }
        public string Nickname { get; set; }
        public User User { get; set; }
    }
}
