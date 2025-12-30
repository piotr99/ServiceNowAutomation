using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using OllamaSharp;
using ServiceNowAutomation.Infrastructure;
using ServiceNowAutomation.Models;
using ServiceNowAutomation.Services;

namespace ServiceNowAutomationTests
{
    [TestFixture]
    public class AiServiceTests
    {
        //private ServiceProvider _sp = default!;
        private IChatClient _chatClient = default!;
        private AiService _aiService = default!;

        [SetUp]
        public void Setup()
        {
            //Nie korzystam z AddOllamaChatClient() ponieważ nie widziałem sensu podawać SP
            IChatClient chatClient = new OllamaApiClient(
                new Uri("http://localhost:11434"),
                "gpt-oss:120b-cloud"
            );

            _aiService = new AiService(chatClient);
        }

        [Test]
        public async Task GetAssignmentGroupsAsync_Return_NetworkAssignmentGroup()
        {
            var incidents = new List<Incident>
            {
                new Incident
                {
                    Number = "INC001",
                    ShortDescription = "The office network is down and no one can connect to the internet."
                }
            };
            //Sprawdza czy AI konsekwentnie przypisuje tę samą grupę przy wielokrotnych zapytaniach
            var aiResponses = new List<AiResponse>();

            for (int i = 0; i < 5; i++)
            {
                var response = await _aiService.GetAssignmentGroupsAsync(incidents, default);
                aiResponses.AddRange(response);
            }

            var correctAnswers = aiResponses.Count(r => r.AssignmentGroup == "Zespół ds. sieci");

            Assert.That(correctAnswers, Is.GreaterThanOrEqualTo(3));
        }
    }
}
