# ServiceNowAutomation

Prosty projekt automatyzacji pracy z ServiceNow z wykorzystaniem LLM (Ollama) do klasyfikacji / sugestii grup przypisania (assignment groups).

## Wymagania
- .NET 10
- Uruchomiona Ollama lokalnie: `http://localhost:11434`
- Uruchomiony model (przykład — docelowo należy wybrać model wytrenowany):
  - `gpt-oss:120b-cloud`

## Konfiguracja
W projekcie aplikacji klient Ollama jest rejestrowany w DI:

```csharp
builder.Services.AddOllamaChatClient(
    baseUrl: "http://localhost:11434",
    model: "gpt-oss:120b-cloud"
);
```
## Zasada działania
- Impotrujemy z ServiceNow zgłosznia w formacie: incydent number, short description
- Wrzucamy eksportowane incydenty w pliku o nazwie incident.xlsx w ścieżce podanej w Program.cs linia: var inputPath = @"C:\Users\PPI3XY\source\repos\ServiceNowAutomation\ServiceNowAutomation\incident.xlsx";
- Uruchamiamy program i czekamy na zakończenie działania AI.


# Test: `AiServiceTests` (NUnit) – walidacja sugestii grupy przypisania z użyciem Ollama

Ten test sprawdza, czy usługa `AiService` potrafi zasugerować poprawną **grupę przypisania (assignment group)** dla incydentu związanego z problemem sieciowym, korzystając z lokalnie uruchomionego LLM przez **Ollama**.

## Cel testu

Celem jest zweryfikowanie, że dla zgłoszenia o treści wskazującej na awarię sieci (np. brak internetu w biurze), model AI zwraca **„Zespół ds. sieci”** jako rekomendowaną grupę przypisania — oraz że wynik jest wystarczająco powtarzalny (stabilny).

Ponieważ modele LLM mogą generować odpowiedzi niedeterministyczne, test wykonuje zapytanie wielokrotnie i sprawdza, czy poprawna odpowiedź pojawia się przynajmniej w większości prób.

## Wymagania środowiska

Aby test działał, wymagane jest:
- Uruchomiona lokalnie Ollama pod adresem: `http://localhost:11434`
- Dostępny model: `gpt-oss:120b-cloud`
- Poprawnie skonfigurowana instancja `AiService` oraz `IChatClient`

## Struktura testu

Plik testowy zawiera klasę NUnit:

- **`[SetUp] Setup()`** – przygotowanie zależności (klienta czatu)
- **`[Test] GetAssignmentGroupsAsync_Return_NetworkAssignmentGroup()`** – właściwy test scenariusza sieciowego

### Używany klient AI (Ollama)

W `Setup()` tworzony jest klient czatu bez użycia `AddOllamaChatClient()` oraz bez rejestrowania w kontenerze DI:

```csharp
IChatClient chatClient = new OllamaApiClient(
    new Uri("http://localhost:11434"),
    "gpt-oss:120b-cloud"
);
