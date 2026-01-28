<<<<<<< HEAD
﻿using System.Text;
using Microsoft.Extensions.AI;
using Newtonsoft.Json;
using ServiceNowAutomation.Models;
using ServiceNowAutomation.Helpers;
using Microsoft.Extensions.Logging;
=======
﻿using Microsoft.Extensions.AI;
using ServiceNowAutomation.Models;
>>>>>>> d41ddc2f0f4c215655d9cc0f56767631cdecd60e

namespace ServiceNowAutomation.Services;

public class AiService
{
    private readonly IChatClient _chatClient;
<<<<<<< HEAD
    private readonly ILogger<AiService> _logger;

    public AiService(IChatClient chatClient, ILogger<AiService> logger)
    {
        _chatClient = chatClient;
        _logger = logger;
=======

    public AiService(IChatClient chatClient)
    {
        _chatClient = chatClient;
>>>>>>> d41ddc2f0f4c215655d9cc0f56767631cdecd60e
    }

    public async Task<List<AiResponse>> GetAssignmentGroupsAsync(List<Incident> incidents, CancellationToken ct = default)
    {
<<<<<<< HEAD
        //Open txt file
        string instrukcje = ReadInstructions.Read("C:\\Users\\PPI3XY\\source\\repos\\ServiceNowAutomation\\ServiceNowAutomation\\Instrukcje.txt");
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


=======
        const string prePrompt =
@"Jesteś pomocnym asystentem, który analizuje opisy incydentów i sugeruje odpowiednie grupy przypisania.
Wybierz TYLKO JEDNĄ z: Zespół ds. sieci; Pomoc techniczna ds. oprogramowania; Konserwacja sprzętu; Zespół ds. bezpieczeństwa; Pomoc techniczna dla użytkowników.
Następnie podaj średnik ';' i link do instrukcji, która może pomóc rozwiązać problem.
Zwróć WYŁĄCZNIE: <nazwa grupy>;<link> (bez niczego więcej).";

        var results = new List<AiResponse>(incidents.Count);

>>>>>>> d41ddc2f0f4c215655d9cc0f56767631cdecd60e
        foreach (var incident in incidents)
        {
            var messages = new List<ChatMessage>
            {
                new(ChatRole.System, prePrompt),
                new(ChatRole.User, incident.ShortDescription)
            };

<<<<<<< HEAD

            int retryLimit = 3;
            AiResponse? jsonIncident = null;

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
                    jsonIncident = JsonConvert.DeserializeObject<AiResponse>(raw);

                    if (jsonIncident is null)
                        throw new JsonException("Deserializacja zwróciła null.");

                    results.Add(new AiResponse
                    {
                        Number = incident.Number,
                        AssignmentGroup = jsonIncident.AssignmentGroup,
                        Response = jsonIncident.Response
                    });

                    _logger.LogInformation("{Incident} -> {Group}; {Response}",
                         incident.Number, jsonIncident.AssignmentGroup, jsonIncident.Response);
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
            if (jsonIncident is null)
            {
                results.Add(new AiResponse
                {
                    Number = incident.Number,
                    AssignmentGroup = "Pomoc techniczna dla użytkowników", // albo "Unknown"
                    Response = "" // albo jakiś ogólny link
                });
            }

        }




        return results;
    }
}
=======
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
>>>>>>> d41ddc2f0f4c215655d9cc0f56767631cdecd60e
