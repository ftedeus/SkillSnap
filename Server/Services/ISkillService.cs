using SkillSnap.Shared.Models.Dtos;

namespace SkillSnap.Server.Services;

public interface ISkillService
{
    Task<SkillDto?> AddSkillAsync(int userId, SkillDto dto);
    Task<SkillDto?> UpdateSkillAsync(int userId, int skillId, SkillDto dto);
    Task<bool> DeleteSkillAsync(int userId, int skillId);
}