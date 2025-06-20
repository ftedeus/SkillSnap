using SkillSnap.Shared.Models;
 

namespace SkillSnap.Server.Data
{
    public static class DbInitializer
    {
        public static void Initialize(SkillSnapContext context)
        {
            if (context.PortfolioUsers.Any()) return; // DB has been seeded

            var users = new List<PortfolioUser>
            {
                new PortfolioUser
                {
                    Name = "Jane Doe",
                    Bio = "Full-stack developer passionate about clean code.",
                    ProfileImageUrl = "https://example.com/images/jane.jpg",
                    Projects = new List<Project>
                    {
                        new Project
                        {
                            Title = "Task Tracker",
                            Description = "A web app for managing daily tasks.",
                            ImageUrl = "https://example.com/images/task-tracker.png"
                        }
                    },
                    Skills = new List<Skill>
                    {
                        new Skill { Name = "C#", Level = "Advanced" },
                        new Skill { Name = "Blazor", Level = "Intermediate" }
                    }
                },
                new PortfolioUser
                {
                    Name = "John Smith",
                    Bio = "UX designer who codes a bit on the side.",
                    ProfileImageUrl = "https://example.com/images/john.jpg",
                    Projects = new List<Project>
                    {
                        new Project
                        {
                            Title = "Design System Kit",
                            Description = "Reusable components for web UI consistency.",
                            ImageUrl = "https://example.com/images/design-kit.png"
                        }
                    },
                    Skills = new List<Skill>
                    {
                        new Skill { Name = "Figma", Level = "Expert" },
                        new Skill { Name = "HTML/CSS", Level = "Intermediate" }
                    }
                }
            };

           

            context.PortfolioUsers.AddRange(users);
            context.SaveChanges();
        }
    }
}