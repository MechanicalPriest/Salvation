using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using NUnit.Framework;
using Salvation.Core;
using Salvation.Core.Constants;
using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Profile.Model;
using Salvation.Core.State;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Salvation.CoreTests
{
    [TestFixture]
    class DependencyInjectionTests
    {
        [SetUp]
        public void Init()
        {

        }

        IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    // Common services
                    services.AddSalvationCore();
                    // Add DI Tester
                    services.AddHostedService<DiTester>();
                });

        [Test]
        public void TryTestDIExtensions()
        {
            Assert.DoesNotThrowAsync(async () => await CreateHostBuilder(null).Build().RunAsync());
        }
    }

    class DiTester : IHostedService
    {
        private readonly IModellingService _modellingService;

        public DiTester(IModellingService modellingService,
            IHostApplicationLifetime lifeTime)
        {
            _modellingService = modellingService;
            lifeTime.StopApplication();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var basePath = "TestData";

            IConstantsService constantsService = new ConstantsService();
            var constants = constantsService.ParseConstants(
                File.ReadAllText(Path.Combine(basePath, "BaseTests_constants.json")));
            var profile = JsonConvert.DeserializeObject<PlayerProfile>(
                File.ReadAllText(Path.Combine(basePath, "BaseTests_profile.json")));

            IGameStateService gameStateService = new GameStateService();

            var gameState = gameStateService.CreateValidatedGameState(profile, constants);

            _modellingService.GetResults(gameState);

            await Task.Delay(1);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.Delay(1);
        }
    }
}
