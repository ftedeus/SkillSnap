using Microsoft.EntityFrameworkCore;
using SkillSnap.Server.Data;
using SkillSnap.Shared.Models;
using SkillSnap.Shared.Models.Dtos;

namespace SkillSnap.Server.Services;

public class ProjectService : IProjectService
{
    private readonly SkillSnapContext _db;
    private readonly IPortfolioValidator _validator;


    public ProjectService(SkillSnapContext db, IPortfolioValidator validator)
    {
        _db = db;
        _validator = validator;
    }


    public async Task<ProjectDto?> AddProjectAsync(int userId, ProjectDto dto)
    {
        var user = await _db.PortfolioUsers
            .Include(u => u.Projects)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null) return null;


        // if (user.Projects != null && user.Projects.Any(p => p.Title == dto.Title))
        //     throw new InvalidOperationException($"Project '{dto.Title}' already exists for this user.");

        if (await _validator.ProjectTitleExistsAsync(userId, dto.Title))
            throw new InvalidOperationException($"Project '{dto.Title}' already exists for this user.");

        var project = new Project
        {
            Title = dto.Title,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            PortfolioUser = user
        };

        (user.Projects ??= new List<Project>()).Add(project);
        await _db.SaveChangesAsync();

        return new ProjectDto
        {
            Id = project.Id,
            Title = project.Title,
            Description = project.Description,
            ImageUrl = project.ImageUrl
        };
    }
    public async Task<ProjectDto?> UpdateProjectAsync(int userId, int projectId, ProjectDto dto)
    {
        var project = await _db.Projects
            .Include(p => p.PortfolioUser)
            .FirstOrDefaultAsync(p => p.Id == projectId && p.PortfolioUserId == userId);

        if (project is null) return null;

        if (await _validator.ProjectTitleExistsAsync(userId, dto.Title, projectId))
            throw new InvalidOperationException($"Project '{dto.Title}' already exists for this user.");

        project.Title = dto.Title;
        project.Description = dto.Description;
        project.ImageUrl = dto.ImageUrl;

        await _db.SaveChangesAsync();

        return new ProjectDto
        {
            Id = project.Id,
            Title = project.Title,
            Description = project.Description,
            ImageUrl = project.ImageUrl
        };
    }

    public async Task<bool> DeleteProjectAsync(int userId, int projectId)
    {
        var project = await _db.Projects
            .FirstOrDefaultAsync(p => p.Id == projectId && p.PortfolioUserId == userId);

        if (project is null) return false;

        _db.Projects.Remove(project);
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<List<ProjectDto>> GetProjectsAsync(int userId)
    {
        return await _db.Projects
            .Where(p => p.PortfolioUserId == userId)
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                ImageUrl = p.ImageUrl
            })
            .ToListAsync();
    }

    public async Task<ProjectDto?> GetProjectByIdAsync(int userId, int projectId)
    {
        var project = await _db.Projects
            .FirstOrDefaultAsync(p => p.Id == projectId && p.PortfolioUserId == userId);

        if (project is null) return null;

        return new ProjectDto
        {
            Id = project.Id,
            Title = project.Title,
            Description = project.Description,
            ImageUrl = project.ImageUrl
        };
    }
    public async Task<List<ProjectDto>> GetProjectsAsync()
    {
        return await _db.Projects
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                ImageUrl = p.ImageUrl
            })
            .ToListAsync();
    }
}