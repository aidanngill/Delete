namespace Delete.Api
{
    public class User
    {
        public string ID { get; set; }
        public string Username { get; set; }
        public string Discriminator { get; set; }
        public string Avatar { get; set; }
        public override string ToString()
        {
            return $"{Username}#{Discriminator}";
        }
    }
}
