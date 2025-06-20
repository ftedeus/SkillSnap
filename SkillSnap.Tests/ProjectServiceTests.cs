namespace SkillSnap.Tests;

public class ProjectServiceTests : TestBase

{


    [Fact]
    public async Task AddProjectAsync_ShouldAddProject_WhenTitleIsUnique()
    {
        using var context = CreateInMemoryContext();
        var user = CreateTestUser(1, "Alice");
        context.PortfolioUsers.Add(user);
        await context.SaveChangesAsync();

        var validator = CreateValidatorMock();
        validator.Setup(v => v.ProjectTitleExistsAsync(1, "New Project", null)).ReturnsAsync(false);

        var service = new ProjectService(context, validator.Object);
        var dto = new ProjectDto { Title = "New Project", Description = "Cool one", ImageUrl = "url" };

        var result = await service.AddProjectAsync(1, dto);

        Assert.NotNull(result);
        Assert.Equal("New Project", result.Title);
    }




    [Fact]
    public async Task AddProjectAsync_ShouldThrow_WhenTitleIsDuplicate()
    {
        // Arrange
        using var context = CreateInMemoryContext();

        var user = CreateTestUser(1, "Sophie");
        context.PortfolioUsers.Add(user);
        await context.SaveChangesAsync();

        var mockValidator = CreateValidatorMock();
        mockValidator.Setup(v => v.ProjectTitleExistsAsync(1, "My Project", null))
                     .ReturnsAsync(true); // Simulate duplicate

        var service = new ProjectService(context, mockValidator.Object);
        var dto = new ProjectDto
        {
            Title = "My Project",
            Description = "This shouldn't be added",
            ImageUrl = "https://example.com/collision.jpg"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.AddProjectAsync(1, dto));
    }


    [Fact]
    public async Task UpdateProjectAsync_ShouldUpdateProject_WhenValid()
    {

        using var context = CreateInMemoryContext();



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

        //var mockValidator = new Mock<IPortfolioValidator>();
        var mockValidator = CreateValidatorMock();
        mockValidator.Setup(v => v.ProjectTitleExistsAsync(1, "New Title", 10))
                     .ReturnsAsync(false);

        var service = new ProjectService(context, mockValidator.Object);
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

        using var context = CreateInMemoryContext();


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

        //  var mockValidator = new Mock<IPortfolioValidator>();
        var mockValidator = CreateValidatorMock();
        var service = new ProjectService(context, mockValidator.Object);

        // Act
        var result = await service.DeleteProjectAsync(2, 20);

        // Assert
        Assert.True(result);
        Assert.Empty(context.Projects.Where(p => p.Id == 20));
    }

    [Fact]
    public async Task UpdateProjectAsync_ShouldReturnNull_WhenProjectNotFound()
    {
        // Arrange


        using var context = CreateInMemoryContext();


        var mockValidator = CreateValidatorMock();
        var service = new ProjectService(context, mockValidator.Object);

        var updateDto = new ProjectDto { Title = "Doesn't Matter", Description = "None", ImageUrl = "none.jpg" };

        // Act
        var result = await service.UpdateProjectAsync(userId: 1, projectId: 999, updateDto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteProjectAsync_ShouldReturnFalse_WhenProjectDoesNotExist()
    {
        // Arrange


        using var context = CreateInMemoryContext();

        var mockValidator = CreateValidatorMock();
        var service = new ProjectService(context, mockValidator.Object);

        // Act
        var result = await service.DeleteProjectAsync(userId: 1, projectId: 999);

        // Assert
        Assert.False(result);
    }
}