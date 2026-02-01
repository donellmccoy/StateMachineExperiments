using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StateMachineExperiments;
using StateMachineExperiments.Common.Data;
using StateMachineExperiments.Common.Infrastructure;
using StateMachineExperiments.Modules.InformalLOD.Services;
using StateMachineExperiments.Modules.FormalLOD.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure logging
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Configure DbContext factory with in-memory database (SQLite doesn't work in browser)
builder.Services.AddDbContextFactory<LodDbContext>(options =>
    options.UseInMemoryDatabase("LodCasesDb"));

// Bind configuration settings
var smtpSettings = new SmtpSettings();
builder.Configuration.GetSection("SmtpSettings").Bind(smtpSettings);
builder.Services.AddSingleton(smtpSettings);

var businessRulesSettings = new BusinessRulesSettings();
builder.Configuration.GetSection("BusinessRules").Bind(businessRulesSettings);
builder.Services.AddSingleton(businessRulesSettings);

// Register SMTP and notification services
builder.Services.AddScoped<ISmtpService, SmtpService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Register Informal LOD services
builder.Services.AddScoped<IInformalLineOfDutyDataService, InformalLineOfDutyService>();
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
