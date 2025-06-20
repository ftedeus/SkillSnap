using Server.Components;
using Microsoft.EntityFrameworkCore;
using SkillSnap.Server.Data;
using SkillSnap.Server.Helpers;
 using SkillSnap.Shared.Models;
 using SkillSnap.Shared.Models.Dtos;
using SkillSnap.Server.Services;


var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("https://localhost:5162") // 👈 your Blazor client URL
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});


// builder.Services.AddDbContext<SkillSnapContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<SkillSnapContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPortfolioUserService, PortfolioUserService>();
builder.Services.AddScoped<IPortfolioValidator, PortfolioValidator>();

builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ISkillService, SkillService>();


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<SkillSnapContext>(options =>
    options.UseSqlite("Data Source=skillsnap.db"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});


var app = builder.Build();

// execption Handler just before app.UseHttpsRedirection():


app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.ContentType = "application/json";

        var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;

        var message = exception is InvalidOperationException
            ? exception.Message
            : "An unexpected error occurred.";

        context.Response.StatusCode = exception is InvalidOperationException ? StatusCodes.Status409Conflict : 500;

        var errorResponse = new ErrorResponse { Message = message };
        await context.Response.WriteAsJsonAsync(errorResponse);
    });
});


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SkillSnap API v1");
    c.RoutePrefix = "swagger"; // or "" to serve at root
});


 
app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthorization();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SkillSnapContext>();
    DbInitializer.Initialize(context);
}

 

app.MapGet("/api/users", async (IPortfolioUserService service) =>
    Results.Ok(await service.GetAllAsync()));

 

app.MapGet("/api/users/{id}", async (int id, IPortfolioUserService service) =>
    await service.GetByIdAsync(id) is PortfolioUserDto dto
        ? Results.Ok(dto)
        : Results.NotFound());



 
app.MapPost("/api/users", async (PortfolioUserDto dto, IPortfolioUserService service) =>
{
    var created = await service.CreateAsync(dto);
    return Results.Created($"/api/users/{created.Id}", created);
});


app.MapPut("/api/users/{id}", async (int id, PortfolioUserDto dto, IPortfolioUserService service) =>
    await service.UpdateAsync(id, dto) ? Results.NoContent() : Results.NotFound());

app.MapDelete("/api/users/{id}", async (int id, IPortfolioUserService service) =>
    await service.DeleteAsync(id) ? Results.Ok() : Results.NotFound());



app.MapPost("/api/users/{userId}/projects", async (
   int userId,
   ProjectDto dto,
   IProjectService projectService) =>
{
   try
   {
       var created = await projectService.AddProjectAsync(userId, dto);

       return created is null
           ? Results.NotFound(new { message = $"User with ID {userId} not found." })
           : Results.Created($"/api/users/{userId}/projects/{created.Id}", created);
   }
   catch (InvalidOperationException ex)
   {
       return Results.Conflict(new { message = ex.Message });
   }
});


app.MapPost("/api/users/{userId}/skills", async (
    int userId,
    SkillDto dto,
    ISkillService skillService) =>
{
    try
    {
        var created = await skillService.AddSkillAsync(userId, dto);

        return created is null
            ? Results.NotFound(new { message = $"User with ID {userId} not found." })
            : Results.Created($"/api/users/{userId}/skills/{created.Id}", created);
    }
    catch (InvalidOperationException ex)
    {
        return Results.Conflict(new { message = ex.Message });
    }
});

app.MapPut("/api/users/{userId}/skills/{skillId}", async (
    int userId,
    int skillId,
    SkillDto dto,
    ISkillService skillService) =>
{
    try
    {
        var updated = await skillService.UpdateSkillAsync(userId, skillId, dto);
        return updated is null
            ? Results.NotFound(new { message = $"Skill not found or user mismatch." })
            : Results.Ok(updated);
    }
    catch (InvalidOperationException ex)
    {
        return Results.Conflict(new { message = ex.Message });
    }
});

app.MapDelete("/api/users/{userId}/skills/{skillId}", async (
    int userId,
    int skillId,
    ISkillService skillService) =>
{
    var success = await skillService.DeleteSkillAsync(userId, skillId);
    return success
        ? Results.NoContent()
        : Results.NotFound(new { message = "Skill not found or already deleted." });
});

app.MapPut("/api/users/{userId}/projects/{projectId}", async (
    int userId,
    int projectId,
    ProjectDto dto,
    IProjectService projectService) =>
{
    try
    {
        var updated = await projectService.UpdateProjectAsync(userId, projectId, dto);
        return updated is null
            ? Results.NotFound(new { message = $"Project not found or user mismatch." })
            : Results.Ok(updated);
    }
    catch (InvalidOperationException ex)
    {
        return Results.Conflict(new { message = ex.Message });
    }
});

app.MapDelete("/api/users/{userId}/projects/{projectId}", async (
    int userId,
    int projectId,
    IProjectService projectService) =>
{
    var success = await projectService.DeleteProjectAsync(userId, projectId);
    return success
        ? Results.NoContent()
        : Results.NotFound(new { message = "Project not found or already deleted." });
});
app.Run();
