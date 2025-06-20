// public abstract class TestBase
// {
//     protected SkillSnapContext CreateInMemoryContext()
//     {
//         var options = new DbContextOptionsBuilder<SkillSnapContext>()
//             .UseInMemoryDatabase($"SkillSnapTest_{Guid.NewGuid()}")
//             .Options;

//         return new SkillSnapContext(options);
//     }

//     protected PortfolioUser CreateTestUser(int id = 1, string name = "Test User") => new PortfolioUser
//     {
//         Id = id,
//         Name = name,
//         Bio = "Test bio",
//         ProfileImageUrl = "https://example.com/profile.jpg"
//     };

//     protected Mock<IPortfolioValidator> CreateValidatorMock() => new Mock<IPortfolioValidator>();
// }