using Microsoft.AspNetCore.Mvc;

namespace graphql_proj_Csharp.Controllers;

[ApiController]
[Route("api/controllers")]
public sealed class HealthController(IConfiguration configuration) : ControllerBase
{
    [HttpGet("health")]
    public IActionResult Health() =>
        Ok(new { status = "ok", service = "GraphQL Bookstore API" });

    [HttpGet("info")]
    public IActionResult Info() =>
        Ok(new
        {
            name = "GraphQL Bookstore API",
            graphql = "/graphql",
            database = "PostgreSQL",
            controllers = "/api/controllers",
            minimalApi = "/api",
            applyMigrationsOnStartup = configuration.GetValue("Database:ApplyMigrationsOnStartup", false)
        });
}
