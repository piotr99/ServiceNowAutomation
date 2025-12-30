using ClosedXML.Excel;
using ServiceNowAutomation.Models;

namespace ServiceNowAutomation.Services;

public class ResponseExcelExporter
{
    public void Save(List<AiResponse> responses, string path)
    {
        using var wb = new XLWorkbook();
        var ws = wb.AddWorksheet("Responses");

        ws.Cell(1, 1).Value = "Number";
        ws.Cell(1, 2).Value = "AssignmentGroup";
        ws.Cell(1, 3).Value = "Link";

        for (int i = 0; i < responses.Count; i++)
        {
            var r = responses[i];
            ws.Cell(i + 2, 1).Value = r.Number;
            ws.Cell(i + 2, 2).Value = r.AssignmentGroup;
            ws.Cell(i + 2, 3).Value = r.Response;
        }

        ws.Columns().AdjustToContents();
        wb.SaveAs(path);
    }
}
