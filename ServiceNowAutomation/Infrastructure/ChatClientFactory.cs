using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using OllamaSharp;

namespace ServiceNowAutomation.Infrastructure;

public static class ChatClientFactory
{
    public static IServiceCollection AddOllamaChatClient(
        this IServiceCollection services,
        string baseUrl,
        string model)
    {
        services.AddChatClient(sp =>
        {
            IChatClient client = new OllamaApiClient(baseUrl, model);

            return client
                .AsBuilder()
<<<<<<< HEAD
                .Build(sp);
            //Może korzytsać z funkcji
            // .UseFunctionInvocation()
=======
                // .UseFunctionInvocation()
                .Build(sp);
>>>>>>> d41ddc2f0f4c215655d9cc0f56767631cdecd60e
        });

        return services;
    }
}
