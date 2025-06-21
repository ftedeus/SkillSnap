namespace SkillSnap.Tests;

public class ProjectServiceTests : IClassFixture<ProjectServiceFixture>
{
    private readonly SkillSnapContext _context;
    private readonly Mock<IPortfolioValidator> _validator;
    private readonly ProjectServiceFixture _fixture;


    public ProjectServiceTests(ProjectServiceFixture fixture)
    {
        _fixture = fixture;
        _context = fixture.Context;
        _validator = fixture.ValidatorMock;

    }
    [Fact]
    public async Task AddProjectAsync_ShouldAddProject_WhenTitleIsUnique()
    {
        //Arrange
        _context.PortfolioUsers.RemoveRange(_context.PortfolioUsers);
        _context.Projects.RemoveRange(_context.Projects);
        await _context.SaveChangesAsync();


        var user = new PortfolioUser { Id = 1, Name = "Nina", Bio = "Test", ProfileImageUrl = "url" };
        _context.PortfolioUsers.Add(user);
        await _context.SaveChangesAsync();

        _validator.Reset();

        _validator.Setup(v => v.ProjectTitleExistsAsync(1, "My Project", null)).ReturnsAsync(false);

        var service = new ProjectService(_context, _validator.Object);
        var dto = new ProjectDto { Title = "My Project", Description = "Yes", ImageUrl = "image" };

        var result = await service.AddProjectAsync(1, dto);

        Assert.NotNull(result);
        Assert.Equal("My Project", result.Title);
    }
    [Fact]
    public async Task AddProjectAsync_ShouldThrow_WhenTitleIsDuplicate()
    {
        using var context = _fixture.CreateNewContext();
        var user = _fixture.CreateUser(2);
        context.PortfolioUsers.Add(user);
        await context.SaveChangesAsync();

        var validator = new Mock<IPortfolioValidator>();
        validator.Setup(v => v.ProjectTitleExistsAsync(2, "My Project", null)).ReturnsAsync(true);

        var service = new ProjectService(context, validator.Object);
        var dto = new ProjectDto
        {
            Title = "My Project",
            Description = "Shouldn't succeed",
            ImageUrl = "boom.jpg"
        };
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.AddProjectAsync(2, dto));
    }
    [Fact]
    public async Task UpdateProjectAsync_ShouldUpdateProject_WhenValid()
    {


        using var context = _fixture.CreateNewContext();


        var user = new PortfolioUser
        {
            Id = 1,
            Name = "Eve",
            Bio = "Updater",
            ProfileImageUrl = "https://example.com/eve.jpg",
            Projects = new List<Project>
        {
            new Project { Id = 10, Title = "Old Title", Description = "Old Desc", ImageUrl = "old.jpg" }
        }
        };
        context.PortfolioUsers.Add(user);
        await context.SaveChangesAsync();


        _validator.Setup(v => v.ProjectTitleExistsAsync(1, "New Title", 10)).ReturnsAsync(false);

        var service = new ProjectService(context, _validator.Object);
        var dto = new ProjectDto { Title = "New Title", Description = "Updated Desc", ImageUrl = "new.jpg" };

        // Act
        var result = await service.UpdateProjectAsync(1, 10, dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Title", result.Title);
        Assert.Equal("Updated Desc", result.Description);
    }

    [Fact]
    public async Task DeleteProjectAsync_ShouldDeleteProject_WhenExists()
    {
        // Arrange
        using var context = _fixture.CreateNewContext();

        var user = new PortfolioUser
        {
            Id = 2,
            Name = "Frank",
            Bio = "Deleter",
            ProfileImageUrl = "https://example.com/frank.jpg",
            Projects = new List<Project>
        {
            new Project { Id = 20, Title = "To Delete", Description = "Gone soon", ImageUrl = "del.jpg" }
        }
        };
        context.PortfolioUsers.Add(user);
        await context.SaveChangesAsync();

        var validator = new Mock<IPortfolioValidator>();
        var service = new ProjectService(context, validator.Object);

        // Act
        var result = await service.DeleteProjectAsync(2, 20);

        // Assert
        Assert.True(result);
        Assert.Empty(context.Projects.Where(p => p.Id == 20));
    }



    [Fact]
    public async Task DeleteProjectAsync_ShouldReturnFalse_WhenProjectDoesNotExist()
    {
        // Arrange
        using var context = _fixture.CreateNewContext(); // Fresh DB with no seeded projects
        var validator = new Mock<IPortfolioValidator>(); // No validation needed for this test

        var service = new ProjectService(context, validator.Object);

        // Act
        var result = await service.DeleteProjectAsync(userId: 42, projectId: 999); // IDs that do not exist

        // Assert
        Assert.False(result); // Should return false when nothing is deleted
    }

    [Fact]
    public async Task UpdateProjectAsync_ShouldReturnNull_WhenProjectNotFound()
    {
        // Arrange
        using var context = _fixture.CreateNewContext(); // Fresh in-memory DB
        var validator = new Mock<IPortfolioValidator>(); // Fresh mock to avoid conflicts

        var service = new ProjectService(context, validator.Object);
        var updateDto = new ProjectDto
        {
            Title = "Nonexistent Project",
            Description = "No update here",
            ImageUrl = "https://example.com/ghost.jpg"
        };

        // Act
        var result = await service.UpdateProjectAsync(userId: 99, projectId: 999, updateDto);

        // Assert
        Assert.Null(result);
    }
}