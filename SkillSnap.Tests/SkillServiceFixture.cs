public class SkillServiceFixture
{
    public SkillSnapContext CreateNewContext()
    {
        var options = new DbContextOptionsBuilder<SkillSnapContext>()
            .UseInMemoryDatabase($"SkillSnapDb_{Guid.NewGuid()}")
            .Options;

        return new SkillSnapContext(options);
    }

    public PortfolioUser CreateUser(int id = 1, string name = "Test User")
    {
        return new PortfolioUser
        {
            Id = id,
            Name = name,
            Bio = "Sample bio",
            ProfileImageUrl = "https://example.com/user.jpg",
            Skills = new List<Skill>()
        };
    }

    public Skill CreateSkill(int id, string name, string level = "Intermediate")
    {
        return new Skill
        {
            Id = id,
            Name = name,
            Level = level
        };
    }
}