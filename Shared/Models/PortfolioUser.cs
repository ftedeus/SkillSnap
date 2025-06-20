using System.ComponentModel.DataAnnotations;

namespace SkillSnap.Shared.Models
{
    public class PortfolioUser
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        public required string Bio { get; set; }
        public required string ProfileImageUrl { get; set; }
        public List<Project>? Projects { get; set; }
        public List<Skill>? Skills { get; set; }
    }
}