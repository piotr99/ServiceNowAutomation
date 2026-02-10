using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using ServiceNowAutomation.Helpers;
using ServiceNowAutomation.Models;

namespace ServiceNowAutomation.Services;

public class AiService
{
    private readonly IChatClient _chatClient;
    private readonly ILogger<AiService> _logger;

    public AiService(IChatClient chatClient, ILogger<AiService> logger)
    {
        _chatClient = chatClient;
        _logger = logger;
    }

    // Przydatne np. w testach, gdy nie chcesz/nie masz loggera
    public AiService(IChatClient chatClient)
    {
        _chatClient = chatClient;
        _logger = NullLogger<AiService>.Instance;
    }

    public async Task<List<AiResponse>> GetAssignmentGroupsAsync(
        List<Incident> incidents,
        CancellationToken ct = default)
    {
        // Open txt file
        string instrukcje = ReadInstructions.Read(
            @"C:\Users\PPI3XY\source\repos\ServiceNowAutomation\ServiceNowAutomation\Instrukcje.txt"
        );

        string prePrompt =
$@"Jesteś asystentem triage. Na podstawie opisu incydentu wybierasz grupę przypisania oraz odpowiedź z listy instrukcji.

Zasady:
1) Wybierz DOKŁADNIE JEDNĄ grupę z listy poniżej.
2) Pole Response musi zawierać DOKŁADNIE linię tekstu JEDNEGO pasującego wpisu w sekcji INSTRUKCJE.
   - Nie wymyślaj nowych linków, ścieżek, nazw plików ani kontaktów.
   - Jeśli pasuje kilka wpisów, wybierz NAJBARDZIEJ KONKRETNY (np. 'Wabco 7.10 PL' zamiast ogólnego 'Wabco').
3) Jeśli nie ma pasującej instrukcji albo nie jesteś pewien, ustaw Response dokładnie na:
""potrzebujemy więcej informacji na temat problemu, który napotkałeś. Proszę opisz, czego dotyczy Twoje zgłoszenie""

Dostępne grupy (wybierz jedną):
- Zespół ds. sieci
- Pomoc techniczna ds. oprogramowania
- Konserwacja sprzętu
- Zespół ds. bezpieczeństwa
- Pomoc techniczna dla użytkowników

Zwróć WYŁĄCZNIE poprawny JSON (bez markdown, bez dodatkowego tekstu) w formacie:
{{""AssignmentGroup"":""<nazwa>"",""Response"":""<wartość z instrukcji lub wiadomość>""}}

--- INSTRUKCJE (jedyny dozwolony słownik odpowiedzi Response) ---
{instrukcje}
--- KONIEC INSTRUKCJI ---";

        var results = new List<AiResponse>(incidents.Count);

        foreach (var incident in incidents)
        {
            var messages = new List<ChatMessage>
            {
                new(ChatRole.System, prePrompt),
                new(ChatRole.User, incident.ShortDescription)
            };

            const int retryLimit = 3;
            AiResponse? parsed = null;

            for (int attempt = 1; attempt <= retryLimit; attempt++)
            {
                var response = await _chatClient.GetResponseAsync(
                    messages,
                    new ChatOptions { Temperature = 0 },
                    ct
                );

                var raw = (response.Text ?? "").Trim();

                try
                {
                    parsed = JsonConvert.DeserializeObject<AiResponse>(raw);

                    if (parsed is null)
                        throw new JsonException("Deserializacja zwróciła null.");

                    results.Add(new AiResponse
                    {
                        Number = incident.Number,
                        AssignmentGroup = parsed.AssignmentGroup,
                        Response = parsed.Response
                    });

                    _logger.LogInformation("{Incident} -> {Group}; {Response}",
                        incident.Number, parsed.AssignmentGroup, parsed.Response);

                    break;
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning("JSON fail for {Incident} ({Attempt}/{RetryLimit}): {Message}; raw: {Raw}",
                        incident.Number, attempt, retryLimit, ex.Message, raw);

                    if (attempt < retryLimit)
                        await Task.Delay(TimeSpan.FromMilliseconds(250 * attempt), ct);
                }
            }

            // fallback po retry
            if (parsed is null)
            {
                results.Add(new AiResponse
                {
                    Number = incident.Number,
                    AssignmentGroup = "Pomoc techniczna dla użytkowników",
                    Response = "potrzebujemy więcej informacji na temat problemu, który napotkałeś. Proszę opisz, czego dotyczy Twoje zgłoszenie"
                });
            }
        }

        return results;
    }
}
