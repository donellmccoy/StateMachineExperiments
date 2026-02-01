using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.EntityFrameworkCore;
using StateMachineExperiments;
using StateMachineExperiments.Common.Data;
using StateMachineExperiments.Common.Infrastructure;
using StateMachineExperiments.Modules.InformalLOD.Services;
using StateMachineExperiments.Modules.FormalLOD.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure DbContext with in-memory database (SQLite doesn't work in browser)
builder.Services.AddDbContext<LodDbContext>(options =>
    options.UseInMemoryDatabase("LodCasesDb"));

// Register SMTP settings and services
builder.Services.AddSingleton(new SmtpSettings
{
    Host = "smtp.gmail.com",
    Port = 587,
    UseSsl = true,
    FromEmail = "noreply@lod-system.example.com",
    FromName = "LOD System"
});
builder.Services.AddScoped<ISmtpService, SmtpService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Register Informal LOD services
builder.Services.AddScoped<ILodDataService, LodDataService>();
builder.Services.AddScoped<ILodBusinessRuleService, LodBusinessRuleService>();
builder.Services.AddScoped<ILodStateMachineFactory, LodStateMachineFactory>();
builder.Services.AddScoped<ILodTransitionValidator, LodTransitionValidator>();
builder.Services.AddScoped<ILodStateMachineService, LodStateMachineService>();
builder.Services.AddScoped<ILodVisualizationService, LodVisualizationService>();

// Register Formal LOD services
builder.Services.AddScoped<IFormalLodDataService, FormalLodDataService>();
builder.Services.AddScoped<IFormalLodBusinessRuleService, FormalLodBusinessRuleService>();
builder.Services.AddScoped<IFormalLodStateMachineFactory, FormalLodStateMachineFactory>();
builder.Services.AddScoped<IFormalLodTransitionValidator, FormalLodTransitionValidator>();
builder.Services.AddScoped<IFormalLodStateMachineService, FormalLodStateMachineService>();

await builder.Build().RunAsync();
