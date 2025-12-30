using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OllamaSharp;

class Program
{
    static async Task Main()
    {
        // create connection to ai client
        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddChatClient(sp =>
        {
            IChatClient client = (IChatClient)new OllamaApiClient("http://localhost:11434", "gpt-oss:120b-cloud");
            return client
                .AsBuilder()
                // .UseFunctionInvocation() // włącz tylko, jeśli faktycznie używasz narzędzi/functions
                .Build(sp);
        });

        using var app = builder.Build();
        var chatClient = app.Services.GetRequiredService<IChatClient>();

        // wczytanie incydentów z excela
        List<Incident> incidents = IncidentImport();

        // test
        var testIncidents = new List<Incident>
        {
            new Incident
            {
                Number = "INC0010001",
                ShortDescription = "Użytkownik nie może połączyć się z siecią VPN. Komunikat o błędzie wskazuje na niepowodzenie uwierzytelnienia."
            }
        };

        // for each incident ask llama for assignment group
        var aiResponses = await GetAssignmentGroup(chatClient, testIncidents);

        // zapisz do Excela
        var outPath = BuildOutputPath(
            baseDir: @"C:\Users\PPI3XY\source\repos\ServiceNowAutomation\ServiceNowAutomation",
            filePrefix: "response"
        );

        SaveResponsesToExcel(aiResponses, outPath);

        Console.WriteLine($"Zapisano wynik: {outPath}");
    }

    public static List<Incident> IncidentImport()
    {
        var path = @"C:\Users\PPI3XY\source\repos\ServiceNowAutomation\ServiceNowAutomation\incident.xlsx";

        var incidents = new List<Incident>();

        using var wb = new XLWorkbook(path);
        var ws = wb.Worksheet(1);

        int startRow = 1; // jeśli masz nagłówki: 2

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

    public static async Task<List<AiResponse>> GetAssignmentGroup(IChatClient chatClient, List<Incident> incidents)
    {
        string prePrompt =
@"Jesteś pomocnym asystentem, który analizuje opisy incydentów i sugeruje odpowiednie grupy przypisania.
Wybierz TYLKO JEDNĄ z: Zespół ds. sieci; Pomoc techniczna ds. oprogramowania; Konserwacja sprzętu; Zespół ds. bezpieczeństwa; Pomoc techniczna dla użytkowników.
Następnie podaj średnik ';' i link do instrukcji, która może pomóc rozwiązać problem.
Zwróć WYŁĄCZNIE: <nazwa grupy>;<link> (bez niczego więcej).";

        var results = new List<AiResponse>();

        foreach (var incident in incidents)
        {
            var messages = new List<ChatMessage>
            {
                new(ChatRole.System, prePrompt),
                new(ChatRole.User, incident.ShortDescription)
            };

            var response = await chatClient.GetResponseAsync(
                messages,
                new ChatOptions { Temperature = 0 }
            );

            var text = (response.Text ?? "").Trim();

            // Bezpieczny split: max 2 części
            var parts = text.Split(';', 2, StringSplitOptions.TrimEntries);

            var group = parts.Length >= 1 ? parts[0] : "";
            var link = parts.Length == 2 ? parts[1] : "";

            results.Add(new AiResponse
            {
                Number = incident.Number,
                AssignmentGroup = group,
                Response = link
            });

            Console.WriteLine($"{incident.Number} -> {group} ; {link}");
        }

        return results;
    }

    private static string BuildOutputPath(string baseDir, string filePrefix)
    {
        // yyyyMMdd_HHmmss jest bezpieczne dla nazw plików
        var ts = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        return Path.Combine(baseDir, $"{filePrefix}_{ts}.xlsx");
    }

    private static void SaveResponsesToExcel(List<AiResponse> responses, string path)
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

    public class Incident
    {
        public string Number { get; set; } = "";
        public string ShortDescription { get; set; } = "";
    }

    public class AiResponse
    {
        public string Number { get; set; } = "";
        public string AssignmentGroup { get; set; } = "";
        public string Response { get; set; } = "";
    }
}
