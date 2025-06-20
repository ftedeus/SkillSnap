// using Microsoft.EntityFrameworkCore;
// using SkillSnap.Server.Data;
 
// using SkillSnap.Server.Services;
// using SkillSnap.Shared.Models;
 
// using SkillSnap.Shared.Models.Dtos;
// using Moq;



namespace SkillSnap.Tests;



public class ProjectServiceTests
{
    [Fact]
    public async Task AddProjectAsync_ShouldAddProject_WhenTitleIsUnique()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<SkillSnapContext>()
            .UseInMemoryDatabase(databaseName: $"Project_Add_Test_{Guid.NewGuid()}")
            .Options;

        using var context = new SkillSnapContext(options);
        var user = new PortfolioUser { Id = 1, Name = "Alice", Bio = "Test user", ProfileImageUrl = "https://example.com/alice.jpg" };
        context.PortfolioUsers.Add(user);
        await context.SaveChangesAsync();

        var mockValidator = new Mock<IPortfolioValidator>();
        mockValidator.Setup(v => v.ProjectTitleExistsAsync(1, "New Project", null))
                     .ReturnsAsync(false);

        var service = new ProjectService(context, mockValidator.Object);
        var dto = new ProjectDto { Title = "New Project", Description = "Testing", ImageUrl = "url" };

        // Act
        var result = await service.AddProjectAsync(1, dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Project", result.Title);
    }

    [Fact]
    public async Task AddProjectAsync_ShouldThrow_WhenTitleIsDuplicate()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<SkillSnapContext>()
            .UseInMemoryDatabase(databaseName: $"DuplicateProjectTest_{Guid.NewGuid()}")
            .Options;

        using var context = new SkillSnapContext(options);
        context.PortfolioUsers.Add(new PortfolioUser
        {
            Id = 2,
            Name = "Bob",
            Bio = "Duplicate tester",
            ProfileImageUrl = "https://example.com/bob.jpg"
        });
        await context.SaveChangesAsync();

        var mockValidator = new Mock<IPortfolioValidator>();
        mockValidator.Setup(v => v.ProjectTitleExistsAsync(2, "Existing Project", null))
                     .ReturnsAsync(true);

        var service = new ProjectService(context, mockValidator.Object);
        var dto = new ProjectDto { Title = "Existing Project", Description = "Clash", ImageUrl = "url" };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.AddProjectAsync(2, dto));
    }

    // [Fact]
    // public async Task GetProjectsAsync_ShouldReturnProjectsForUser()
    // {
    //     // Arrange
    //     var options = new DbContextOptionsBuilder<SkillSnapContext>()
    //         .UseInMemoryDatabase(databaseName: $"GetProjectsTest_{Guid.NewGuid()}")
    //         .Options;

    //     using var context = new SkillSnapContext(options);

    //     var user = new PortfolioUser
    //     {
    //         Id = 3,
    //         Name = "Dana",
    //         Bio = "Projects collector",
    //         ProfileImageUrl = "https://example.com/dana.jpg",
    //         Projects = new List<Project>
    //         {
    //             new Project { Title = "One", Description = "Desc 1", ImageUrl = "url1" },
    //             new Project { Title = "Two", Description = "Desc 2", ImageUrl = "url2" }
    //         }
    //     };

    //     context.PortfolioUsers.Add(user);
    //     await context.SaveChangesAsync();

    //     var mockValidator = new Mock<IPortfolioValidator>();
    //     var service = new ProjectService(context, mockValidator.Object);

    //     // Act
    //     var result = await service.GetProjectsAsync(3);

    //     // Assert
    //     Assert.NotNull(result);
    //     Assert.Equal(2, result.Count());
    //     Assert.Contains(result, p => p.Title == "One");
    //     Assert.Contains(result, p => p.Title == "Two");
    // }
    [Fact]
    public async Task UpdateProjectAsync_ShouldUpdateProject_WhenValid()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<SkillSnapContext>()
            .UseInMemoryDatabase(databaseName: $"UpdateProjectTest_{Guid.NewGuid()}")
            .Options;

        using var context = new SkillSnapContext(options);
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

        var mockValidator = new Mock<IPortfolioValidator>();
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
        // Arrange
        var options = new DbContextOptionsBuilder<SkillSnapContext>()
            .UseInMemoryDatabase(databaseName: $"DeleteProjectTest_{Guid.NewGuid()}")
            .Options;

        using var context = new SkillSnapContext(options);
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

        var mockValidator = new Mock<IPortfolioValidator>();
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
        var options = new DbContextOptionsBuilder<SkillSnapContext>()
            .UseInMemoryDatabase($"Update_Missing_{Guid.NewGuid()}")
            .Options;

        using var context = new SkillSnapContext(options);

        var mockValidator = new Mock<IPortfolioValidator>();
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
    var options = new DbContextOptionsBuilder<SkillSnapContext>()
        .UseInMemoryDatabase($"Delete_Missing_{Guid.NewGuid()}")
        .Options;

    using var context = new SkillSnapContext(options);
    var mockValidator = new Mock<IPortfolioValidator>();
    var service = new ProjectService(context, mockValidator.Object);

    // Act
    var result = await service.DeleteProjectAsync(userId: 1, projectId: 999);

    // Assert
    Assert.False(result);
}
}