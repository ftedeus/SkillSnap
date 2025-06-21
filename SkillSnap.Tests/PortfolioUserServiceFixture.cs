public class PortfolioUserServiceFixture
{
    public SkillSnapContext CreateNewContext()
    {
        var options = new DbContextOptionsBuilder<SkillSnapContext>()
            .UseInMemoryDatabase($"PortfolioUserTestDb_{Guid.NewGuid()}")
            .Options;

        return new SkillSnapContext(options);
    }

    public PortfolioUserDto CreateTestDto(string name = "Test User") => new PortfolioUserDto
    {
        Name = name,
        Bio = "Test bio",
        ProfileImageUrl = "https://example.com/profile.jpg"
    };

    public PortfolioUser CreateTestUser(int id = 1, string name = "Test User") => new PortfolioUser
    {
        Id = id,
        Name = name,
        Bio = "Real user",
        ProfileImageUrl = "https://example.com/user.jpg"
    };
}