using SkillSnap.Shared.Models.Dtos;

namespace SkillSnap.Server.Services;

public interface IPortfolioValidator
{
    Task<string?> ValidateUniqueUserNameAsync(string name, int? excludeUserId = null);
    string? ValidateDuplicateProjectTitlesInDto(IEnumerable<ProjectDto> projects);
    Task<bool> ProjectTitleExistsAsync(int userId, string title, int? excludeProjectId = null);
    Task<bool> SkillNameExistsAsync(int userId, string name, int? excludeSkillId = null);
    Task<string?> ValidateUserAsync(PortfolioUserDto dto, bool isUpdate = false);
}