using ClosedXML.Excel;
<<<<<<< HEAD
using Microsoft.Extensions.Logging;
=======
>>>>>>> d41ddc2f0f4c215655d9cc0f56767631cdecd60e
using ServiceNowAutomation.Models;

namespace ServiceNowAutomation.Services;

public class IncidentExcelImporter
{
<<<<<<< HEAD
    private readonly ILogger<AiService> _logger;
    public IncidentExcelImporter(ILogger<AiService> logger)
    {
        _logger = logger;
    }
=======
>>>>>>> d41ddc2f0f4c215655d9cc0f56767631cdecd60e
    public List<Incident> Import(string path, int worksheetIndex = 1, int startRow = 2)
    {
        var incidents = new List<Incident>();

        using var wb = new XLWorkbook(path);
        var ws = wb.Worksheet(worksheetIndex);

        foreach (var row in ws.RangeUsed().RowsUsed().Skip(startRow - 1))
        {
            var number = row.Cell(1).GetString();
            var desc = row.Cell(2).GetString();

            if (string.IsNullOrWhiteSpace(number) && string.IsNullOrWhiteSpace(desc))
                continue;

            incidents.Add(new Incident
            {
                Number = number,
                ShortDescription = desc
            });
        }

<<<<<<< HEAD
        _logger.LogInformation("Imported {Count} incidents from Excel.", 
            incidents.Count);
=======
        Console.WriteLine($"Wczytano: {incidents.Count}");
>>>>>>> d41ddc2f0f4c215655d9cc0f56767631cdecd60e
        return incidents;
    }
}
