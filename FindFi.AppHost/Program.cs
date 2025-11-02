using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

// Force using Docker Compose orchestrator and disable built-in dashboard to avoid requiring Aspire CLI binaries
Environment.SetEnvironmentVariable("Aspire__Orchestrator__Type", "Compose");
Environment.SetEnvironmentVariable("Aspire__Dashboard__Enabled", "false");

var builder = DistributedApplication.CreateBuilder(args);


// MySQL server and database (managed by Aspire). This will provide a connection string named "Default".
var mysql = builder.AddMySql("app")
                   .WithImage("mysql", "8.4")
                   .WithEnvironment("MYSQL_ROOT_PASSWORD", "12345678")
                   .WithLifetime(ContainerLifetime.Persistent);

var db = mysql.AddDatabase("Default");

// Web/API project wired into the distributed app; reference the DB to flow connection strings via config.
var api = builder.AddProject("findfi-api", "../FindFi/FindFi.csproj")
                 .WithReference(db)
                 .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
                 .WithReplicas(1);

builder.Build().Run();