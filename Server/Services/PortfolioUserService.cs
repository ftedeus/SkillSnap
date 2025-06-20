
using Microsoft.EntityFrameworkCore;
using SkillSnap.Server.Data;
using SkillSnap.Server.Helpers;
using SkillSnap.Shared.Models.Dtos;
using SkillSnap.Shared.Models;

namespace SkillSnap.Server.Services;

public class PortfolioUserService : IPortfolioUserService
{
    private readonly SkillSnapContext _db;
    private readonly IPortfolioValidator _validator;

    private readonly ILogger<PortfolioUserService> _logger;
    // public PortfolioUserService(SkillSnapContext db)
    // {
    //     _db = db;
    // }
    // public PortfolioUserService(SkillSnapContext db, PortfolioUserValidator validator)
    // {
    //     _db = db;
    //     _validator = validator;
    // }
    public PortfolioUserService(SkillSnapContext db, IPortfolioValidator validator, ILogger<PortfolioUserService> logger)
{
    _db = db;
    _validator = validator;
    _logger = logger;
}
    public async Task<List<PortfolioUserDto>> GetAllAsync()
    {
        var users = await _db.PortfolioUsers
         .TagWith("Fetching users with projects and skills")
            .Include(u => u.Projects)
            .Include(u => u.Skills)
            .AsSplitQuery()
            .ToListAsync();

        return users.Select(u => u.ToDto()).ToList();
    }

    public async Task<PortfolioUserDto?> GetByIdAsync(int id)
    {
        var user = await _db.PortfolioUsers
            .Include(u => u.Projects)
            .Include(u => u.Skills)
            .FirstOrDefaultAsync(u => u.Id == id);

        return user?.ToDto();
    }

    // public async Task<PortfolioUserDto> CreateAsync(PortfolioUserDto dto)
    // {
    //     var user = dto.ToEntity();
    //     _db.PortfolioUsers.Add(user);
    //     await _db.SaveChangesAsync();
    //     return user.ToDto();
    // }

    //     public async Task<PortfolioUserDto> CreateAsync(PortfolioUserDto dto)
    // {
    //     if (await _db.PortfolioUsers.AnyAsync(u => u.Name == dto.Name))
    //         throw new InvalidOperationException($"User with name '{dto.Name}' already exists.");

    //         foreach (var projDto in dto.Projects)
    //         {
    //             bool duplicate = await _db.Projects
    //                 .AnyAsync(p => p.PortfolioUserId == dto.Id && p.Title == projDto.Title);

    //             if (duplicate)
    //                 throw new InvalidOperationException($"Project title '{projDto.Title}' already exists for this user.");
    //         }


    //         var user = dto.ToEntity();
    //     _db.PortfolioUsers.Add(user);
    //     await _db.SaveChangesAsync();
    //     return user.ToDto();
    // }

    public async Task<PortfolioUserDto> CreateAsync(PortfolioUserDto dto)
    {
        var error = await _validator.ValidateUserAsync(dto);
        if (error is not null)
            throw new InvalidOperationException(error);

        var user = dto.ToEntity();
        _db.PortfolioUsers.Add(user);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Creating new user: {UserName}", dto.Name);
        return user.ToDto();
    }



    public async Task<bool> UpdateAsync(int id, PortfolioUserDto dto)
    {
        dto.Id = id; // ensure consistency

        var error = await _validator.ValidateUserAsync(dto, isUpdate: true);
        if (error is not null)
            throw new InvalidOperationException(error);

        var user = await _db.PortfolioUsers
            .Include(u => u.Projects)
            .Include(u => u.Skills)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user is null) return false;

        user.Name = dto.Name;
        user.Bio = dto.Bio;
        user.ProfileImageUrl = dto.ProfileImageUrl;

        // Update Projects
        user.Projects ??= new();
        user.Projects.RemoveAll(p => !dto.Projects.Any(dp => dp.Id == p.Id));
        foreach (var projDto in dto.Projects)
        {
            var proj = user.Projects.FirstOrDefault(p => p.Id == projDto.Id);
            if (proj is null)
            {
                user.Projects.Add(new Project
                {
                    Title = projDto.Title,
                    Description = projDto.Description,
                    ImageUrl = projDto.ImageUrl,
                    PortfolioUser = user
                });
            }
            else
            {
                proj.Title = projDto.Title;
                proj.Description = projDto.Description;
                proj.ImageUrl = projDto.ImageUrl;
            }
        }

        // Update Skills
        user.Skills ??= new();
        user.Skills.RemoveAll(s => !dto.Skills.Any(ds => ds.Id == s.Id));
        foreach (var skillDto in dto.Skills)
        {
            var skill = user.Skills.FirstOrDefault(s => s.Id == skillDto.Id);
            if (skill is null)
            {
                user.Skills.Add(new Skill
                {
                    Name = skillDto.Name,
                    Level = skillDto.Level,
                    PortfolioUser = user
                });
            }
            else
            {
                skill.Name = skillDto.Name;
                skill.Level = skillDto.Level;
            }
        }

        await _db.SaveChangesAsync();
          _logger.LogInformation("Updating  user: {UserName}", user.Name);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _db.PortfolioUsers.FindAsync(id);
        if (user is null) return false;

        _db.PortfolioUsers.Remove(user);
        await _db.SaveChangesAsync();
        return true;
    }
}