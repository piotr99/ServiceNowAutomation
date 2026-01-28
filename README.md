# ServiceNowAutomation

Prosty projekt automatyzacji pracy z ServiceNow z wykorzystaniem LLM (Ollama) do klasyfikacji / sugestii grup przypisania (assignment groups).

## Wymagania
- .NET 10
- Uruchomiona Ollama lokalnie: `http://localhost:11434`
- Uruchmiony model (przykład):
  - `gpt-oss:120b-cloud`

## Konfiguracja
W projekcie aplikacji klient Ollama jest rejestrowany w DI:

```csharp
builder.Services.AddOllamaChatClient(
    baseUrl: "http://localhost:11434",
    model: "gpt-oss:120b-cloud"
);
