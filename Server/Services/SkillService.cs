using Microsoft.EntityFrameworkCore;
using SkillSnap.Server.Data;
using SkillSnap.Shared.Models;
using SkillSnap.Shared.Models.Dtos;

namespace SkillSnap.Server.Services;

public class SkillService : ISkillService
{
    private readonly SkillSnapContext _db;
    private readonly IPortfolioValidator _validator;
    public SkillService(SkillSnapContext db, IPortfolioValidator validator)
    {
        _db = db;
        _validator = validator;

    }

    public async Task<SkillDto?> AddSkillAsync(int userId, SkillDto dto)
    {
        var user = await _db.PortfolioUsers
            .Include(u => u.Skills)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null) return null;






        if (await _validator.SkillNameExistsAsync(userId, dto.Name))
            throw new InvalidOperationException($"Skill '{dto.Name}' already exists for this user.");

        var skill = new Skill
        {
            Name = dto.Name,
            Level = dto.Level,
            PortfolioUser = user
        };

        (user.Skills ??= new List<Skill>()).Add(skill);
        await _db.SaveChangesAsync();

        return new SkillDto
        {
            Id = skill.Id,
            Name = skill.Name,
            Level = skill.Level
        };
    }

    public async Task<SkillDto?> UpdateSkillAsync(int userId, int skillId, SkillDto dto)
    {
        var skill = await _db.Skills
            .Include(s => s.PortfolioUser)
            .FirstOrDefaultAsync(s => s.Id == skillId && s.PortfolioUserId == userId);

        if (skill is null) return null;

        if (await _validator.SkillNameExistsAsync(userId, dto.Name, skillId))
            throw new InvalidOperationException($"Skill '{dto.Name}' already exists for this user.");

        skill.Name = dto.Name;
        skill.Level = dto.Level;

        await _db.SaveChangesAsync();

        return new SkillDto
        {
            Id = skill.Id,
            Name = skill.Name,
            Level = skill.Level
        };
    }

    public async Task<bool> DeleteSkillAsync(int userId, int skillId)
    {
        var skill = await _db.Skills
            .FirstOrDefaultAsync(s => s.Id == skillId && s.PortfolioUserId == userId);

        if (skill is null) return false;

        _db.Skills.Remove(skill);
        await _db.SaveChangesAsync();

        return true;
    }
}