using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OllamaSharp;
using ServiceNowAutomation.Helpers;
using ServiceNowAutomation.Infrastructure;
using Microsoft.Extensions.Logging;
using ServiceNowAutomation.Services;

var builder = Host.CreateApplicationBuilder();

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// AI client
builder.Services.AddOllamaChatClient(
    baseUrl: "http://localhost:11434",
    model: "gpt-oss:120b-cloud"
);

// App services
builder.Services.AddSingleton<IncidentExcelImporter>();
builder.Services.AddSingleton<ResponseExcelExporter>();
builder.Services.AddSingleton<AiService>();


using var app = builder.Build();

var importer = app.Services.GetRequiredService<IncidentExcelImporter>();
var ai = app.Services.GetRequiredService<AiService>();
var exporter = app.Services.GetRequiredService<ResponseExcelExporter>();

var inputPath = @"C:\Users\PPI3XY\source\repos\ServiceNowAutomation\ServiceNowAutomation\incident.xlsx";
var baseDir = @"C:\Users\PPI3XY\source\repos\ServiceNowAutomation\ServiceNowAutomation";

var incidents = importer.Import(inputPath);

var aiResponses = await ai.GetAssignmentGroupsAsync(incidents);

var outPath = OutputPathBuilder.Build(baseDir, "response");
exporter.Save(aiResponses, outPath);
