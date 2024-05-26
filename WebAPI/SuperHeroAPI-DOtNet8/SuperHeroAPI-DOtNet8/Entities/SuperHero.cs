namespace SuperHeroAPI_DOtNet8.Entities
{
    public class SuperHero
    {
        // 創建四個屬性，分別是 Idm Name, FirstName, LastName, Place
        public int Id { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Place { get; set; } = string.Empty;
    }
}
