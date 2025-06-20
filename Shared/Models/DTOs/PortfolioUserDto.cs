 
namespace SkillSnap.Shared.Models.Dtos;


public class PortfolioUserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Bio { get; set; } = default!;
    public string ProfileImageUrl { get; set; } = default!;
    public List<ProjectDto> Projects { get; set; } = new();
    public List<SkillDto> Skills { get; set; } = new();
}
