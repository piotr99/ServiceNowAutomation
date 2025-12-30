using Microsoft.Extensions.AI;
using ServiceNowAutomation.Models;

namespace ServiceNowAutomation.Services;

public class AiService
{
    private readonly IChatClient _chatClient;

    public AiService(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task<List<AiResponse>> GetAssignmentGroupsAsync(List<Incident> incidents, CancellationToken ct = default)
    {
        const string prePrompt =
@"Jesteś pomocnym asystentem, który analizuje opisy incydentów i sugeruje odpowiednie grupy przypisania.
Wybierz TYLKO JEDNĄ z: Zespół ds. sieci; Pomoc techniczna ds. oprogramowania; Konserwacja sprzętu; Zespół ds. bezpieczeństwa; Pomoc techniczna dla użytkowników.
Następnie podaj średnik ';' i link do instrukcji, która może pomóc rozwiązać problem.
Zwróć WYŁĄCZNIE: <nazwa grupy>;<link> (bez niczego więcej).";

        var results = new List<AiResponse>(incidents.Count);

        foreach (var incident in incidents)
        {
            var messages = new List<ChatMessage>
            {
                new(ChatRole.System, prePrompt),
                new(ChatRole.User, incident.ShortDescription)
            };

            var response = await _chatClient.GetResponseAsync(
                messages,
                new ChatOptions { Temperature = 0 },
                ct
            );

            var text = (response.Text ?? "").Trim();
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
}
