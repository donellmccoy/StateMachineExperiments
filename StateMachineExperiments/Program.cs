using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StateMachineExperiments;
using StateMachineExperiments.Data;
using StateMachineExperiments.Enums;
using StateMachineExperiments.Factories;
using StateMachineExperiments.Infrastructure;
using StateMachineExperiments.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure logging
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Configure DbContext factory with in-memory database (SQLite doesn't work in browser)
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("LodCasesDb"));

// Bind configuration settings
var smtpSettings = new SmtpSettings();
builder.Configuration.GetSection("SmtpSettings").Bind(smtpSettings);
builder.Services.AddSingleton(smtpSettings);

var businessRulesSettings = new BusinessRulesSettings();
builder.Configuration.GetSection("BusinessRules").Bind(businessRulesSettings);
builder.Services.AddSingleton(businessRulesSettings);

// Register Radzen services
builder.Services.AddScoped<Radzen.DialogService>();
builder.Services.AddScoped<Radzen.NotificationService>();
builder.Services.AddScoped<Radzen.TooltipService>();
builder.Services.AddScoped<Radzen.ContextMenuService>();

// Register SMTP and notification services
builder.Services.AddScoped<ISmtpService, SmtpService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Register unified state machine factory for both Informal and Formal LOD
builder.Services.AddScoped<ILineOfDutyStateMachineFactory, LineOfDutyStateMachineFactory>();

// Register unified LOD services
builder.Services.AddScoped<ILineOfDutyDataService, LineOfDutyDataService>();
builder.Services.AddScoped<ILineOfDutyBusinessRuleService, LineOfDutyBusinessRuleService>();
builder.Services.AddScoped<ILineOfDutyTransitionValidator, LineOfDutyTransitionValidator>();
builder.Services.AddScoped<ILineOfDutyStateMachineService, LineOfDutyStateMachineService>();

await builder.Build().RunAsync();
