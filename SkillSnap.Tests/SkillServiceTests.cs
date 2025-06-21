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
}