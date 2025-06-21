using Microsoft.Extensions.Logging;

public class PortfolioUserServiceTests : IClassFixture<PortfolioUserServiceFixture>
{
    private readonly PortfolioUserServiceFixture _fixture;

    public PortfolioUserServiceTests(PortfolioUserServiceFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateAsync_ShouldAddUser_WhenValid()
    {
        using var context = _fixture.CreateNewContext();

        var validator = new Mock<IPortfolioValidator>();
        var logger = new Mock<ILogger<PortfolioUserService>>();

        var service = new PortfolioUserService(context, validator.Object, logger.Object);
        var dto = _fixture.CreateTestDto("Charlie");

        var result = await service.CreateAsync(dto);

        Assert.NotNull(result);
        Assert.Equal("Charlie", result.Name);
        Assert.Single(context.PortfolioUsers);
    }


    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser_WhenExists()
    {
        using var context = _fixture.CreateNewContext();
        var validator = new Mock<IPortfolioValidator>();
        var logger = new Mock<ILogger<PortfolioUserService>>();
        var user = _fixture.CreateTestUser(1, "Nina");
        context.PortfolioUsers.Add(user);
        await context.SaveChangesAsync();

        // var service = new PortfolioUserService(context);
        var service = new PortfolioUserService(context, validator.Object, logger.Object);

        var result = await service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Nina", result.Name);
    }


    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        using var context = _fixture.CreateNewContext();
        var validator = new Mock<IPortfolioValidator>();
        var logger = new Mock<ILogger<PortfolioUserService>>();



        var service = new PortfolioUserService(context, validator.Object, logger.Object);
        var result = await service.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUser_WhenValid()
    {
        using var context = _fixture.CreateNewContext();
        var validator = new Mock<IPortfolioValidator>();
        var logger = new Mock<ILogger<PortfolioUserService>>();


        var user = _fixture.CreateTestUser(1, "Alex");
        context.PortfolioUsers.Add(user);
        await context.SaveChangesAsync();

        var dto = new PortfolioUserDto { Name = "Alexander", Bio = "Updated bio" };
        var service = new PortfolioUserService(context, validator.Object, logger.Object);
        var result = await service.UpdateAsync(1, dto);

        // Assert.NotNull(result);
        // Assert.Equal("Alexander", result.Name);
        // Assert.Equal("Updated bio", result.Bio);
        Assert.True(result);

        var updated = await context.PortfolioUsers.FindAsync(1);
        Assert.NotNull(updated);
        Assert.Equal("Alexander", updated.Name);
        Assert.Equal("Updated bio", updated.Bio);

    }


    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        using var context = _fixture.CreateNewContext();
        var validator = new Mock<IPortfolioValidator>();
        var logger = new Mock<ILogger<PortfolioUserService>>();
        var dto = new PortfolioUserDto { Name = "Nonexistent User1" };
        var service = new PortfolioUserService(context, validator.Object, logger.Object);

        var result = await service.UpdateAsync(999, dto);


        Assert.False(result);

    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveUser_WhenExists()
    {
        using var context = _fixture.CreateNewContext();
        var validator = new Mock<IPortfolioValidator>();
        var logger = new Mock<ILogger<PortfolioUserService>>();
        var user = _fixture.CreateTestUser(1, "Bob");
        context.PortfolioUsers.Add(user);
        await context.SaveChangesAsync();

        var service = new PortfolioUserService(context, validator.Object, logger.Object);
        var result = await service.DeleteAsync(1);

        Assert.True(result);
        Assert.Empty(context.PortfolioUsers);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenUserDoesNotExist()
    {
        using var context = _fixture.CreateNewContext();
        var validator = new Mock<IPortfolioValidator>();
        var logger = new Mock<ILogger<PortfolioUserService>>();
        var service = new PortfolioUserService(context, validator.Object, logger.Object);

        var result = await service.DeleteAsync(999);

        Assert.False(result);
    }
}