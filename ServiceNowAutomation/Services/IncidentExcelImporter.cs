using ClosedXML.Excel;
using ServiceNowAutomation.Models;

namespace ServiceNowAutomation.Services;

public class IncidentExcelImporter
{
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

        Console.WriteLine($"Wczytano: {incidents.Count}");
        return incidents;
    }
}
