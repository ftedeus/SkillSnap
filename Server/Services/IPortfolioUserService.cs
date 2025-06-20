using SkillSnap.Shared.Models.Dtos;

namespace SkillSnap.Server.Services;

public interface IPortfolioUserService
{
    Task<List<PortfolioUserDto>> GetAllAsync();
    Task<PortfolioUserDto?> GetByIdAsync(int id);
    Task<PortfolioUserDto> CreateAsync(PortfolioUserDto dto);
    Task<bool> UpdateAsync(int id, PortfolioUserDto dto);
    Task<bool> DeleteAsync(int id);
}