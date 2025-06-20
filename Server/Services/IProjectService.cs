using SkillSnap.Shared.Models.Dtos;

namespace SkillSnap.Server.Services;

public interface IProjectService
{
    Task<ProjectDto?> AddProjectAsync(int userId, ProjectDto dto);
    Task<ProjectDto?> UpdateProjectAsync(int userId, int projectId, ProjectDto dto);
        Task<bool> DeleteProjectAsync(int userId, int projectId);
}