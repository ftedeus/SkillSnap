public class SkillServiceTests
{
    [Fact]
    public async Task AddSkillAsync_ShouldAddSkill_WhenNameIsUnique()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<SkillSnapContext>()
            .UseInMemoryDatabase(databaseName: $"AddSkillTest_{Guid.NewGuid()}")
            .Options;

        using var context = new SkillSnapContext(options);
        context.PortfolioUsers.Add(new PortfolioUser
        {
            Id = 1,
            Name = "Charlie",
            Bio = "Skill guy",
            ProfileImageUrl = "https://example.com/charlie.jpg"
        });
        await context.SaveChangesAsync();

        var mockValidator = new Mock<IPortfolioValidator>();
        mockValidator.Setup(v => v.SkillNameExistsAsync(1, "C#", null))
                     .ReturnsAsync(false);

        var service = new SkillService(context, mockValidator.Object);
        var dto = new SkillDto { Name = "C#", Level = "Expert" };

        // Act
        var result = await service.AddSkillAsync(1, dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("C#", result.Name);
    }
}