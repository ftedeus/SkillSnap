using SkillSnap.Shared.Models;
using SkillSnap.Shared.Models.Dtos;

namespace SkillSnap.Server.Helpers;

public static class DtoMapper
{
    public static PortfolioUserDto ToDto(this PortfolioUser user) => new()
    {
        Id = user.Id,
        Name = user.Name,
        Bio = user.Bio,
        ProfileImageUrl = user.ProfileImageUrl,
        Projects = user.Projects?.Select(p => new ProjectDto
        {
            Id = p.Id,
            Title = p.Title,
            Description = p.Description,
            ImageUrl = p.ImageUrl
        }).ToList() ?? new(),

        Skills = user.Skills?.Select(s => new SkillDto
        {
            Id = s.Id,
            Name = s.Name,
            Level = s.Level
        }).ToList() ?? new()
    };

    public static PortfolioUser ToEntity(this PortfolioUserDto dto)
{
    var user = new PortfolioUser
    {
        Id = dto.Id,
        Name = dto.Name,
        Bio = dto.Bio,
        ProfileImageUrl = dto.ProfileImageUrl,
        Projects = dto.Projects.Select(p => new Project
        {
            Id = p.Id,
            Title = p.Title,
            Description = p.Description,
            ImageUrl = p.ImageUrl,
            PortfolioUserId = dto.Id // cautious: might be 0 for new users
        }).ToList(),

        Skills = dto.Skills.Select(s => new Skill
        {
            Id = s.Id,
            Name = s.Name,
            Level = s.Level,
            PortfolioUserId = dto.Id
        }).ToList()
    };

    // Link navigation properties (important for EF tracking)
    foreach (var proj in user.Projects)
    {
        proj.PortfolioUser = user;
    }

    foreach (var skill in user.Skills)
    {
        skill.PortfolioUser = user;
    }

    return user;
}
}