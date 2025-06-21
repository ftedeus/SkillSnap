# 💥 Clean previous coverage
Remove-Item -Recurse -Force .\TestResults, .\CoverageReport -ErrorAction Ignore

# 🔧 Run tests with coverage
dotnet test SkillSnap.Tests/SkillSnap.Tests.csproj `
  --configuration Release `
  --collect:"XPlat Code Coverage" `
  --results-directory ./TestResults

# 📊 Generate HTML report
reportgenerator `
  -reports:"**/coverage.cobertura.xml" `
  -targetdir:"CoverageReport" `
  -reporttypes:Html

# 🚀 Open the report (Windows-only)
Start-Process CoverageReport\index.html