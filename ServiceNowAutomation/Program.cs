<<<<<<< HEAD
=======
﻿using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
>>>>>>> d41ddc2f0f4c215655d9cc0f56767631cdecd60e
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.AI;
using OllamaSharp;
using ServiceNowAutomation.Helpers;
using ServiceNowAutomation.Infrastructure;
using ServiceNowAutomation.Services;

<<<<<<< HEAD
var builder = Host.CreateApplicationBuilder(args);

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
=======
var builder = Host.CreateApplicationBuilder();

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

>>>>>>> d41ddc2f0f4c215655d9cc0f56767631cdecd60e
var aiResponses = await ai.GetAssignmentGroupsAsync(incidents);

var outPath = OutputPathBuilder.Build(baseDir, "response");
exporter.Save(aiResponses, outPath);
<<<<<<< HEAD
=======

Console.WriteLine($"Zapisano wynik: {outPath}");
>>>>>>> d41ddc2f0f4c215655d9cc0f56767631cdecd60e
