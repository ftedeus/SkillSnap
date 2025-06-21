public class ProjectServiceFixture
{
    public SkillSnapContext Context { get; }
    public Mock<IPortfolioValidator> ValidatorMock { get; }

    public ProjectServiceFixture()
    {
        var options = new DbContextOptionsBuilder<SkillSnapContext>()
            .UseInMemoryDatabase($"SkillSnapTest_{Guid.NewGuid()}")
            .Options;

        Context = new SkillSnapContext(options);
        ValidatorMock = new Mock<IPortfolioValidator>();
    }
    public SkillSnapContext CreateNewContext()
    {
        var options = new DbContextOptionsBuilder<SkillSnapContext>()
            .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}") // unique per test
            .Options;

        return new SkillSnapContext(options);
    }

 
public PortfolioUser CreateUser(int id = 1, string name = "User")
{
    return new PortfolioUser
    {
        Id = id,
        Name = name,
        Bio = "Test user",
        ProfileImageUrl = "https://example.com/user.jpg",
        Projects = new List<Project>()
    };
}

    public Project CreateProject(int id, string title) => new Project
    {
        Id = id,
        Title = title,
        Description = "Sample Desc",
        ImageUrl = $"https://example.com/{title}.jpg"
    };
}