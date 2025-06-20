using Microsoft.EntityFrameworkCore;
using SkillSnap.Server.Data;
using SkillSnap.Shared.Models.Dtos;

namespace SkillSnap.Server.Services;

public class PortfolioValidator : IPortfolioValidator
{
    private readonly SkillSnapContext _db;

    public PortfolioValidator(SkillSnapContext db)
    {
        _db = db;
    }

    // public async Task<string?> ValidateUserAsync(PortfolioUserDto dto, bool isUpdate = false)
    // {
    //     if (string.IsNullOrWhiteSpace(dto.Name))
    //         return "User name is required.";

    //     var nameExists = await _db.PortfolioUsers
    //         .AnyAsync(u => u.Name == dto.Name && (isUpdate == false || u.Id != dto.Id));

    //     if (nameExists)
    //         return $"A user named '{dto.Name}' already exists.";

    //     var duplicateProjectTitles = dto.Projects
    //         .GroupBy(p => p.Title)
    //         .Where(g => g.Count() > 1)
    //         .Select(g => g.Key)
    //         .ToList();

    //     if (duplicateProjectTitles.Any())
    //         return $"Duplicate project titles found: {string.Join(", ", duplicateProjectTitles)}";

    //     foreach (var proj in dto.Projects)
    //     {
    //         bool existsInDb = await _db.Projects
    //             .AnyAsync(p => p.Title == proj.Title && p.PortfolioUserId == dto.Id && p.Id != proj.Id);

    //         if (existsInDb)
    //             return $"Project title '{proj.Title}' already exists for this user.";
    //     }

    //     return null;
    // }

    public async Task<string?> ValidateUserAsync(PortfolioUserDto dto, bool isUpdate = false)
    {
        var nameError = await ValidateUniqueUserNameAsync(dto.Name, isUpdate ? dto.Id : null);
        if (nameError != null) return nameError;

        var duplicateProjectsError = ValidateDuplicateProjectTitlesInDto(dto.Projects);
        if (duplicateProjectsError != null) return duplicateProjectsError;

        foreach (var proj in dto.Projects)
        {
            bool existsInDb = await ProjectTitleExistsAsync(dto.Id, proj.Title, proj.Id);
            if (existsInDb)
                return $"Project title '{proj.Title}' already exists for this user.";
        }

        return null;
    }

    /// <summary>
    /// Validates that the provided user name is unique, excluding a specific user ID if provided. 
    ///  </summary>
    /// <param name="name"></param>
    /// <param name="excludeUserId"></param>
    /// <returns></returns>
    public async Task<string?> ValidateUniqueUserNameAsync(string name, int? excludeUserId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "User name is required.";

        bool exists = await _db.PortfolioUsers
            .AnyAsync(u => u.Name == name && (excludeUserId == null || u.Id != excludeUserId));

        return exists ? $"A user named '{name}' already exists." : null;
    }
    /// <summary>
    /// Validates that there are no duplicate project titles in the provided list of projects.
    /// </summary>
    /// <param name="projects"></param>
    /// <returns></returns>
    public string? ValidateDuplicateProjectTitlesInDto(IEnumerable<ProjectDto> projects)
    {
        var duplicates = projects
            .GroupBy(p => p.Title)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        return duplicates.Any()
            ? $"Duplicate project titles found: {string.Join(", ", duplicates)}"
            : null;
    }
    /// <summary>
    /// Checks if a project title already exists for a user, optionally excluding a specific project ID.    
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="title"></param>
    /// <param name="excludeProjectId"></param>
    /// <returns></returns>
    public async Task<bool> ProjectTitleExistsAsync(int userId, string title, int? excludeProjectId = null)
    {
        return await _db.Projects
            .AnyAsync(p =>
                p.PortfolioUserId == userId &&
                p.Title == title &&
                (excludeProjectId == null || p.Id != excludeProjectId));
    }
/// <summary>
/// Checks if a skill name already exists for a user, optionally excluding a specific skill ID.
/// </summary>
/// <param name="userId"></param>
/// <param name="name"></param>
/// <param name="excludeSkillId"></param>
/// <returns></returns>
public async Task<bool> SkillNameExistsAsync(int userId, string name, int? excludeSkillId = null)
{
    return await _db.Skills
        .AnyAsync(s =>
            s.PortfolioUserId == userId &&
            s.Name == name &&
            (excludeSkillId == null || s.Id != excludeSkillId));
}
}