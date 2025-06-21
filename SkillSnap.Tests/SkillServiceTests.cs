public class SkillServiceTests : IClassFixture<SkillServiceFixture>
{
    private readonly SkillServiceFixture _fixture;

    public SkillServiceTests(SkillServiceFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task AddSkillAsync_ShouldAddSkill_WhenNameIsUnique()
    {
        using var context = _fixture.CreateNewContext();
        var user = _fixture.CreateUser(1, "Alex");
        context.PortfolioUsers.Add(user);
        await context.SaveChangesAsync();

        var validator = new Mock<IPortfolioValidator>();
        validator.Setup(v => v.SkillNameExistsAsync(1, "C#", null)).ReturnsAsync(false);

        var service = new SkillService(context, validator.Object);
        var dto = new SkillDto { Name = "C#", Level = "Expert" };

        var result = await service.AddSkillAsync(1, dto);

        Assert.NotNull(result);
        Assert.Equal("C#", result.Name);
    }

    [Fact]
    public async Task AddSkillAsync_ShouldThrow_WhenNameIsDuplicate()
    {
        using var context = _fixture.CreateNewContext();
        var user = _fixture.CreateUser(2);
        context.PortfolioUsers.Add(user);
        await context.SaveChangesAsync();

        var validator = new Mock<IPortfolioValidator>();
        validator.Setup(v => v.SkillNameExistsAsync(2, "React", null)).ReturnsAsync(true);

        var service = new SkillService(context, validator.Object);
        var dto = new SkillDto { Name = "React", Level = "Advanced" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.AddSkillAsync(2, dto));
    }

    [Fact]
    public async Task UpdateSkillAsync_ShouldReturnNull_WhenSkillNotFound()
    {
        using var context = _fixture.CreateNewContext();
        var validator = new Mock<IPortfolioValidator>();
        var service = new SkillService(context, validator.Object);

        var dto = new SkillDto { Name = "Swift", Level = "Advanced" };

        var result = await service.UpdateSkillAsync(42, 999, dto);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteSkillAsync_ShouldReturnFalse_WhenSkillNotFound()
    {
        using var context = _fixture.CreateNewContext();
        var validator = new Mock<IPortfolioValidator>();
        var service = new SkillService(context, validator.Object);

        var result = await service.DeleteSkillAsync(1, 1000);

        Assert.False(result);
    }

    [Fact]
    public async Task GetAllSkillsAsync_ShouldReturnAllSkills()
    {
        using var context = _fixture.CreateNewContext();

        var user1 = _fixture.CreateUser(1);
        var user2 = _fixture.CreateUser(2);

        user1.Skills!.Add(_fixture.CreateSkill(1, "C#"));
        user2.Skills!.Add(_fixture.CreateSkill(2, "SQL"));

        context.PortfolioUsers.AddRange(user1, user2);
        await context.SaveChangesAsync();

        var validator = new Mock<IPortfolioValidator>();
        var service = new SkillService(context, validator.Object);

        var result = await service.GetAllSkillsAsync();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, s => s.Name == "C#");
        Assert.Contains(result, s => s.Name == "SQL");
    }

    [Fact]
    public async Task GetSkillsByUserIdAsync_ShouldReturnUserSkills()
    {
        using var context = _fixture.CreateNewContext();

        var user = _fixture.CreateUser(1);
        user.Skills!.AddRange(new[]
        {
        _fixture.CreateSkill(1, "HTML"),
        _fixture.CreateSkill(2, "CSS")
    });

        context.PortfolioUsers.Add(user);
        await context.SaveChangesAsync();

        var validator = new Mock<IPortfolioValidator>();
        var service = new SkillService(context, validator.Object);

        var result = await service.GetSkillsByUserIdAsync(1);

        Assert.Equal(2, result.Count);
        Assert.All(result, s => Assert.Contains(s.Name, new[] { "HTML", "CSS" }));
    }

    [Fact]
    public async Task GetSkillByIdAsync_ShouldReturnSkill_WhenExists()
    {
        using var context = _fixture.CreateNewContext();

        var user = _fixture.CreateUser(1);
        var skill = _fixture.CreateSkill(1, "JavaScript");
        user.Skills!.Add(skill);

        context.PortfolioUsers.Add(user);
        await context.SaveChangesAsync();

        var validator = new Mock<IPortfolioValidator>();
        var service = new SkillService(context, validator.Object);

        var result = await service.GetSkillByIdAsync(1, 1);

        Assert.NotNull(result);
        Assert.Equal("JavaScript", result!.Name);
    }

    [Fact]
    public async Task GetAllSkillsAsync_ShouldReturnEmptyList_WhenNoSkillsExist()
    {
        using var context = _fixture.CreateNewContext();
        var validator = new Mock<IPortfolioValidator>();
        var service = new SkillService(context, validator.Object);

        var result = await service.GetAllSkillsAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetSkillsByUserIdAsync_ShouldReturnEmptyList_WhenUserHasNoSkills()
    {
        using var context = _fixture.CreateNewContext();
        var user = _fixture.CreateUser(1); // No skills added
        context.PortfolioUsers.Add(user);
        await context.SaveChangesAsync();

        var validator = new Mock<IPortfolioValidator>();
        var service = new SkillService(context, validator.Object);

        var result = await service.GetSkillsByUserIdAsync(1);

        Assert.NotNull(result);
        Assert.Empty(result);
    }
[Fact]
public async Task GetSkillByIdAsync_ShouldReturnNull_WhenSkillNotFound()
{
    using var context = _fixture.CreateNewContext();
    var user = _fixture.CreateUser(1);
    context.PortfolioUsers.Add(user);
    await context.SaveChangesAsync();

    var validator = new Mock<IPortfolioValidator>();
    var service = new SkillService(context, validator.Object);

    var result = await service.GetSkillByIdAsync(1, 404); // Skill ID 404 doesn't exist

    Assert.Null(result);
}
}