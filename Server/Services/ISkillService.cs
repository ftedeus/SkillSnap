using SkillSnap.Shared.Models.Dtos;

namespace SkillSnap.Server.Services;

public interface ISkillService
{
    Task<SkillDto?> AddSkillAsync(int userId, SkillDto dto);
    Task<SkillDto?> UpdateSkillAsync(int userId, int skillId, SkillDto dto);
    Task<bool> DeleteSkillAsync(int userId, int skillId);

     Task<List<SkillDto>> GetAllSkillsAsync();                  // All skills, across users
    Task<SkillDto?> GetSkillByIdAsync(int userId, int skillId); // Skill scoped to user
    Task<List<SkillDto>> GetSkillsByUserIdAsync(int userId);    // All skills 
}