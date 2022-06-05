namespace Delete.Api
{
    internal class Guild
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string OwnerID { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
